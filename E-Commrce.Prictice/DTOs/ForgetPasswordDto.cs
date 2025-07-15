namespace E_Commrce.Prictice.DTOs
{
    public class ForgetPasswordDto
    {
        public required string Email { get; set; }
        public required string OtpCode { get; set; }
        public required string NewPassword { get; set; }
    }
}

