
using E_Commrce.Prictice.DTOs;

using E_Commrce.Prictice.Repository.Interface;

using Microsoft.AspNetCore.Mvc;



namespace E_Commrce.Prictice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        //Register OTP 

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            
            var result=await _authRepo.RegisterAsync(dto);
            return result == "User Successfully Registered" ? BadRequest(result) : Ok(result); 

        }

        //Login using Password 

        //  [Authorize(Roles ="Admin,User")]

        [HttpPost("login-password")]
        public async Task<IActionResult> LoginWithPassword(LoginDto dto)
        {
            var token=await _authRepo.LoginWithPasswordAsync(dto);
            return token==null ?Unauthorized("Invalid Information") : Ok(new { token= token });
           
        }



        //Request OTP for login
        //   [Authorize(Roles ="Admin,User")]

        [HttpPost("Request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] string email)
        {
            var result= await _authRepo.RequestOtpAsync(email);
            return result == null ?Unauthorized("Email Not Registered..") : Ok(result);


        }


        //Lgin with OTP
        // [Authorize(Roles = "Admin,User")]
        [HttpPost("login-otp")]
        public async Task<IActionResult> LoginWithOtp(OtpLoginDto dto)
        {
            var token= await _authRepo.LoginWithOtpAsync(dto);

            if (string.IsNullOrEmpty(token))   //if it got the null or empty value it show the true then this message is show 
                return Unauthorized("Invalid OTP or user not found..");  //if the otp is wrong or the not add then it show the error that why we use it 
;
            return Ok(new { Token=token});

        }


        //Forgot password
        // [Authorize(Roles = "Admin,User")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var result= await _authRepo.ForgotPasswordAsync(email);
            return result==null ?BadRequest("Email not registered.."):Ok(result);
          
        }

        //Reset the Password using OTP
        // [Authorize(Roles = "Admin,User")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ForgetPasswordDto dto)
        {
            var result= await _authRepo.ResetPasswordAsync(dto);

            if(string.IsNullOrEmpty(result))
                return Unauthorized("Invalid OTP or user not found..");


            return Ok(result);
         //   return result=="Password Reset Successfully.."? Unauthorized(result) :Ok(result);
        }



    }
}
