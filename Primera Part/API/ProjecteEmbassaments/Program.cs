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

// Endpoints de l'API
app.MapGet("/", () => "API del Projecte d'Embassaments.");

app.MapGet("/dades", (EstacioContext ctx) => 
    ObtenirDadesReals(ctx).ToList());

app.MapGet("/dades/{id}", (int id, EstacioContext ctx) => {
    var dades = ctx.Estacions
        .Where(e => e.Id == id)
        .Select(e => new {
            e.Nom,
            e.Municipi,
            UltimaMesura = e.Mesures
                .OrderByDescending(m => m.Data)
                .FirstOrDefault()
        }).FirstOrDefault();

    return dades is not null ? Results.Ok(dades) : Results.NotFound();
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
