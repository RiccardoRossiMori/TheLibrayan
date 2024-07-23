using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TheLibrayan.Data;
using TheLibrayan.Models;

namespace TheLibrayan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _authContext = AuthContext.Instance;


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin model)
        {
            if (!_authContext.ValidateUser(model.Email, model.Password))
            {
                return Unauthorized();
            }

            var tokenString = GenerateToken(model.Email);
            return Ok(new { Token = tokenString });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegister model)
        {
            if (_authContext.UserExists(model.Email)) return BadRequest("User already exists.");

            _authContext.CreateUser(model.Email, model.Password, model.Nome, model.Cognome);
            return Ok("User registered successfully.");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Invalidate the token (in a real scenario, you might store the invalidated tokens in a blacklist)
            return Ok("Logged out successfully? no, da sistemare ovviamente!");
        }

        private string GenerateToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authContext.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}