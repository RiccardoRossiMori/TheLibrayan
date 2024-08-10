namespace TheLibrayan.Models;

/// <summary>
///     Rappresenta le credenziali di login di un utente.
/// </summary>
public class UserLogin
{
    /// <summary>
    ///     Ottiene o imposta l'email dell'utente.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     Ottiene o imposta la password dell'utente.
    /// </summary>
    public string Password { get; set; }
}