using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost;Database=sql_server_daw;Uid=sa;Pwd=hiqz3652#A;TrustServerCertificate=True;";
builder.Services.AddDbContext<ComarcalContext>(options =>
    options.UseSqlServer(connectionString, new MySqlServerVersion(new Version(8, 0, 31))));
var app = builder.Build();

IQueryable<Estacio> ObtenirDadesReals(ComarcalContext ctx) =>
    from e in ctx.Edificacions
    join p in ctx.Pobles on e.poble_id equals p.id
    join c in ctx.Contribuents on e.contribuent_id equals c.id
    join t in ctx.Tipus on e.tipus_id equals t.id
    select new Estacio {
        Identificador = e.id,
        Adreça = e.direccio,
        Població = p.nom,
        Codi_postal = p.codi_postal,
        TipusImmoble = t.nom,
        MetresQuadrats = e.metres_quadrats,
        PersonesQueViuenEnLaCasa = e.habitants,
        Menors = e.menors,
        Contribuent = c.nom,
        DNIContribuent = c.dni
    };

// Endpoints de l'API
app.MapGet("/", () => "API del Projecte d'Embassaments.");

app.MapGet("/dades", (ComarcalContext db) => 
    ObtenirDadesReals(db).ToList());

app.MapGet("/dades/{id}", (int id, ComarcalContext db) => 
    ObtenirDadesReals(db).FirstOrDefault(d => d.Identificador == id) is Estacio e 
    ? Results.Ok(e) : Results.NotFound());

app.MapGet("/poble/contribuents/{nom}", (string nom, ComarcalContext db) => 
    ObtenirDadesReals(db).Where(d => d.Contribuent == nom).ToList());

app.MapGet("/poble/{cp}/comptar", (int cp, ComarcalContext db) => 
    Results.Ok(new { 
        Poblacio = cp, 
        TotalHabitatges = ObtenirDadesReals(db).Count(d => d.Codi_postal == cp) 
    }));

app.MapGet("/poble/{cp}/contribuents", (int cp, ComarcalContext db) => 
    ObtenirDadesReals(db).Where(d => d.Codi_postal == cp).Select(d => d.Contribuent).Distinct().ToList());

app.MapGet("/propietari/{dni}", (string dni, ComarcalContext db) => 
    ObtenirDadesReals(db).Where(d => d.DNIContribuent == dni).ToList());

app.MapGet("/poble/{cp}", (int cp, ComarcalContext db) => 
    ObtenirDadesReals(db).Where(d => d.Codi_postal == cp).ToList());

app.MapGet("/descomptes", (ComarcalContext db) => 
    ObtenirDadesReals(db).Where(d => d.PersonesQueViuenEnLaCasa > 5).ToList());

app.Run();

// --- DEFINICIÓ DEL CONTEXT DE LA BASE DE DADES ---

public class ComarcalContext : DbContext {
    public ComarcalContext(DbContextOptions<ComarcalContext> options) : base(options) { }
    public DbSet<Estacio> Estacions { get; set; }
    public DbSet<Mesura> Mesures { get; set; }
}


[Table("Estacions")]
    public class Estacio {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Municipi { get; set; } = "";
        public List<Mesura> Mesures { get; set; } = new();
    }
[Table("Mesures")]
    public class Mesura {
        [Key]
        public int Id { get; set; }
        public int EstacioId { get; set; }
        public DateTime Data { get; set; }
        public decimal NivellAbsolut { get; set; }
        public decimal Percentatge { get; set; }
        public decimal Volum { get; set; }
    }
}