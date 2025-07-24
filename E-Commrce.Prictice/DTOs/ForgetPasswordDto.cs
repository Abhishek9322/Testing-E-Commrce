using System.ComponentModel.DataAnnotations;

namespace E_Commrce.Prictice.DTOs
{
    public class ForgetPasswordDto
    {
        [Required]
       // [EmailAddress]
      //  public required string Email { get; set; }
        public required string OtpCode { get; set; }
        public required string NewPassword { get; set; }
    }
}

