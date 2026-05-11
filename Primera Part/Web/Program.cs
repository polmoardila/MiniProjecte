using Microsoft.EntityFrameworkCore;
using EmbassamentDB.Models; // Namespace da sua ClassLib

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Configurar a Connection String e o DbContext
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Database=sql_server_daw;Uid=sa;Pwd=hiqz3652#A;TrustServerCertificate=True;";

        builder.Services.AddDbContext<EstacioContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // 2. Garantir que a base de dados é criada ao iniciar
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EstacioContext>();
            db.Database.EnsureCreated();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}