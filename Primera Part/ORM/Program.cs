using MiniProjecte.Models;

using (var db = new EstacionsContext())
{
    db.Database.EnsureCreated();
}

Importador.Executar();

Console.ReadKey();