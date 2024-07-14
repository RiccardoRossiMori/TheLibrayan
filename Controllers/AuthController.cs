/*using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TheLibrayan.Data;

namespace TheLibrayan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin model)
    {
        if (model.Username != "test" || model.Password != "password") return Unauthorized();
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("your_secret_key_here");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, model.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });

    }

    [HttpGet("test2")]
    public string Test2()//Test2 completamente funzionante. OTTIMO!
    {
        var connection = new SqlConnection();
        connection.ConnectionString = "Server=localhost;Database=TheLibrayan;Integrated Security=True;";
        var x = "";
            connection.Open();

            string query = "SELECT * FROM Libri";
            using (var command = new SqlCommand(query, connection))

            using (var reader = command.ExecuteReader())
            {

                while (reader.Read())
                {
                    int LibroID = reader.GetInt32(reader.GetOrdinal("LibroID"));
                    string Titolo = reader.GetString(reader.GetOrdinal("Titolo"));
                    string Autore = reader.GetString(reader.GetOrdinal("Autore"));
                    int DataPubblicazione = reader.GetInt32(reader.GetOrdinal("DataPubblicazione"));

                    x += ($"ID: {LibroID}, Title: {Titolo}, Author: {Autore}, PublishedYear: {DataPubblicazione}");
                }
            }

            return x;
    }
}

public class UserLogin
{
    public string Username { get; set; }
    public string Password { get; set; }
}

*/

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TheLibrayan.Data;

namespace TheLibrayan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _authContext;
        

        public AuthController()
        {
            _authContext = new AuthContext();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin model)
        {
            if (!_authContext.ValidateUser(model.Email, model.Password)) return Unauthorized();

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
            return Ok("Logged out successfully.");
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegister
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
    }
}


//*/