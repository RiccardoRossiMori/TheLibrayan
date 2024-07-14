namespace TheLibrayan;

public class Categorie
{
    public int CategoriaID { get; set; }
    public string NomeCategoria { get; set; }
    public ICollection<LibriCategorie> LibriCategorie { get; set; }
}