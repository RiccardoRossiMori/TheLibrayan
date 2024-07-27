using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TheLibrayan.Data;

namespace TheLibrayan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly BookContext _bookContext = BookContext.Instance;

    [HttpGet("get-books")]
    public IActionResult GetBooks()
    {
        var books = _bookContext.GetBooks();
        return Ok(books);
    }

    [HttpPost("add-book")]
    public IActionResult AddBook([FromBody] Libri book)
    {
        if (!_bookContext.AddBookIfNotExists(book)) return BadRequest("Book not added");
        var bookJson = JsonConvert.SerializeObject(book);
        return Ok($"Book added: {bookJson}");
    }

    [HttpDelete("delete-book")]
    public IActionResult DeleteBook([FromBody] Libri book)
    {
        if (!_bookContext.DeleteBook(book)) return BadRequest("Book not deleted");
        var bookJson = JsonConvert.SerializeObject(book);
        return Ok($"Book deleted: {bookJson}");
    }
}