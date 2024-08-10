using System.Data.SqlClient;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TheLibrayan.Data;

/// <summary>
///     Fornisce metodi per eseguire comandi e query SQL.
/// </summary>
public class LinkContext
{
    /// <summary>
    ///     Inizializza una nuova istanza della classe <see cref="LinkContext" />.
    /// </summary>
    /// <param name="connectionString">La stringa di connessione per il database.</param>
    /// <param name="secretKey">La chiave segreta per la crittografia o altri scopi.</param>
    protected LinkContext(string connectionString, string secretKey)
    {
        ConnectionString = connectionString;
        SecretKey = secretKey;
    }

    /// <summary>
    ///     Ottiene o imposta la stringa di connessione per il database.
    /// </summary>
    private string ConnectionString { get; }

    /// <summary>
    ///     Ottiene o imposta la chiave segreta per la crittografia o altri scopi.
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    ///     Crea e restituisce un nuovo oggetto <see cref="SqlConnection" />.
    /// </summary>
    /// <returns>Un nuovo oggetto <see cref="SqlConnection" />.</returns>
    private SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    /// <summary>
    ///     Esegue un comando SQL non di query.
    /// </summary>
    /// <param name="query">La query SQL da eseguire.</param>
    /// <param name="parameters">I parametri per la query SQL.</param>
    protected void ExecuteNonQuery(string query, JObject parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = GetSqlCommand(query, parameters, connection);
        command.ExecuteNonQuery();
    }

    /// <summary>
    ///     Crea e restituisce un nuovo oggetto <see cref="SqlCommand" />.
    /// </summary>
    /// <param name="query">La query SQL da eseguire.</param>
    /// <param name="parameters">I parametri per la query SQL.</param>
    /// <param name="connection">L'oggetto <see cref="SqlConnection" />.</param>
    /// <returns>Un nuovo oggetto <see cref="SqlCommand" />.</returns>
    private SqlCommand GetSqlCommand(string query, JObject parameters, SqlConnection connection)
    {
        if (parameters == null)
            Console.WriteLine("I parametri sono nulli.");
        if (connection == null)
        {
            connection = GetConnection();
            connection.Open();
        }

        var command = new SqlCommand(query, connection);
        foreach (var param in parameters)
            command.Parameters.AddWithValue(param.Key, param.Value.ToObject<object>());

        return command;
    }

    /// <summary>
    ///     Esegue un comando SQL scalare e restituisce il risultato.
    /// </summary>
    /// <param name="query">La query SQL da eseguire.</param>
    /// <param name="parameters">I parametri per la query SQL.</param>
    /// <returns>Il risultato della query scalare.</returns>
    protected int ExecuteScalar(string query, JObject parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = GetSqlCommand(query, parameters, connection);
        return (int)command.ExecuteScalar();
    }

    /// <summary>
    ///     Esegue un comando SQL reader e restituisce se sono state lette righe.
    /// </summary>
    /// <param name="query">La query SQL da eseguire.</param>
    /// <param name="parameters">I parametri per la query SQL.</param>
    /// <returns><c>true</c> se sono state lette righe; altrimenti, <c>false</c>.</returns>
    public bool ExecuteReader(string query, JObject parameters)
    {
        using var reader = GetReader(query, parameters);
        return reader.Read();
    }

    /// <summary>
    ///     Crea e restituisce un nuovo oggetto <see cref="SqlDataReader" />.
    /// </summary>
    /// <param name="query">La query SQL da eseguire.</param>
    /// <param name="parameters">I parametri per la query SQL.</param>
    /// <param name="connection">L'oggetto <see cref="SqlConnection" />.</param>
    /// <returns>Un nuovo oggetto <see cref="SqlDataReader" />.</returns>
    private SqlDataReader GetReader(string query, JObject parameters, SqlConnection connection = null)
    {
        var command = GetSqlCommand(query, parameters, connection);
        return command.ExecuteReader();
    }

    /// <summary>
    ///     Esegue una query e restituisce i risultati come una lista di oggetti.
    /// </summary>
    /// <typeparam name="T">Il tipo di oggetti da restituire.</typeparam>
    /// <param name="query">La query SQL da eseguire.</param>
    /// <param name="parameters">I parametri per la query SQL.</param>
    /// <returns>Una lista di oggetti di tipo <typeparamref name="T" />.</returns>
    public List<T> ExecuteQuery<T>(string query, JObject parameters) where T : new()
    {
        var result = new List<T>();
        using var connection = GetConnection();
        connection.Open();
        using var reader = GetReader(query, parameters, connection);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        while (reader.Read())
        {
            var item = new T();
            foreach (var property in properties)
                if (reader[property.Name] != DBNull.Value)
                    property.SetValue(item, reader[property.Name]);

            result.Add(item);
        }

        return result;
    }
}