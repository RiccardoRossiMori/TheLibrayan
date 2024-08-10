namespace TheLibrayan.Models;

/// <summary>
///     Rappresenta i dettagli di registrazione di un utente.
/// </summary>
public class UserRegister : UserLogin
{
    /// <summary>
    ///     Ottiene o imposta il nome dell'utente.
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    ///     Ottiene o imposta il cognome dell'utente.
    /// </summary>
    public string Cognome { get; set; }
}