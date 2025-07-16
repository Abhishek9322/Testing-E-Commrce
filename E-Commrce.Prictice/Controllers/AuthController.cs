using E_Commrce.Prictice.Data;
using E_Commrce.Prictice.DTOs;
using E_Commrce.Prictice.Iinterface;
using E_Commrce.Prictice.Model;
using E_Commrce.Prictice.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace E_Commrce.Prictice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailservice;
        private readonly JwtService _jwt;
        public AuthController(ApplicationDbContext context , IEmailService emailservice ,JwtService jwt)
        {
            _context = context;
            _emailservice = emailservice;  
            _jwt = jwt; 
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
        [HttpPost("login-password")]
        public async Task<IActionResult> LoginWithPassword(LoginDto dto)
        {

            var user=await _context.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid Information..");

            var token = _jwt.GenerateToken(user.Email, dto.RememberMe,user);
            return Ok(new { Token = token });
        }







        //Request OTP for login
        
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] string email)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=>x.Email == email);
            if (user == null) return BadRequest("Email Don't Registerd ");

            var otpCode = new Random().Next(100000, 999999).ToString();
            var otp = new OtpToken
            {
                Email = email,
                Code = otpCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };

            _context.OtpTokens.Add(otp);
            await _context.SaveChangesAsync();

            await _emailservice.SendAsync(email,"Login OTP",$"<h2>Your OTP is <b>{otpCode}</b></h2>");
            return Ok("OTP Sent Successfully..");
        }





        //Verify OTP (does not login just verify)
        
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var otp =await _context.OtpTokens
                .Where(o=>o.Email == dto.Email && o.Code==dto.OtpCode)
                .OrderByDescending(o=>o.ExpiryTime)
                .FirstOrDefaultAsync();


            if (otp == null || otp.ExpiryTime < DateTime.UtcNow)
                return Unauthorized("Invalid or expired OTP.");


            return Ok("OTP is valid..");   
        }





        //Lgin with OTP
        
        [HttpPost("login-otp")]
        public async Task<IActionResult>LoginWithOtp(OtpLoginDto dto)
        {
            var otp = await _context.OtpTokens
                .Where(o => o.Email == dto.Email && o.Code == dto.OtpCode)
                .OrderByDescending(o => o.ExpiryTime)
                .FirstOrDefaultAsync();


            if (otp == null || otp.ExpiryTime < DateTime.UtcNow)
                return Unauthorized("Invalid or expired OTP");


            var user=await _context.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);
            if (user == null) return BadRequest("User not found");


            var token = _jwt.GenerateToken(user.Email, dto.RememberMe ,user);
            return Ok(new {Token=token });
        }


        //Forgot password
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return BadRequest("Email not registered.");

            var otpCode = new Random().Next(100000, 999999).ToString();

            var otp = new OtpToken
            {
                Email = email,
                Code = otpCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };


            _context.OtpTokens.Add(otp);    
            await _context.SaveChangesAsync();


            await _emailservice.SendAsync(email, "Reset Password OTP", $"<h2>Your OTP for Password reset is:<b>{otpCode}</b></h2>");

            return Ok("OTP sent for password reset.");
        }




        //Reset the Password using OTP

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPasswordDto dto)
        {
            var otp = await _context.OtpTokens
                .Where(o => o.Email == dto.Email && o.Code == dto.OtpCode)
                .OrderByDescending(o => o.ExpiryTime)
                .FirstOrDefaultAsync();


            if (otp == null || otp.ExpiryTime < DateTime.UtcNow)
                return Unauthorized("Invaild or Expired OTP.");

            var user=await _context.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);
            if (user == null) return BadRequest("User not found..");


            user.PasswordHash=BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();


            return Ok("Password Reset successfully......");

        }






    }
}
