using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using EmbassamentDB.Models; // Importante para usar Estacio e Mesura

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly EstacioContext _context;

    public HomeController(EstacioContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Obtém a última medida de cada estação para o Dashboard
        var ultimesMesures = await _context.Estacions
            .Select(e => e.Mesures.OrderByDescending(m => m.Data).FirstOrDefault())
            .Where(m => m != null)
            .ToListAsync();

        var model = new DashboardVM
        {
            TotalVolum = ultimesMesures.Sum(m => m.Volum),
            PercentatgeGlobal = ultimesMesures.Any() ? ultimesMesures.Average(m => m.Percentatge) : 0,
            TotalEstacions = await _context.Estacions.CountAsync()
        };

        return View(model);
    }

    public async Task<IActionResult> Detalls(int id, int? year, int? month, DateTime? day)
    {
        var estacio = await _context.Estacions
            .Include(e => e.Mesures)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estacio == null) return NotFound();

        var query = estacio.Mesures.AsQueryable();

        // Lógica de filtros do professor
        if (day.HasValue)
        {
            query = query.Where(m => m.Data.Date == day.Value.Date);
        }
        else if (year.HasValue && month.HasValue)
        {
            query = query.Where(m => m.Data.Year == year && m.Data.Month == month);
        }

        var llistaMesures = await query.OrderByDescending(m => m.Data).ToListAsync();

        var model = new EstacioDetall
        {
            Id = estacio.Id,
            Nom = estacio.Nom,
            Municipi = estacio.Municipi,
            UltimaMesura = llistaMesures.FirstOrDefault(),
            Historic = llistaMesures
        };

        return View(model);
    }

    public async Task<IActionResult> LlistatEmbasaments()
    {
        var lista = await _context.Estacions.ToListAsync();
        return View(lista);
    }
}