﻿namespace E_Commrce.Prictice.Model
{
    public class User
    {
        public int Id {  get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;




        //public string RefreshToken { get; set; }
        //public DateTime RefreshTokenExpiry { get; set; }

        //  public string? OtpCode {  get; set; }
        // public DateTime? OtpExpiresAt { get; set; }
        // public bool IsVerified { get; set; } = false;




    }
}
