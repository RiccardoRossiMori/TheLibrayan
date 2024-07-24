using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace TheLibrayan.Data;

public class LinkContext
{
    protected LinkContext(string connectionString, string secretKey)
    {
        ConnectionString = connectionString;
        SecretKey = secretKey;
    }

    private string ConnectionString { get; set; }
    public string SecretKey { get; set; }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    protected void ExecuteNonQuery(string query, JObject parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = GetSqlCommand(query, parameters, connection);
        command.ExecuteNonQuery();
    }

    private SqlCommand GetSqlCommand(string query, JObject parameters, SqlConnection connection)
    {
        var command = new SqlCommand(query, connection);
        foreach (var param in parameters) command.Parameters.AddWithValue(param.Key, param.Value.ToObject<object>());

        return command;
    }

    protected int ExecuteScalar(string query, JObject parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = GetSqlCommand(query, parameters, connection);
        return (int)command.ExecuteScalar();
    }
    
    public bool ExecuteReader(string query, JObject parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = GetSqlCommand(query, parameters, connection);
        using var reader = command.ExecuteReader();
        return reader.Read();
    }
    
}