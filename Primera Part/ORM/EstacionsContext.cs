using Microsoft.EntityFrameworkCore;

namespace MiniProjecte.Models
{
    public class EstacionsContext : DbContext
    {
        public DbSet<Mesura> Mesures { get; set; }
        public DbSet<Estacio> Estacions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
            .UseSqlServer("Server=localhost,1433;Database=sql_server_daw;User Id=sa;Password=hiqz3652#A;TrustServerCertificate=True;")
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging();
        }
    }
}