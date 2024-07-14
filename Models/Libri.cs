namespace TheLibrayan;

public class Libri
{
    public int LibroID { get; set; }
    public string Titolo { get; set; }
    public string Autore { get; set; }
    public int DataPubblicazione { get; set; }
    public ICollection<LibriCategorie> LibriCategorie { get; set; }
}