using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin model)
        {
            // Проверяем учетные данные пользователя (в реальной ситуации, следует использовать базу данных)
            if (model.Username == "admin" && model.Password == "admin")
            {
                var token = GenerateJwtToken(model.Username);

                return Ok(new { token });
            }


            if (model.Username == "altadmin" && model.Password == "altadmin")
            {
                var token = GenerateJwtToken(model.Username);

                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin") // Присваиваем роль
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsAVeryStrongKeyWithMoreThan32Chars"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
