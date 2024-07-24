using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace TheLibrayan.Data;

public class LinkContext
{
    public LinkContext(string connectionString, string secretKey)
    {
        ConnectionString = connectionString;
        SecretKey = secretKey;
    }

    public string ConnectionString { get; }
    public string SecretKey { get; }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    public void ExecuteNonQuery(string query, JObject parameters)
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

    public int ExecuteScalar(string query, JObject parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = GetSqlCommand(query, parameters, connection);
        return (int)command.ExecuteScalar();
    }
}