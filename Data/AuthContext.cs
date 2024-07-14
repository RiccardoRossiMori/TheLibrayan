using System.Data.SqlClient;

namespace TheLibrayan.Data;

/// <summary>
/// La classe AuthContext gestisce la connessione al database e l'esecuzione delle query per quanto riguarda la tabella Utenti.
/// </summary>
public class AuthContext
{
    public string ConnectionString { get; private set; }
    public string SecretKey { get; private set; }
    public AuthContext (){}
    public AuthContext(string connectionString, string secretKey)
    {
        ConnectionString = connectionString;
        SecretKey = secretKey;
    }
    public bool UserExists(string modelEmail)
    {
        var connection = new SqlConnection();
        connection.ConnectionString = ConnectionString;
        connection.Open();
        var command = new SqlCommand($"SELECT * FROM Utenti WHERE Email = '{modelEmail}'", connection);
        var reader = command.ExecuteReader();
        if (reader.Read())
        {
            connection.Close();
            return true;
        }
        connection.Close();
        return false;
    }
    public bool ValidateUser(string modelEmail, string modelPassword)
    {
        var connection = new SqlConnection();
        connection.ConnectionString = ConnectionString;
        connection.Open();
        var command = new SqlCommand($"SELECT * FROM Utenti WHERE Email = '{modelEmail}' AND Password = '{modelPassword}'", connection);
        var reader = command.ExecuteReader();
        if (reader.Read())
        {
            connection.Close();
            return true;
        }
        connection.Close();
        return false;
    }
    
    
    public void CreateUser(string modelEmail, string modelPassword, string modelNome, string modelCognome)
    {
        throw new NotImplementedException();
    }
    
}