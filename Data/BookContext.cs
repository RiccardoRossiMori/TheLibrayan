using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json.Linq;
using TheLibrayan.Models;

namespace TheLibrayan.Data;

/// <summary>
///     Rappresenta il contesto per l'accesso e la manipolazione dei dati dei libri.
/// </summary>
public class BookContext : LinkContext
{
    private static BookContext? _instance;

    /// <summary>
    ///     Inizializza una nuova istanza della classe <see cref="BookContext" />.
    /// </summary>
    /// <param name="getConnectionString">La stringa di connessione al database.</param>
    /// <param name="secret">La chiave segreta per la crittografia.</param>
    private BookContext(string getConnectionString, string secret) : base(getConnectionString, secret)
    {
        _instance = this;
    }

    /// <summary>
    ///     Ottiene l'istanza singleton della classe <see cref="BookContext" />.
    /// </summary>
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

    /// <summary>
    ///     Recupera tutti i libri dal database.
    /// </summary>
    /// <returns>Una lista di libri.</returns>
    public List<Libri> GetBooks()
    {
        var query = "SELECT * FROM Libri";
        var parameters = new JObject();
        return ExecuteQuery<Libri>(query, parameters);
    }

    /// <summary>
    ///     Aggiunge un libro al database se non esiste già.
    /// </summary>
    /// <param name="book">Il libro da aggiungere.</param>
    /// <returns><c>true</c> se il libro è stato aggiunto; altrimenti, <c>false</c>.</returns>
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

    /// <summary>
    ///     Elimina un libro dal database.
    /// </summary>
    /// <param name="book">Il libro da eliminare.</param>
    /// <returns><c>true</c> se il libro è stato eliminato; altrimenti, <c>false</c>.</returns>
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

    /// <summary>
    ///     Cerca libri in base ai filtri specificati.
    /// </summary>
    /// <param name="categoria">La categoria del libro.</param>
    /// <param name="nomeLibro">Il nome del libro.</param>
    /// <param name="dataPubblicazione">La data di pubblicazione del libro.</param>
    /// <param name="autore">L'autore del libro.</param>
    /// <param name="pageNumber">Il numero della pagina per la paginazione.</param>
    /// <param name="pageSize">Il numero di elementi per pagina.</param>
    /// <returns>Una lista di libri con le loro categorie.</returns>
    public List<LibriECategorie> SearchBooks(string categoria, string nomeLibro, int? dataPubblicazione,
        string autore, int pageNumber, int pageSize)
    {
        var query = new StringBuilder(
            @"SELECT l.LibroID, l.Titolo, l.Autore, l.DataPubblicazione,
                     STRING_AGG(c.NomeCategoria, ',') AS LibriCategorie
              FROM Libri l
              JOIN LibriCategorie lc ON l.LibroID = lc.LibroID
              JOIN Categorie c ON lc.CategoriaID = c.CategoriaID
              WHERE 1=1");
        var parameters = new List<SqlParameter>();

        CheckFilters(categoria, nomeLibro, dataPubblicazione, autore, query, parameters);

        query.Append(" GROUP BY l.LibroID, l.Titolo, l.Autore, l.DataPubblicazione");
        query.Append(" ORDER BY l.Titolo OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
        parameters.Add(new SqlParameter("@Offset", (pageNumber - 1) * pageSize));
        parameters.Add(new SqlParameter("@PageSize", pageSize));

        var jObjectParameters = ConvertSqlParametersToJObject(parameters);

        var results = ExecuteQuery<LibriECategorie>(query.ToString(), jObjectParameters);

        return results.Select(r => new LibriECategorie
        {
            LibroID = r.LibroID,
            Titolo = r.Titolo,
            Autore = r.Autore,
            DataPubblicazione = r.DataPubblicazione,
            LibriCategorie = r.LibriCategorie
        }).ToList();
    }

    /// <summary>
    ///     Verifica e applica i filtri specificati alla query.
    /// </summary>
    /// <param name="categoria">La categoria del libro.</param>
    /// <param name="nomeLibro">Il nome del libro.</param>
    /// <param name="dataPubblicazione">La data di pubblicazione del libro.</param>
    /// <param name="autore">L'autore del libro.</param>
    /// <param name="query">La query a cui aggiungere i filtri.</param>
    /// <param name="parameters">La lista di parametri SQL a cui aggiungere i valori dei filtri.</param>
    private void CheckFilters(string categoria, string nomeLibro, int? dataPubblicazione, string autore,
        StringBuilder query, List<SqlParameter> parameters)
    {
        if (!string.IsNullOrEmpty(categoria) && categoria != "NomeCategoria")
        {
            query.Append(" AND c.NomeCategoria = @NomeCategoria");
            parameters.Add(new SqlParameter("@NomeCategoria", categoria));
        }

        if (!string.IsNullOrEmpty(nomeLibro) && nomeLibro != "NomeLibro")
        {
            query.Append(" AND l.Titolo LIKE @NomeLibro");
            parameters.Add(new SqlParameter("@NomeLibro", $"%{nomeLibro}%"));
        }

        if (dataPubblicazione.HasValue)
        {
            query.Append(" AND l.DataPubblicazione = @DataPubblicazione");
            parameters.Add(new SqlParameter("@DataPubblicazione", dataPubblicazione.Value));
        }

        if (!string.IsNullOrEmpty(autore) && autore != "NomeAutore")
        {
            query.Append(" AND l.Autore LIKE @Autore");
            parameters.Add(new SqlParameter("@Autore", $"%{autore}%"));
        }
    }

    /// <summary>
    ///     Converte una lista di parametri SQL in un JObject.
    /// </summary>
    /// <param name="parameters">La lista di parametri SQL.</param>
    /// <returns>Un JObject contenente i parametri.</returns>
    private JObject ConvertSqlParametersToJObject(List<SqlParameter> parameters)
    {
        var jObject = new JObject();
        foreach (var param in parameters) jObject[param.ParameterName] = JToken.FromObject(param.Value);

        return jObject;
    }

    /// <summary>
    ///     Aggiorna i dettagli di un libro nel database.
    /// </summary>
    /// <param name="book">Il libro da aggiornare.</param>
    /// <returns><c>true</c> se il libro è stato aggiornato; altrimenti, <c>false</c>.</returns>
    public bool UpdateBook(LibriECategorie book)
    {
        var libroId = GetBookId(book);
        if (libroId == null) return false;

        var updateParams = new JObject
        {
            { "@Titolo", book.Titolo },
            { "@Autore", book.Autore },
            { "@DataPubblicazione", book.DataPubblicazione },
            { "@LibroID", libroId }
        };
        ExecuteNonQuery(@"
                UPDATE Libri
                SET Titolo = @Titolo, Autore = @Autore, DataPubblicazione = @DataPubblicazione
                WHERE LibroID = @LibroID", updateParams);

        ExecuteNonQuery("DELETE FROM LibriCategorie WHERE LibroID = @LibroID",
            new JObject { { "@LibroID", libroId } });

        var categories = book.LibriCategorie.Split(',').ToArray();
        return !categories.Any() ||
               categories.All(categoria => UpdateBookCategories(GetCategoriaId(categoria), libroId));
    }

    /// <summary>
    ///     Aggiorna le categorie di un libro nel database.
    /// </summary>
    /// <param name="categoriaId">L'ID della categoria.</param>
    /// <param name="libroId">L'ID del libro.</param>
    /// <returns><c>true</c> se le categorie sono state aggiornate; altrimenti, <c>false</c>.</returns>
    private bool UpdateBookCategories(int? categoriaId, [DisallowNull] int? libroId)
    {
        if (categoriaId == null) return false;

        var insertQuery = "INSERT INTO LibriCategorie (LibroID, CategoriaID) VALUES (@LibroID, @CategoriaID)";
        var insertParams = new JObject
        {
            { "@LibroID", libroId },
            { "@CategoriaID", categoriaId }
        };
        ExecuteNonQuery(insertQuery, insertParams);
        return true;
    }

    /// <summary>
    ///     Recupera l'ID di un libro in base al suo titolo.
    /// </summary>
    /// <param name="book">Il libro di cui ottenere l'ID.</param>
    /// <returns>L'ID del libro, o <c>null</c> se non trovato.</returns>
    private int? GetBookId(LibriECategorie book)
    {
        var query = "SELECT LibroID FROM Libri WHERE Titolo = @Titolo";
        var parameters = new JObject { { "@Titolo", book.Titolo } };
        return ExecuteScalar(query, parameters);
    }

    /// <summary>
    ///     Recupera l'ID di una categoria in base al suo nome.
    /// </summary>
    /// <param name="categoria">Il nome della categoria.</param>
    /// <returns>L'ID della categoria, o <c>null</c> se non trovato.</returns>
    private int? GetCategoriaId(string categoria)
    {
        var query = "SELECT CategoriaID FROM Categorie WHERE NomeCategoria = @NomeCategoria";
        var parameters = new JObject { { "@NomeCategoria", categoria } };
        return ExecuteScalar(query, parameters);
    }
}