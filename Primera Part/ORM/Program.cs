using MiniProjecte.Models;
Console.WriteLine("=== Sistema de Gestió d'Embassaments 2026 ===");
try
{
    using (var db = new EstacionsContext())
    {
        db.Database.EnsureCreated();
    }

    Importador.Executar();
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