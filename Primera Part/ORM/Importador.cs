using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MiniProjecte.Models;

public class Importador
{
    public static void Executar()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { BadDataFound = null };
        using var reader = new StreamReader("C:\\Users\\Pau\\Desktop\\Projecte\\MiniProjecte\\Primera Part\\Arxiu CSV\\mesura.csv");
        using var csv = new CsvReader(reader, config);
        using var db = new EstacionsContext();
        db.Database.EnsureCreated();
        var estacionesCache = db.Estacions.ToList();

        foreach (var r in csv.GetRecords<LiniaCsv>())
        {
            string raw = r.Estacio.Trim(' ', '"');
            string[] partes = raw.Split('(');
            string nombreEstacio = partes[0].Trim();
            string municipio = partes.Length > 1 ? partes[1].Replace(")", "").Trim() : "Desconegut";

            var est = estacionesCache.FirstOrDefault(e => e.Nom == nombreEstacio);
            if (est == null)
            {
                est = new Estacio { Nom = nombreEstacio, Municipi = municipio };
                db.Estacions.Add(est);
                db.SaveChanges();
                estacionesCache.Add(est);
            }
            decimal Convertir(string s) => 
                decimal.TryParse(s?.Trim(' ', '"').Replace(",", "."), CultureInfo.InvariantCulture, out var n) ? n : 0;

            db.Mesures.Add(new Mesura {
                Data = DateTime.Parse(r.Dia.Trim(' ', '"')),
                EstacioId = est.Id,
                NivellAbsolut = Convertir(r.Nivell),
                Percentatge = Convertir(r.Percentatge),
                Volum = Convertir(r.Volum)
            });
        }
        db.SaveChanges();
        Console.WriteLine("Importació finalitzada correctament.");
    }
}