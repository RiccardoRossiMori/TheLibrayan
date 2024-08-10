namespace TheLibrayan.Models;

/// <summary>
///     Rappresenta un libro.
/// </summary>
public class Libri
{
    /// <summary>
    ///     Ottiene o imposta (in locale) l'ID del libro.
    /// </summary>
    public int LibroID { get; set; }

    /// <summary>
    ///     Ottiene o imposta il titolo del libro.
    /// </summary>
    public string Titolo { get; set; }

    /// <summary>
    ///     Ottiene o imposta l'autore del libro.
    /// </summary>
    public string Autore { get; set; }

    /// <summary>
    ///     Ottiene o imposta la data di pubblicazione del libro.
    /// </summary>
    public int DataPubblicazione { get; set; }
}

/// <summary>
///     Rappresenta un libro con le sue categorie.
/// </summary>
public class LibriECategorie : Libri
{
    /// <summary>
    ///     Ottiene o imposta le categorie del libro.
    /// </summary>
    public string LibriCategorie { get; set; }
}