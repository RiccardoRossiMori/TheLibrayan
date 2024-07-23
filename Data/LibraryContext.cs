namespace TheLibrayan.Data;

using System.Data.SqlClient;

/// <summary>
/// La classe LibraryContext gestisce la connessione al database e l'esecuzione delle query per quanto riguarda la tabella Libri.
/// </summary>
public class LibraryContext
{
    private static LibraryContext? _instance;
    public string ConnectionString { get; private set; }
    public string SecretKey { get; }

    private LibraryContext(string connectionString, string secretKey)
    {
        ConnectionString = connectionString;
        SecretKey = secretKey;
        _instance = this;
    }

    public static LibraryContext Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var secretKey = configuration["SecretKey"];
            _instance = new LibraryContext(connectionString, secretKey);
            return _instance;
        }
    }

    public string Prova()
    {
        string x = "";
        var connection = new SqlConnection();
        connection.ConnectionString = "Server=localhost;Database=TheLibrayan;User Id=Librayan;Password=Libraio2024;";
        connection.Open();
        var command = new SqlCommand("SELECT * FROM Libri", connection);
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            x += reader.GetString(1);
            Console.WriteLine(reader.GetString(1));
        }

        connection.Close();
        return x;
    }

    public bool AddCategoryIfNotExists(Categorie category)
    {
        if (!CategoryNameExists(category.NomeCategoria))
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = new SqlCommand("INSERT INTO Categorie (NomeCategoria) VALUES (@NomeCategoria)", connection);
            command.Parameters.AddWithValue("@NomeCategoria", category.NomeCategoria);
            command.ExecuteNonQuery();
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CategoryNameExists(string categoryName)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand("SELECT COUNT(*) FROM Categorie WHERE NomeCategoria = @NomeCategoria", connection);
        command.Parameters.AddWithValue("@NomeCategoria", categoryName);
        int count = (int)command.ExecuteScalar();
        return count > 0;
    }

    public bool DeleteCategoryIfEmpty(Categorie category)
    {
        if (CategoryExists(category.NomeCategoria) && CategoryIsEmpty(category.NomeCategoria))
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var command = new SqlCommand("DELETE FROM Categorie WHERE NomeCategoria = @NomeCategoria", connection);
            command.Parameters.AddWithValue("@NomeCategoria", category.NomeCategoria);
            command.ExecuteNonQuery();
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CategoryExists(string categoryName)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand("SELECT COUNT(*) FROM Categorie WHERE NomeCategoria = @NomeCategoria", connection);
        command.Parameters.AddWithValue("@NomeCategoria", categoryName);
        int count = (int)command.ExecuteScalar();
        return count > 0;
    }

    private bool CategoryIsEmpty(string categoryName)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand("SELECT CategoriaID FROM Categorie WHERE NomeCategoria=@NomeCategoria", connection);
        command.Parameters.AddWithValue("@NomeCategoria", categoryName);
        int categoryId = (int)command.ExecuteScalar();
        command = new SqlCommand("SELECT COUNT(*) FROM LibriCategorie WHERE CategoriaID = @CategoriaID", connection);
        command.Parameters.AddWithValue("@CategoriaID", categoryId);
        int count = (int)command.ExecuteScalar();
        return count == 0;
    }
}