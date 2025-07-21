using E_Commrce.Prictice.Data;
using E_Commrce.Prictice.DTOs;
using E_Commrce.Prictice.Iinterface;
using E_Commrce.Prictice.Model;
using E_Commrce.Prictice.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace E_Commrce.Prictice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailservice;
        private readonly JwtService _jwt;
        private readonly IMemoryCache _cache;
        public AuthController(ApplicationDbContext context , IEmailService emailservice ,JwtService jwt , IMemoryCache cache )
        {
            _context = context;
            _emailservice = emailservice;  
            _jwt = jwt; 
            _cache= cache;
        }


        //Register OTP 

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email Already registered.");


            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) ,
                Role=string.IsNullOrEmpty(dto.Role)?"User":dto.Role
                
            };
             

            _context.Users.Add(user);
            await _context.SaveChangesAsync();




            return Ok("User Successfully Registered");
          

        }





        //Login using Password 

      //  [Authorize(Roles ="Admin,User")]
        [HttpPost("login-password")]
        public async Task<IActionResult> LoginWithPassword(LoginDto dto )
        {

            var user=await _context.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid Information..");

            var token = _jwt.GenerateToken(user.Email, dto.RememberMe,user );

            return Ok(new { Token = token });





        }



        //Request OTP for login
        //   [Authorize(Roles ="Admin,User")]

        [HttpPost("Request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] string email)
        {
            var otp = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
            if (email == null) return Unauthorized("Email  Not Registerd...");


            var otpCode = new Random().Next(100000, 999999).ToString();

            _cache.Set(email, otpCode, TimeSpan.FromMinutes(5));   //store the OTP in cache for temporarily

            await _context.SaveChangesAsync();

            await _emailservice.SendAsync(email, "Login OTP", $"<h2>Your OTP is <b>{otpCode}</b></h2>");

            return Ok("OTP Sent Successfully..");


        }


        //Lgin with OTP
       // [Authorize(Roles = "Admin,User")]
        [HttpPost("login-otp")]
        public async Task<IActionResult>LoginWithOtp(OtpLoginDto dto )
        {

            if(_cache.TryGetValue(dto.Email ,out string cachedOtp)) 
            {

                if (cachedOtp != dto.OtpCode)
                    return Unauthorized("Invalid OTP"); 


                _cache.Remove(dto.Email);


                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null) return BadRequest("User not found");


                  


                var token = _jwt.GenerateToken(dto.Email, dto.RememberMe,user );
                return Ok(new { Token = token });

            }

           
          
               
            return Unauthorized("OTP  Expired or not found...");

           
        }


        //Forgot password
       // [Authorize(Roles = "Admin,User")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return BadRequest("Email not registered.");

            var otpCode = new Random().Next(100000, 999999).ToString();


            _cache.Set(email, otpCode, TimeSpan.FromMinutes(5));

            await _context.SaveChangesAsync();


            await _emailservice.SendAsync(email, "Reset Password OTP", $"<h2>Your OTP for Password reset is:<b>{otpCode}</b></h2>");

            return Ok("OTP sent for password reset.");
        }




        //Reset the Password using OTP
       // [Authorize(Roles = "Admin,User")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPasswordDto dto)
        {
            if (_cache.TryGetValue(dto.Email, out string resetOpt))
            {
                if (resetOpt != dto.OtpCode)
                    return Unauthorized("Invalid OTP ..");


                _cache.Remove(dto.Email);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null) return BadRequest("User not found..");


                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();
            }

            return Ok("Password Reset Successfully......");

        }






    }
}
