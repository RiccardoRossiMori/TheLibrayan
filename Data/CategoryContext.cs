using Newtonsoft.Json.Linq;
using TheLibrayan.Models;

namespace TheLibrayan.Data;

/// <summary>
///     La classe CategoryContext gestisce l'esecuzione delle query per quanto riguarda la tabella Categorie.
/// </summary>
public class CategoryContext : LinkContext
{
    private static CategoryContext? _instance;

    /// <summary>
    ///     Costruttore privato per inizializzare il contesto della categoria.
    /// </summary>
    /// <param name="connectionString">La stringa di connessione al database.</param>
    /// <param name="secretKey">La chiave segreta per la crittografia.</param>
    private CategoryContext(string connectionString, string secretKey) : base(connectionString, secretKey)
    {
        _instance = this;
    }

    /// <summary>
    ///     Ottiene l'istanza singleton del contesto della categoria.
    /// </summary>
    public static CategoryContext Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            _instance = new CategoryContext(configuration.GetConnectionString("DefaultConnection"),
                configuration["SecretKey"]);
            return _instance;
        }
    }

    /// <summary>
    ///     Aggiunge una categoria se non esiste già.
    /// </summary>
    /// <param name="category">La categoria da aggiungere.</param>
    /// <returns>True se la categoria è stata aggiunta, altrimenti false.</returns>
    public bool AddCategoryIfNotExists(Categorie category)
    {
        if (CategoryNameExists(category.NomeCategoria)) return false;
        var query = "INSERT INTO Categorie (NomeCategoria) VALUES (@NomeCategoria)";
        var parameters = new JObject { { "NomeCategoria", category.NomeCategoria } };
        ExecuteNonQuery(query, parameters);
        return true;
    }

    /// <summary>
    ///     Verifica se il nome della categoria esiste già.
    /// </summary>
    /// <param name="categoryName">Il nome della categoria da verificare.</param>
    /// <returns>True se il nome della categoria esiste, altrimenti false.</returns>
    private bool CategoryNameExists(string categoryName)
    {
        var query = "SELECT COUNT(*) FROM Categorie WHERE NomeCategoria = @NomeCategoria";
        var parameters = new JObject { { "NomeCategoria", categoryName } };
        return ExecuteScalar(query, parameters) > 0;
    }

    /// <summary>
    ///     Elimina una categoria se è vuota.
    /// </summary>
    /// <param name="category">La categoria da eliminare.</param>
    /// <returns>True se la categoria è stata eliminata, altrimenti false.</returns>
    public bool DeleteCategoryIfEmpty(Categorie category)
    {
        if (!CategoryExists(category.NomeCategoria) || !CategoryIsEmpty(category.NomeCategoria)) return false;
        var query = "DELETE FROM Categorie WHERE NomeCategoria = @NomeCategoria";
        var parameters = new JObject { { "NomeCategoria", category.NomeCategoria } };
        ExecuteNonQuery(query, parameters);
        return true;
    }

    /// <summary>
    ///     Verifica se una categoria esiste.
    /// </summary>
    /// <param name="categoryName">Il nome della categoria da verificare.</param>
    /// <returns>True se la categoria esiste, altrimenti false.</returns>
    private bool CategoryExists(string categoryName)
    {
        var query = "SELECT COUNT(*) FROM Categorie WHERE NomeCategoria = @NomeCategoria";
        var parameters = new JObject { { "NomeCategoria", categoryName } };
        return ExecuteScalar(query, parameters) > 0;
    }

    /// <summary>
    ///     Verifica se una categoria è vuota.
    /// </summary>
    /// <param name="categoryName">Il nome della categoria da verificare.</param>
    /// <returns>True se la categoria è vuota, altrimenti false.</returns>
    private bool CategoryIsEmpty(string categoryName)
    {
        var query = "SELECT CategoriaID FROM Categorie WHERE NomeCategoria=@NomeCategoria";
        var parameters = new JObject { { "NomeCategoria", categoryName } };
        var categoryId = ExecuteScalar(query, parameters);
        query = "SELECT COUNT(*) FROM LibriCategorie WHERE CategoriaID = @CategoriaID";
        parameters = new JObject { { "CategoriaID", categoryId } };
        return ExecuteScalar(query, parameters) == 0;
    }
}