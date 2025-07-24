using E_Commrce.Prictice.Data;
using E_Commrce.Prictice.DTOs;
using E_Commrce.Prictice.Model;
using E_Commrce.Prictice.Repository.Helper.helperInterface;
using E_Commrce.Prictice.Repository.Interface;
using E_Commrce.Prictice.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace E_Commrce.Prictice.Repository.Service
{

    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly JwtService _jwt;
        private readonly IMemoryCache _Cache;
        private readonly IOtpService _otpService;
        public AuthRepository(ApplicationDbContext context, IEmailService emailService, JwtService jwt, IMemoryCache Cache ,IOtpService otpService)
        {
            _context = context;
            _emailService = emailService;
            _jwt = jwt;
            _Cache = Cache;
            _otpService = otpService;

        }

        //Register user
        public async Task<string> RegisterAsync(RegisterDto dto)
        {

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return "Email Already registered..";

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = string.IsNullOrEmpty(dto.Role) ? "User" : dto.Role,

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User Successfully Regisstered..";
        }


        //Login with password 
        public async Task<string> LoginWithPasswordAsync(LoginDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return _jwt.GenerateToken(user.Email, dto.RememberMe, user);

        }

        //RequestOtp 
        public async Task<string> RequestOtpAsync(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(e => e.Email == email);
            if (user == null)
                return "User not found..";

            var otp = _otpService.GenerateOtp();
            var cacheKey = $"otp_{otp}";

            _Cache.Set(cacheKey,email,TimeSpan.FromMinutes(5));

            await _emailService.SendAsync(email, "Login OTP", $"<h2>Your OTP is <b>{otp}</b></h2>");

            return "OTP Sent Successfully";
        }

        //Login withOtp

        public async Task<string?> LoginWithOtpAsync(OtpLoginDto dto)
        {
            var cacheKey = $"otp_{dto.OtpCode}";

            if (!_Cache.TryGetValue(cacheKey, out string email))
                return null ;
            
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    return null;

                _Cache.Remove(cacheKey);
                return _jwt.GenerateToken(email, dto.RememberMe,user);
            
        }
        //Forget Password 
        public async Task<string> ForgotPasswordAsync([FromBody] string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (user == null)
                return "Email not registered.";

            var otp = _otpService.GenerateOtp();
            var cacheKey = $"otp_{otp}";

            _Cache.Set(cacheKey, email, TimeSpan.FromMinutes(5));

            await _emailService.SendAsync(email, "Reset Password OTP", $"<h2>Your OTP for Password reset is:<b>{otp}</b></h2>");

            return "OTP sent for password reset.";
        }

        //Reset Password 
        public async Task<string> ResetPasswordAsync(ForgetPasswordDto dto)
        {
            var cacheKey = $"otp_{dto.OtpCode}";
            if (!_Cache.TryGetValue(cacheKey, out string email))
                return null;

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            //if (string.IsNullOrWhiteSpace(dto.OtpCode) || resetOtp != dto.OtpCode)
            //    return "Invalid or missing OTP";

            _Cache.Remove(cacheKey);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return "Password Reset Successfully";
        }
    }
}
