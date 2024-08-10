using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TheLibrayan.Data;
using TheLibrayan.Models;

namespace TheLibrayan.Controllers;

/// <summary>
///     Controller per la gestione delle operazioni relative alle categorie.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryContext _categoryContext = CategoryContext.Instance;

    /// <summary>
    ///     Aggiunge una nuova categoria.
    /// </summary>
    /// <param name="category">La categoria da aggiungere.</param>
    /// <returns>Un messaggio di conferma.</returns>
    [HttpPost("add-category")]
    public IActionResult AddCategory([FromBody] Categorie category)
    {
        if (!_categoryContext.AddCategoryIfNotExists(category)) return BadRequest("Categoria non aggiunta");
        var categoryJson = JsonConvert.SerializeObject(category);
        return Ok($"Categoria aggiunta: {categoryJson}");
    }

    /// <summary>
    ///     Elimina una categoria se è vuota.
    /// </summary>
    /// <param name="category">La categoria da eliminare.</param>
    /// <returns>Un messaggio di conferma.</returns>
    [HttpDelete("delete-category")]
    public IActionResult DeleteCategory([FromBody] Categorie category)
    {
        if (!_categoryContext.DeleteCategoryIfEmpty(category)) return BadRequest("Categoria non eliminata");
        var categoryJson = JsonConvert.SerializeObject(category);
        return Ok($"Categoria eliminata: {categoryJson}");
    }
}