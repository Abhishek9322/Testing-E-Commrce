using Microsoft.AspNetCore.Components.Web;

namespace E_Commrce.Prictice.DTOs
{
    public class LoginDto
    {
        public string Email {  get; set; }
        public string Password { get; set; }    

        public bool RememberMe {  get; set; }

    }
}
