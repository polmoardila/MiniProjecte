using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost;Database=sql_server_daw;Uid=sa;Pwd=hiqz3652#A;TrustServerCertificate=True;";
builder.Services.AddDbContext<EstacioContext>(options =>
    options.UseSqlServer(connectionString));
var app = builder.Build();

IQueryable<Estacio> ObtenirDadesReals(EstacioContext ctx) =>
    from m in ctx.Mesures 
    join e in ctx.Estacions on m.EstacioId equals e.Id
    select new Estacio {
        Id = e.Id,
        Nom = e.Nom,
        Municipi = e.Municipi,
    };

app.MapGet("/", () => "API del Projecte d'Embassaments.");

app.MapGet("/estacions", async (EstacioContext ctx) => 
    await ctx.Estacions
        .Select(e => new {
            e.Id,
            e.Nom,
            e.Municipi
        })
        .ToListAsync());
app.MapGet("/dashboard", async (EstacioContext ctx) => {
    var ultimes = await ctx.Estacions
        .Select(e => e.Mesures.OrderByDescending(m => m.Data).FirstOrDefault())
        .Where(m => m != null)
        .ToListAsync();

    if (ultimes.Count == 0) {
        return Results.Ok(new { 
            TotalVolum = 0m, 
            PercentatgeGlobal = 0m, 
            TotalEstacions = 0 
        });
    }
    
    var totalVolum = ultimes.Sum(m => m.Volum);
    var mediaPercentatge = ultimes.Average(m => m.Percentatge);

    return Results.Ok(new {
        TotalVolum = totalVolum,
        PercentatgeGlobal = mediaPercentatge,
        TotalEstacions = ultimes.Count
    });
});

app.MapGet("/embasament/{id}", async (int id, int? year, int? month, string? day, EstacioContext ctx) => {
    
    var query = ctx.Mesures.Where(m => m.EstacioId == id);
    var estacio = await ctx.Estacions.FindAsync(id);
    if (estacio == null) return Results.NotFound();

    // 1. FILTRAR POR DÍA (Estació i dia)
    if (!string.IsNullOrEmpty(day) && DateTime.TryParse(day, out DateTime fechaDia)) {
        var mesuraDia = await query.FirstOrDefaultAsync(m => m.Data.Date == fechaDia.Date);
        return Results.Ok(new {
            estacio.Nom,
            estacio.Municipi,
            UltimaMesura = mesuraDia // Devuelve el dato exacto de ese día
        });
    }

    // 2. FILTRAR POR MES/AÑO (Estació i mes/any: mitjana dels dies)
    if (year.HasValue && month.HasValue) {
        var mesuresMes = await query
            .Where(m => m.Data.Year == year && m.Data.Month == month)
            .ToListAsync();

        if (!mesuresMes.Any()) return Results.NotFound("No hi ha dades per aquest mes.");

        return Results.Ok(new {
            estacio.Nom,
            estacio.Municipi,
            UltimaMesura = new {
                Data = new DateTime(year.Value, month.Value, 1),
                NivellAbsolut = mesuresMes.Average(m => m.NivellAbsolut),
                Percentatge = mesuresMes.Average(m => m.Percentatge),
                Volum = mesuresMes.Average(m => m.Volum)
            }
        });
    }

    // 3. POR DEFECTO: Última medida conocida
    var ultima = await query.OrderByDescending(m => m.Data).FirstOrDefaultAsync();
    return Results.Ok(new { estacio.Nom, estacio.Municipi, UltimaMesura = ultima });
});

app.Run();

// --- DEFINICIÓ DEL CONTEXT DE LA BASE DE DADES ---

public class EstacioContext : DbContext {
    public EstacioContext(DbContextOptions<EstacioContext> options) : base(options) { }
    public DbSet<Estacio> Estacions { get; set; }
    public DbSet<Mesura> Mesures { get; set; }
}


[Table("Estacions")]
    public class Estacio 
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Municipi { get; set; } = "";
        public List<Mesura> Mesures { get; set; } = new();
    }


[Table("Mesures")]
    public class Mesura
    {
        public int Id { get; set; }
        public int EstacioId { get; set; }
        [ForeignKey("EstacioId")]
        public Estacio? Estacio { get; set; }
        public DateTime Data { get; set; }
        public decimal NivellAbsolut { get; set; }
        public decimal Percentatge { get; set; }
        public decimal Volum { get; set; }
    }
