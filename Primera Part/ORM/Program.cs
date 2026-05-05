using MiniProjecte.Models;
Console.WriteLine("=== Sistema de Gestió d'Embassaments 2026 ===");
try
{
    using (var db = new EstacionsContext())
    {
        Console.WriteLine("Connectant amb SQL Server i verificant estructura...");

        db.Database.EnsureCreated();
        Console.WriteLine("Base de dades a punt.");

        if (!db.Estacions.Any())
        {
            Console.WriteLine("La base de dades està buida.");
        }
        else
        {
            var total = db.Estacions.Count();
            Console.WriteLine($"Actualment hi ha {total} estacions a la base de dades.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Error de connexió:");
    Console.WriteLine(ex.Message);
    Console.WriteLine("El contenidor Docker ha d'estar engegat");
}
Console.WriteLine("==============================================");
Console.WriteLine("Prem qualsevol tecla per sortir...");
Console.ReadKey();