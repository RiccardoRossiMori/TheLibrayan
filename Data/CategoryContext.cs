using Newtonsoft.Json.Linq;

namespace TheLibrayan.Data;

/// <summary>
/// La classe CategoryContext gestisce l'esecuzione delle query per quanto riguarda la tabella Categorie.
/// </summary>
public class CategoryContext : LinkContext
{
    private static CategoryContext? _instance;


    private CategoryContext(string connectionString, string secretKey) : base(connectionString, secretKey)
    {
        _instance = this;
    }

    public static CategoryContext Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            _instance = new CategoryContext(configuration.GetConnectionString("DefaultConnection"),
                configuration["SecretKey"]);
            return _instance;
        }
    }

    public bool AddCategoryIfNotExists(Categorie category)
    {
        if (CategoryNameExists(category.NomeCategoria)) return false;
        var query = "INSERT INTO Categorie (NomeCategoria) VALUES (@NomeCategoria)";
        var parameters = new JObject { { "@NomeCategoria", category.NomeCategoria } };
        ExecuteNonQuery(query, parameters);
        return true;
    }

    private bool CategoryNameExists(string categoryName)
    {
        var query = "SELECT COUNT(*) FROM Categorie WHERE NomeCategoria = @NomeCategoria";
        var parameters = new JObject { { "@NomeCategoria", categoryName } };
        return ExecuteScalar(query, parameters) > 0;
    }

    public bool DeleteCategoryIfEmpty(Categorie category)
    {
        if (!CategoryExists(category.NomeCategoria) || !CategoryIsEmpty(category.NomeCategoria)) return false;
        var query = "DELETE FROM Categorie WHERE NomeCategoria = @NomeCategoria";
        var parameters = new JObject { { "@NomeCategoria", category.NomeCategoria } };
        ExecuteNonQuery(query, parameters);
        return true;
    }

    private bool CategoryExists(string categoryName)
    {
        var query = "SELECT COUNT(*) FROM Categorie WHERE NomeCategoria = @NomeCategoria";
        var parameters = new JObject { { "@NomeCategoria", categoryName } };
        return ExecuteScalar(query, parameters) > 0;
    }

    private bool CategoryIsEmpty(string categoryName)
    {
        var query = "SELECT CategoriaID FROM Categorie WHERE NomeCategoria=@NomeCategoria";
        var parameters = new JObject { { "@NomeCategoria", categoryName } };
        var categoryId = ExecuteScalar(query, parameters);
        query = "SELECT COUNT(*) FROM LibriCategorie WHERE CategoriaID = @CategoriaID";
        parameters = new JObject { { "@CategoriaID", categoryId } };
        return ExecuteScalar(query, parameters) == 0;
    }
}