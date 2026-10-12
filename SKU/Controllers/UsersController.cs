//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SKU.DbContexts;
//using System.Text;

//namespace SKU.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
//        private readonly SKUContext _context;

//        public UsersController(SKUContext context)
//        {
//            _context = context;
//        }

//        // 1. Register Endpoint
//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] RegisterDto model)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // التحقق مما إذا كان البريد الإلكتروني موجود مسبقاً
//            if (context.SecUser.AnyAsync(u => u.Email == model.Email))
//                return BadRequest(new { message = "Email is already in use." });

//            // تشفير كلمة المرور (Hashing)
//            string passwordHash = HashPassword(model.Password);

//            var user = new Sec_User
//            {
//                Username = model.Username,
//                Email = model.Email,
//                Password = passwordHash,
//                Phone = model.Phone,
//                UserTypeId = model.UserTypeId, // نوع المستخدم (مثلاً Customer)
//                IsActive = true,
//                IsVerifiedByEmail = false,
//                IsVerifiedByPhone = false
//            };

//            _context.SecUsers.Add(user);
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "User registered successfully", userId = user.UserId });
//        }

//        // 2. Login Endpoint
//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginDto model)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // البحث عن المستخدم بواسطة البريد الإلكتروني أو اسم المستخدم
//            var user = await _context.SecUsers
//                .Include(u => u.UserType)
//                .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Email);

//            if (user == null || !VerifyPassword(model.Password, user.Password))
//                return Unauthorized(new { message = "Invalid email or password." });

//            if (!user.IsActive)
//                return BadRequest(new { message = "User account is inactive." });

//            // تسجيل وقت الدخول الأخير في جدول SecUserLastLogin
//            var lastLogin = new Sec_UserLastLogin
//            {
//                UserId = user.UserId,
//                LastLoginTime = DateTime.UtcNow
//            };

//            _context.SecUserLastLogins.Add(lastLogin);
//            await _context.SaveChangesAsync();

//            // هنا يمكنك لاحقاً توليد وإرجاع الـ JWT Token
//            return Ok(new
//            {
//                message = "Login successful",
//                userId = user.UserId,
//                username = user.Username,
//                userType = user.UserType?.Name
//            });
//        }

//        // 3. Forgot Password Endpoint
//        [HttpPost("forgot-password")]
//        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
//        {
//            var user = await _context.SecUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
//            if (user == null)
//                return NotFound(new { message = "User with this email not found." });

//            // هنا يتم إرسال رمز الاستعادة أو تنفيذ المنطق الخاص بـ Forgot Password

//            return Ok(new { message = "Password reset instructions sent." });
//        }

//#- Helper Methods for Hashing -#
//        private string HashPassword(string password)
//        {
//            using (var sha256 = SHA256.Create())
//            {
//                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
//                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
//            }
//        }

//        private bool VerifyPassword(string enteredPassword, string storedHash)
//        {
//            string hashOfEntered = HashPassword(enteredPassword);
//            return hashOfEntered == storedHash;
//        }
//    }
//}
