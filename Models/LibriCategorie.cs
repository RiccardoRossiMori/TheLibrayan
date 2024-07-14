namespace TheLibrayan;

public class LibriCategorie
{
    public int LibroID { get; set; }
    public Libri Libro { get; set; }
    public int CategoriaID { get; set; }
    public Categorie Categoria { get; set; }
}