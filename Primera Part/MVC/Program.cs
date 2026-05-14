using Microsoft.EntityFrameworkCore;
using EmbassamentDB.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EstacioContext>(options =>
    options.UseSqlServer("Server=localhost;Database=sql_server_daw;Uid=sa;Pwd=hiqz3652#A;TrustServerCertificate=True;"));

var app = builder.Build();
using (var scope = app.Services.CreateScope()) {
    scope.ServiceProvider.GetRequiredService<EstacioContext>().Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();