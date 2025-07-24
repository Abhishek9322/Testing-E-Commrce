using E_Commrce.Prictice.DTOs;

namespace E_Commrce.Prictice.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginWithPasswordAsync(LoginDto dto);
        Task<string> RequestOtpAsync(string email);
        Task<string?> LoginWithOtpAsync(OtpLoginDto dto);
        Task<string> ForgotPasswordAsync(string email);

        Task<string> ResetPasswordAsync(ForgetPasswordDto dto);
    }
}
