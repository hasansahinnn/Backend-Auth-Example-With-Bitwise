using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(LoginFilter))]
public class ProductController : ControllerBase
{
    [HttpGet]
    [SecurityAction((int)SecurityControllers.Product, (long)ProductActions.List)]
    public IActionResult GetList()
    {
        return Ok("Ürünler listelendi.");
    }

    [HttpGet("detail")]
    [SecurityAction((int)SecurityControllers.Product, (long)ProductActions.Detail)]
    public IActionResult GetDetail()
    {
        return Ok("Ürün detayı görüntülendi.");
    }

    [HttpDelete("{id}")]
    [SecurityAction((int)SecurityControllers.Product, (long)ProductActions.Delete)]
    public IActionResult Delete(int id)
    {
        return Ok("Ürün silindi.");
    }

    [HttpGet("public")]
    [Ignore]
    public IActionResult PublicData()
    {
        return Ok("Bu veri herkese açık!");
    }
}