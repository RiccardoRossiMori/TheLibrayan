using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TheLibrayan.Data;
using TheLibrayan.Models;

namespace TheLibrayan.Controllers;

/// <summary>
///     Controller per la gestione delle operazioni relative ai libri.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly BookContext _bookContext = BookContext.Instance;

    /// <summary>
    ///     Recupera tutti i libri.
    /// </summary>
    /// <returns>Una lista di libri.</returns>
    [HttpGet("get-all-books")]
    public IActionResult GetBooks()
    {
        var books = _bookContext.GetBooks();
        return Ok(books);
    }

    /// <summary>
    ///     Aggiunge un nuovo libro.
    /// </summary>
    /// <param name="book">Il libro da aggiungere.</param>
    /// <returns>Un messaggio di conferma.</returns>
    [HttpPost("add-book")]
    public IActionResult AddBook([FromBody] Libri book)
    {
        if (!_bookContext.AddBookIfNotExists(book)) return BadRequest("Libro non aggiunto");
        var bookJson = JsonConvert.SerializeObject(book);
        return Ok($"Libro aggiunto: {bookJson}");
    }

    /// <summary>
    ///     Elimina un libro.
    /// </summary>
    /// <param name="book">Il libro da eliminare.</param>
    /// <returns>Un messaggio di conferma.</returns>
    [HttpDelete("delete-book")]
    public IActionResult DeleteBook([FromBody] Libri book)
    {
        if (!_bookContext.DeleteBook(book)) return BadRequest("Libro non eliminato");
        var bookJson = JsonConvert.SerializeObject(book);
        return Ok($"Libro eliminato: {bookJson}");
    }

    /// <summary>
    ///     Cerca libri in base a vari criteri.
    /// </summary>
    /// <param name="categoria">La categoria del libro.</param>
    /// <param name="nomeLibro">Il nome del libro.</param>
    /// <param name="dataPubblicazione">La data di pubblicazione del libro.</param>
    /// <param name="autore">L'autore del libro.</param>
    /// <param name="pageNumber">Il numero della pagina per la paginazione.</param>
    /// <param name="pageSize">Il numero di elementi per pagina.</param>
    /// <returns>Una lista di libri che corrispondono ai criteri di ricerca.</returns>
    [HttpGet("search-books")]
    public IActionResult SearchBooks([FromQuery] string categoria = "NomeCategoria",
        [FromQuery] string nomeLibro = "NomeLibro", [FromQuery] int? dataPubblicazione = null,
        [FromQuery] string autore = "NomeAutore", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        Console.WriteLine("categoria: " + categoria + " nomeLibro: " + nomeLibro + " dataPubblicazione: " +
                          dataPubblicazione + " autore: " + autore + " pageNumber: " + pageNumber + " pageSize: " +
                          pageSize);
        var books = _bookContext.SearchBooks(categoria, nomeLibro, dataPubblicazione, autore, pageNumber, pageSize);
        return Ok(books);
    }

    /// <summary>
    ///     Aggiorna un libro.
    /// </summary>
    /// <param name="book">Il libro da aggiornare.</param>
    /// <returns>Un messaggio di conferma.</returns>
    [HttpPut("update-book")]
    public IActionResult UpdateBook([FromBody] LibriECategorie book)
    {
        if (!_bookContext.UpdateBook(book)) return BadRequest("Libro non aggiornato");
        var bookJson = JsonConvert.SerializeObject(book);
        return Ok($"Libro aggiornato: {bookJson}");
    }
}