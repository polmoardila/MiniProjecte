using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MiniProjecte.Models;
using Microsoft.EntityFrameworkCore;
public class Importador
{
    public static void Executar()
    {

        // Obtenim un csv reader:
        var csv_config = new CsvConfiguration(CultureInfo.InvariantCulture) { BadDataFound = null };
        var csv_path = "mesura.csv";        
        using var csv_streamreader = new StreamReader(csv_path);
        using var csv_reader = new CsvReader(csv_streamreader, csv_config);

        // Obtenim el dbcontext:
        using var ctx = new EstacionsContext();
        ctx.Database.EnsureCreated();

        var estacionesCache = ctx.Estacions.ToList();

        foreach (var csv_row in csv_reader.GetRecords<LiniaCsv>())
        {
            AfegirMesuraAlaBaseDeDades(ctx, estacionesCache, csv_row);
            
        }

        ctx.SaveChanges();
        Console.WriteLine("Importació finalitzada correctament.");
    }

private static void AfegirMesuraAlaBaseDeDades(EstacionsContext ctx, List<Estacio> estacionesCache, LiniaCsv csv_row)
{
    var (nombreEstacio, municipio) = ExtreuEstacioMunicipi(csv_row);
    var estacio = CreaEstacioSiNoExisteix(ctx, estacionesCache, nombreEstacio, municipio);

    // Funcions de conversió
    decimal Convertir(string s) =>
        decimal.TryParse(s?.Trim(' ', '"').Replace(",", "."), CultureInfo.InvariantCulture, out var n) ? n : 0;

    DateTime DataMesura = DateTime.Parse(csv_row.Dia.Trim(' ', '"'));
    decimal Nivell = Convertir(csv_row.Nivell);
    decimal Perc = Convertir(csv_row.Percentatge);
    decimal Vol = Convertir(csv_row.Volum);

    try 
    {
        // Procediment passant els paràmetres necessaris per registrar la mesura a la base de dades
        ctx.Database.ExecuteSqlRaw("EXEC sp_RegistrarMesura {0}, {1}, {2}, {3}, {4}", 
            DataMesura, estacio.Id, Nivell, Perc, Vol);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en importar fila ({nombreEstacio} - {DataMesura:dd/MM/yyyy}): {ex.Message}");
    }
}

    /// <summary>
    /// Aquesta funció comprova si una estació amb el nom donat ja existeix a la base de dades (utilitzant una cache en memòria per optimitzar les consultes). Si no existeix, crea una nova estació, l'afegeix a la base de dades i a la cache. Retorna l'estació existent o la nova estació creada.
    /// </summary>
    /// <param name="db">El dbcontext</param>
    /// <param name="estacionesCache">Les estacions a la memòria</param>
    /// <param name="nombreEstacio">El nom de l'estació</param>
    /// <param name="municipio">El municipi de l'estació</param>
    /// <returns>L'estació existent o la nova estació creada</returns>
    private static Estacio CreaEstacioSiNoExisteix(EstacionsContext db, List<Estacio> estacionesCache, string nombreEstacio, string municipio)
    {
        var est = estacionesCache.FirstOrDefault(e => e.Nom == nombreEstacio);
        // Si l'estació no existeix, crear-la i afegir-la a la base de dades i a la memòria
        if (est == null)
        {
            est = new Estacio { Nom = nombreEstacio, Municipi = municipio };
            db.Estacions.Add(est);
            db.SaveChanges();
            estacionesCache.Add(est);
        }

        return est;
    }

    private static (string, string) ExtreuEstacioMunicipi(LiniaCsv r)
    {
        string raw = r.Estacio.Trim(' ', '"');
        // Dividir el camp per trobar el nom de l'estació i el municipi (si està present)
        string[] partes = raw.Split('(');
        // El nom de l'estació és la primera part, i el municipi és la segona part (si existeix)
        string nombreEstacio = partes[0].Trim();
        // Si hi ha una segona part, és el municipi; si no, es marca com "Desconegut"
        string municipio = partes.Length > 1 ? partes[1].Replace(")", "").Trim() : "Desconegut";
        return (nombreEstacio, municipio);
    }
}