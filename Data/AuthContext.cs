using System.Data.SqlClient;

namespace TheLibrayan.Data;

/// <summary>
/// La classe AuthContext gestisce la connessione al database e l'esecuzione delle query per quanto riguarda la tabella Utenti.
/// </summary>
public class AuthContext
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

    public AuthContext(string connectionString, string secretKey)
    {
        ConnectionString = connectionString;
        SecretKey = secretKey;
        _instance = this;
    }

    public bool UserExists(string modelEmail)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand($"SELECT * FROM Utenti WHERE Email = '{modelEmail}'", connection);
        using var reader = command.ExecuteReader();
        return reader.Read();
    }

    public bool ValidateUser(string modelEmail, string modelPassword)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand(
            $"SELECT * FROM Utenti WHERE Email = '{modelEmail}' AND Password = '{modelPassword}'", connection);
        using var reader = command.ExecuteReader();
        return reader.Read();
    }


    public void CreateUser(string modelEmail, string modelPassword, string modelNome, string modelCognome)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand(
            $"INSERT INTO Utenti (Email, Password, Nome, Cognome) VALUES ('{modelEmail}', '{modelPassword}', '{modelNome}', '{modelCognome}')",
            connection);
        command.ExecuteNonQuery();
    }
}