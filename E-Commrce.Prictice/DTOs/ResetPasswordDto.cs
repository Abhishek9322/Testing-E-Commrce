﻿namespace E_Commrce.Prictice.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string NewPassword { get; set; }
    }
}

