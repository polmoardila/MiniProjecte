using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

// connexió a la base de dades i definició dels models
var builder = WebApplication.CreateBuilder(args);
var connectionString = "Server=embasaments.c8owd5colahf.us-east-1.rds.amazonaws.com,1433;Database=embasaments;User Id=admin;Password=pyqju6-buvpeP-bifto;TrustServerCertificate=True;";builder.Services.AddDbContext<EstacioContext>(options =>
    options.UseSqlServer(connectionString));
var app = builder.Build();

//enpoints

app.MapGet("/", () => "API del Projecte d'Embassaments.");

app.MapGet("/estacions", async (EstacioContext ctx) => 
    await ctx.Estacions
        .Select(e => new { e.Id, e.Nom, e.Municipi })
        .ToListAsync());

app.MapGet("/dashboard", async (EstacioContext ctx) => {
    var ultimes = await ctx.Estacions
        .Select(e => e.Mesures.OrderByDescending(m => m.Data).FirstOrDefault())
        .Where(m => m != null)
        .ToListAsync();

// si no hi ha mesures, retornar valors per defecte
    if (!ultimes.Any()) return Results.Ok(new { TotalVolum = 0m, PercentatgeGlobal = 0m, TotalEstacions = 0 });
// calcular i retornar les estadístiques del dashboard
    return Results.Ok(new {
        TotalVolum = ultimes.Sum(m => m.Volum),
        PercentatgeGlobal = ultimes.Average(m => m.Percentatge),
        TotalEstacions = ultimes.Count
    });
});

//endpoint per obtenir les dades d'una estació específica.

app.MapGet("/embasament/{id}", async (int id, int? year, int? month, string? day, EstacioContext ctx) => {
    var query = ctx.Mesures.Where(m => m.EstacioId == id);
    var estacio = await ctx.Estacions.FindAsync(id);
    if (estacio == null) return Results.NotFound();
    
    // Aplicar filtres de data si es proporcionen
    var ultima = await query.OrderByDescending(m => m.Data).FirstOrDefaultAsync();
    return Results.Ok(new { estacio.Nom, estacio.Municipi, UltimaMesura = ultima });
});

app.Run();

// models i context

public class EstacioContext : DbContext {
    public EstacioContext(DbContextOptions<EstacioContext> options) : base(options) { }
    public DbSet<Estacio> Estacions { get; set; }
    public DbSet<Mesura> Mesures { get; set; }
}

[Table("Estacions")]
public class Estacio {
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public string Municipi { get; set; } = "";
    public List<Mesura> Mesures { get; set; } = new();
}

[Table("Mesures")]
public class Mesura {
    public int Id { get; set; }
    public int EstacioId { get; set; }
    [ForeignKey("EstacioId")] public Estacio? Estacio { get; set; }
    public DateTime Data { get; set; }
    public decimal NivellAbsolut { get; set; }
    public decimal Percentatge { get; set; }
    public decimal Volum { get; set; }
}