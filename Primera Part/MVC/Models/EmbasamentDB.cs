using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmbassamentDB.Models;

public class EstacioContext : DbContext {
    public EstacioContext(DbContextOptions<EstacioContext> options) : base(options) { }
    public DbSet<Estacio> Estacions { get; set; }
    public DbSet<Mesura> Mesures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Estacio>()
            .HasMany(e => e.Mesures)
            .WithOne(m => m.Estacio)
            .HasForeignKey(m => m.EstacioId);
    }
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