using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public async Task<IActionResult> Detalls(int id)
{
    // Buscamos la estación e incluimos sus medidas
    var estacio = await _context.Estacions
        .Include(e => e.Mesures)
        .FirstOrDefaultAsync(e => e.Id == id);

    if (estacio == null) return NotFound();

    // Pasamos la estación a la vista. 
    // La vista se encargará de buscar la medida más nueva.
    return View(estacio);
}
}
