using E_Commrce.Prictice.Iinterface;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace E_Commrce.Prictice.Service
{
    public class EmailService : IEmailService
    {
        public async Task SendAsync(string to, string subject, string bodyHtml)
        {
            var fromEmail = "kambleabhishek3303@gmail.com";        //sender email               //  Must match the credential below
            var appPassword = "wdlaplhwcmbuxwmm";                               //  App password only Create fromt the Google verification 

            var mail = new MailMessage(fromEmail, to, subject, bodyHtml)
            {
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, appPassword)
            };

            await smtp.SendMailAsync(mail);
        }


       
    }

}
//[HttpPost("verify-otp")]
//public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
//{
//    if(_cache.TryGetValue(dto.Email,out string cachedOtp))
//    {

//        if(cachedOtp==dto.OtpCode)
//        {
//            _cache.Remove(dto.Email);
//            return Ok(" OTP is valid..");
//        }

//        return Unauthorized("Invalid OTP...");
//    }


//    return Ok("OTP Expired or not Found...");   
//}
