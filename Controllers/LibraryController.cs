using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TheLibrayan.Data;

namespace TheLibrayan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LibraryController: ControllerBase
{
  private readonly AuthContext _authContext = AuthContext.Instance;
  private readonly LibraryContext _libraryContext = LibraryContext.Instance;

  //TODO: non ricordo se funziona o meno, da testare in maniera appropriata più avanti ed implementare in maniera corretta su altri metodi.
    [Authorize]
    [HttpGet("test-token")]
    public IActionResult TestToken()
    {
        return Ok("ok, bro");
    }
    
    [HttpPost("add-category")]
    public IActionResult AddCategory([FromBody] Categorie category)
    {
      if (_libraryContext.AddCategoryIfNotExists(category))
      {
        var categoryJson = JsonConvert.SerializeObject(category);
        return Ok($"Category added: {categoryJson}");
      }
      else
      {
        return BadRequest("Category not added");
      }
    }
    
    [HttpDelete("delete-category")]
    public IActionResult DeleteCategory([FromBody] Categorie category)
    {
      if (_libraryContext.DeleteCategoryIfEmpty(category))
      {
        var categoryJson = JsonConvert.SerializeObject(category);
        return Ok($"Category deleted: {categoryJson}");
      }
      else
      {
        return BadRequest("Category not deleted");
      }
    }
}