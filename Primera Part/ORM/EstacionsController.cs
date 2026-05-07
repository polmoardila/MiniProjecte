using Microsoft.AspNetCore.Mvc;
using MiniProjecte.Models;

namespace MiniProjecte.Controllers 
{
    [Route("api/[controller]")] 
    [ApiController]
    public class EstacionsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("La API funciona! Si veus això, el problema era la configuració del controlador.");
        }
    }
}