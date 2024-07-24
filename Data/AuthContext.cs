using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace TheLibrayan.Data;

/// <summary>
/// La classe AuthContext gestisce l'esecuzione delle query per quanto riguarda la tabella Utenti.
/// </summary>
public class AuthContext : LinkContext
{
    public string ConnectionString { get; private set; }
    public string SecretKey { get; }

    private static AuthContext? _instance;

    public static AuthContext Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var secretKey = configuration["SecretKey"];
            _instance = new AuthContext(connectionString, secretKey);
            return _instance;
        }
    }

    public AuthContext(string connectionString, string secretKey): base(connectionString, secretKey)
    {
        ConnectionString = connectionString;
        SecretKey = secretKey;
        _instance = this;
    }

    public bool UserExists(string modelEmail)
    {
        var query = "SELECT * FROM Utenti WHERE Email = @Email";
        var parameters = new JObject { { "@Email", modelEmail } };
        return ExecuteReader(query, parameters);
    }

    public bool ValidateUser(string modelEmail, string modelPassword)
    {
        var query = "SELECT * FROM Utenti WHERE Email = @Email AND Password = @Password";
        var parameters = new JObject { { "@Email", modelEmail }, { "@Password", modelPassword } };
        return ExecuteReader(query, parameters);
    }


    public void CreateUser(string modelEmail, string modelPassword, string modelNome, string modelCognome)
    {
        var query = "INSERT INTO Utenti (Email, Password, Nome, Cognome) VALUES (@Email, @Password, @Nome, @Cognome)";
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