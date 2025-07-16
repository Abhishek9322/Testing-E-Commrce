using E_Commrce.Prictice.Iinterface;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace E_Commrce.Prictice.Service
{
    public class EmailService : IEmailService
    {
        public async Task SendAsync(string to, string subject, string bodyHtml)
        {
            var fromEmail = "kambleabhishek3303@gmail.com";                       //  Must match the credential below
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
