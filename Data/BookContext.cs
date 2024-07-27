using Newtonsoft.Json.Linq;

namespace TheLibrayan.Data;

public class BookContext : LinkContext
{
    private static BookContext? _instance;

    private BookContext(string getConnectionString, string secret) : base(getConnectionString, secret)
    {
        _instance = this;
    }


    public static BookContext Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            _instance = new BookContext(configuration.GetConnectionString("DefaultConnection"),
                configuration["SecretKey"]);
            return _instance;
        }
    }


    public List<Libri> GetBooks()
    {
        var query = "SELECT * FROM Libri";
        var parameters = new JObject();
        return ExecuteQuery<Libri>(query, parameters);
    }

    public bool AddBookIfNotExists(Libri book)
    {
        var checkQuery = "SELECT COUNT(*) FROM Libri WHERE LibroID = @LibroID";
        var checkParameters = new JObject { { "@LibroID", book.LibroID } };
        var exists = ExecuteScalar(checkQuery, checkParameters) > 0;

        if (exists) return false;

        var insertQuery =
            @"INSERT INTO Libri (Titolo, Autore, DataPubblicazione) VALUES (@Titolo, @Autore, @DataPubblicazione)";
        var insertParameters = new JObject
        {
            { "@Titolo", book.Titolo },
            { "@Autore", book.Autore },
            { "@DataPubblicazione", book.DataPubblicazione }
        };
        ExecuteNonQuery(insertQuery, insertParameters);
        return true;
    }

    public bool DeleteBook(Libri book)
    {
        var checkQuery = "SELECT COUNT(*) FROM Libri WHERE LibroID = @LibroID";
        var checkParameters = new JObject { { "@LibroID", book.LibroID } };
        var exists = ExecuteScalar(checkQuery, checkParameters) > 0;

        if (!exists) return false;

        var deleteQuery = "DELETE FROM Libri WHERE LibroID = @LibroID";
        var deleteParameters = new JObject { { "@LibroID", book.LibroID } };
        ExecuteNonQuery(deleteQuery, deleteParameters);
        return true;
    }
}