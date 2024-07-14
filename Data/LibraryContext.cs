namespace TheLibrayan.Data;
using System.Data.SqlClient;

/// <summary>
/// La classe LibraryContext gestisce la connessione al database e l'esecuzione delle query per quanto riguarda la tabella Libri.
/// </summary>
public class LibraryContext
{

    public string Prova()
    {
        string x="";
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
}