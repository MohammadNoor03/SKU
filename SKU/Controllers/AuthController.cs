using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SKU.DbContexts;
using SKU.Dtos.Auth;
using SKU.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SKU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Sec_UserType => Customer (نفس الـ Id المستخدم بالـ Seeding)
        private const long CustomerUserTypeId = 1;

        private readonly SKUContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(SKUContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
        {
            var user = _dbContext.Sec_User
                .Select(x => new
                {
                    User = x,
                    UserTypeName = x.UserType_Lookup.Name
                })
                .FirstOrDefault(x => x.User.Email.ToUpper() == loginDto.Email.ToUpper());

            if (user == null)
            {
                return Unauthorized("Invalid Email Or Password"); // 401
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.User.Password))
            {
                return Unauthorized("Invalid Email Or Password"); // 401
            }

            if (!user.User.IsActive)
            {
                return BadRequest("User Account Is Inactive");
            }

            // Last Login => Sec_Login
            _dbContext.Sec_Login.Add(new UserLastLogins()
            {
                Id = 0,
                UserId = user.User.UserId,
                LastLoginTime = DateTime.UtcNow
            });
            _dbContext.SaveChanges();

            // Token => JwtBearer
            var token = GenerateJwtToken(user.User, user.UserTypeName);

            return Ok(token);
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto registerDto)
        {
            var emailExist = _dbContext.Sec_User.Any(x => x.Email.ToUpper() == registerDto.Email.ToUpper());
            if (emailExist)
            {
                return BadRequest("Email Already Exist");
            }

            var usernameExist = _dbContext.Sec_User.Any(x => x.Username.ToUpper() == registerDto.Username.ToUpper());
            if (usernameExist)
            {
                return BadRequest("Username Already Exist");
            }

            var user = new User()
            {
                // لا تعطي قيمة لل id لما بدك تبعثوا على DB
                UserId = 0,
                Username = registerDto.Username,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                UserTypeId = CustomerUserTypeId, // التسجيل العام دائماً Customer
                IsActive = true,
                IsVerifiedByEmail = false,
                IsVerifiedByPhone = false
            };

            _dbContext.Sec_User.Add(user);
            _dbContext.SaveChanges();

            return Ok(user.UserId);
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = _dbContext.Sec_User.FirstOrDefault(x => x.Email.ToUpper() == forgotPasswordDto.Email.ToUpper());

            if (user != null)
            {
                // TODO: توليد رمز الاستعادة وتخزينه (يحتاج جدول) وإرساله بالإيميل
            }

            // نفس الرد دائماً حتى ما ينعرف إذا الإيميل موجود أو لا
            return Ok("If The Email Exists, Reset Instructions Have Been Sent");
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
