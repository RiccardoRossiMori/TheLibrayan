using Newtonsoft.Json.Linq;

namespace TheLibrayan.Data;

/// <summary>
///     La classe AuthContext gestisce l'esecuzione delle query per quanto riguarda la tabella Utenti.
/// </summary>
public class AuthContext : LinkContext
{
    private static AuthContext? _instance;

    /// <summary>
    ///     Costruttore per inizializzare il contesto di autenticazione.
    /// </summary>
    /// <param name="connectionString">La stringa di connessione al database.</param>
    /// <param name="secretKey">La chiave segreta per la crittografia.</param>
    public AuthContext(string connectionString, string secretKey) : base(connectionString, secretKey)
    {
        _instance = this;
    }

    /// <summary>
    ///     Ottiene l'istanza singleton del contesto di autenticazione.
    /// </summary>
    public static AuthContext Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var secretKey = configuration["SecretKey"];
            _instance = new AuthContext(connectionString, secretKey);
            return _instance;
        }
    }

    /// <summary>
    ///     Verifica se un utente esiste nel database.
    /// </summary>
    /// <param name="modelEmail">L'email dell'utente da verificare.</param>
    /// <returns>True se l'utente esiste, altrimenti false.</returns>
    public bool UserExists(string modelEmail)
    {
        var query = "SELECT * FROM Utenti WHERE Email = @Email";
        var parameters = new JObject { { "@Email", modelEmail } };
        return ExecuteReader(query, parameters);
    }

    /// <summary>
    ///     Valida le credenziali di un utente.
    /// </summary>
    /// <param name="modelEmail">L'email dell'utente.</param>
    /// <param name="modelPassword">La password dell'utente.</param>
    /// <returns>True se le credenziali sono valide, altrimenti false.</returns>
    public bool ValidateUser(string modelEmail, string modelPassword)
    {
        var query = "SELECT * FROM Utenti WHERE Email = @Email AND Password = @Password";
        var parameters = new JObject { { "@Email", modelEmail }, { "@Password", modelPassword } };
        return ExecuteReader(query, parameters);
    }

    /// <summary>
    ///     Crea un nuovo utente nel database.
    /// </summary>
    /// <param name="modelEmail">L'email dell'utente.</param>
    /// <param name="modelPassword">La password dell'utente.</param>
    /// <param name="modelNome">Il nome dell'utente.</param>
    /// <param name="modelCognome">Il cognome dell'utente.</param>
    public void CreateUser(string modelEmail, string modelPassword, string modelNome, string modelCognome)
    {
        var query =
            "INSERT INTO Utenti (Email, Password, Nome, Cognome) VALUES (@Email, @Password, @Nome, @Cognome)";
        var parameters = new JObject
        {
            ["@Email"] = modelEmail,
            ["@Password"] = modelPassword,
            ["@Nome"] = modelNome,
            ["@Cognome"] = modelCognome
        };
        ExecuteNonQuery(query, parameters);
    }
}