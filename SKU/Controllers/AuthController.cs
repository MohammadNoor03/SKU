using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SKU.DbContexts;
using SKU.Dtos.Auth;
using SKU.Models;
using SKU.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SKU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Sec_UserType => Customer (same Id used in seeding)
        private const long CustomerUserTypeId = 1;

        private const int PhoneCodeLength = 5;
        private const int ResetCodeLength = 6;

        private readonly SKUContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly VerificationService _verification;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;

        public AuthController(
            SKUContext dbContext,
            IConfiguration configuration,
            VerificationService verification,
            ISmsSender smsSender,
            IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _verification = verification;
            _smsSender = smsSender;
            _emailSender = emailSender;
        }

        // ---------------------------------------------------------------- Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _dbContext.Sec_User
                .Select(x => new
                {
                    User = x,
                    UserTypeName = x.UserType_Lookup.Name
                })
                .FirstOrDefaultAsync(x => x.User.Email.ToUpper() == loginDto.Email.ToUpper());

            if (user == null || !IsPasswordValid(loginDto.Password, user.User.Password))
            {
                return Unauthorized("Invalid Email Or Password"); // 401
            }

            if (!user.User.IsActive)
            {
                return BadRequest("User Account Is Inactive");
            }

            // Last Login => Sec_UserLastLogin
            _dbContext.Sec_UserLastLogin.Add(new UserLastLogins()
            {
                Id = 0,
                UserId = user.User.UserId,
                LastLoginTime = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            // Token => JwtBearer
            var token = GenerateJwtToken(user.User, user.UserTypeName);

            return Ok(token);
        }

        // ------------------------------------------------------------- Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var emailExist = await _dbContext.Sec_User.AnyAsync(x => x.Email.ToUpper() == registerDto.Email.ToUpper());
            if (emailExist)
            {
                return BadRequest("Email Already Exist");
            }

            var usernameExist = await _dbContext.Sec_User.AnyAsync(x => x.Username.ToUpper() == registerDto.Username.ToUpper());
            if (usernameExist)
            {
                return BadRequest("Username Already Exist");
            }

            var user = new User()
            {
                // Do not set the id when sending to DB
                UserId = 0,
                Username = registerDto.Username,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                UserTypeId = CustomerUserTypeId, // Public registration is always Customer
                IsActive = true,
                IsVerifiedByEmail = false,
                IsVerifiedByPhone = false
            };

            _dbContext.Sec_User.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok(user.UserId);
        }

        // ------------------------------------------------------- Phone verification
        // Requires login (Authorize button in Swagger => paste the token from /login)
        [Authorize]
        [HttpPost("send-phone-code")]
        public async Task<IActionResult> SendPhoneCode()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.IsVerifiedByPhone)
            {
                return BadRequest("Phone Already Verified");
            }

            var code = await _verification.IssueAsync(user.UserId, VerificationPurpose.PhoneVerification, PhoneCodeLength);
            if (code == null)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, "Please Wait Before Requesting Another Code");
            }

            await _smsSender.SendAsync(
                user.PhoneNumber,
                $"Your SKU verification code is {code}. It expires in {VerificationService.CodeLifetime.TotalMinutes:0} minutes.");

            return Ok("Verification Code Sent");
        }

        [Authorize]
        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone(VerifyPhoneDto dto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.IsVerifiedByPhone)
            {
                return BadRequest("Phone Already Verified");
            }

            var valid = await _verification.VerifyAsync(user.UserId, VerificationPurpose.PhoneVerification, dto.Code);
            if (!valid)
            {
                return BadRequest("Invalid Or Expired Code");
            }

            user.IsVerifiedByPhone = true;
            await _dbContext.SaveChangesAsync();

            return Ok("Phone Verified Successfully");
        }

        // ------------------------------------------------------- Password reset
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _dbContext.Sec_User
                .FirstOrDefaultAsync(x => x.Email.ToUpper() == forgotPasswordDto.Email.ToUpper());

            if (user != null && user.IsActive)
            {
                var code = await _verification.IssueAsync(user.UserId, VerificationPurpose.PasswordReset, ResetCodeLength);
                if (code != null)
                {
                    await _emailSender.SendAsync(
                        user.Email,
                        "SKU Password Reset Code",
                        $"Your password reset code is {code}. It expires in {VerificationService.CodeLifetime.TotalMinutes:0} minutes.");
                }
            }

            // Same response always, so nobody can find out if the email exists
            return Ok("If The Email Exists, Reset Instructions Have Been Sent");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _dbContext.Sec_User
                .FirstOrDefaultAsync(x => x.Email.ToUpper() == dto.Email.ToUpper());

            if (user == null || !user.IsActive)
            {
                return BadRequest("Invalid Or Expired Code");
            }

            var valid = await _verification.VerifyAsync(user.UserId, VerificationPurpose.PasswordReset, dto.Code);
            if (!valid)
            {
                return BadRequest("Invalid Or Expired Code");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _dbContext.SaveChangesAsync();

            return Ok("Password Reset Successfully");
        }

        // ---------------------------------------------------------------- Helpers
        private static bool IsPasswordValid(string plainPassword, string storedHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Stored value is not a valid BCrypt hash => treat as wrong password
                return false;
            }
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(idValue, out var userId))
            {
                return null;
            }

            return await _dbContext.Sec_User.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Token
        private string GenerateJwtToken(User user, string userTypeName)
        {
            // Claims => User Info
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            claims.Add(new Claim(ClaimTypes.Role, userTypeName)); // Customer, Admin...

            // Secret Key + Signing Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token Settings
            var tokenSettings = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddDays(1)
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenSettings);
        }
    }
}