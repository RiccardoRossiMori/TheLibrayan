using System.Data.SqlClient;
using System.Reflection;
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
        if (parameters == null)
            Console.WriteLine(
                "diocristobabbione???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????");
        if (connection == null)
        {
            connection = GetConnection();
            connection.Open();
        }

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
        using var reader = GetReader(query, parameters);
        return reader.Read();
    }

    private SqlDataReader GetReader(string query, JObject parameters, SqlConnection connection = null)
    {
        var command = GetSqlCommand(query, parameters, connection);
        return command.ExecuteReader();
    }


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

    public T ExecuteQuerySingle<T>(string query, JObject parameters) where T : new()
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = GetSqlCommand(query, parameters, connection);
        using var reader = command.ExecuteReader();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (!reader.Read()) return default;
        var item = new T();
        foreach (var property in properties)
            if (reader[property.Name] != DBNull.Value)
                property.SetValue(item, reader[property.Name]);

        return item;
    }
}