using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;


namespace E_Commrce.Prictice.Service
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config) 
        {
            _config = config;   
        }

        public string GenerateToken(string email,bool rememberMe)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiry = rememberMe
                ? DateTime.UtcNow.AddDays(int.Parse(_config["JwtSettings:ExpiryDays"]))
                : DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryMinutes"]));

            var token = new JwtSecurityToken(

                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettigns:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds

                );

            return new JwtSecurityTokenHandler().WriteToken(token); 


        }

    }
}
