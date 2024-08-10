using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TheLibrayan.Data;
using TheLibrayan.Models;

namespace TheLibrayan.Controllers;

/// <summary>
///     Controller per gestire le operazioni relative all'autenticazione.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthContext _authContext = AuthContext.Instance;

    /// <summary>
    ///     Autentica un utente e genera un token JWT.
    /// </summary>
    /// <param name="model">Il modello di login dell'utente contenente email e password.</param>
    /// <returns>Un IActionResult contenente il token JWT se l'autenticazione ha successo.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin model)
    {
        if (!_authContext.ValidateUser(model.Email, model.Password)) return Unauthorized();

        var tokenString = GenerateToken(model.Email);
        return Ok(new { Token = tokenString });
    }

    /// <summary>
    ///     Registra un nuovo utente.
    /// </summary>
    /// <param name="model">Il modello di registrazione dell'utente contenente i dettagli dell'utente.</param>
    /// <returns>Un IActionResult che indica il risultato della registrazione.</returns>
    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegister model)
    {
        if (_authContext.UserExists(model.Email)) return BadRequest("L'utente esiste già.");

        _authContext.CreateUser(model.Email, model.Password, model.Nome, model.Cognome);
        return Ok("Utente registrato con successo.");
    }

    /// <summary>
    ///     Effettua il logout dell'utente.
    /// </summary>
    /// <returns>Un IActionResult che indica il risultato dell'operazione di logout.</returns>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Invalida il token (in uno scenario reale, potresti memorizzare i token invalidati in una blacklist)
        return Ok("Logout effettuato con successo.");
    }

    /// <summary>
    ///     Genera un token JWT per l'utente autenticato.
    /// </summary>
    /// <param name="email">L'email dell'utente autenticato.</param>
    /// <returns>Il token JWT generato come stringa.</returns>
    private string GenerateToken(string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authContext.SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        return tokenString;
    }
}