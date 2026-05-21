using Microsoft.EntityFrameworkCore;

namespace MiniProjecte.Models
{
    public class EstacionsContext : DbContext
    {
        public DbSet<Mesura> Mesures { get; set; }
        public DbSet<Estacio> Estacions { get; set; }
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        optionsBuilder.UseSqlServer("Server=embasaments.c8owd5colahf.us-east-1.rds.amazonaws.com,1433;Database=embasaments;Uid=admin;Pwd=pyqju6-buvpeP-bifto;TrustServerCertificate=True;");
    }
}
    }
}