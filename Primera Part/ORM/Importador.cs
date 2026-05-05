using System;
using System.IO;
using System.Linq;
using System.Globalization;
using MiniProjecte.Models;

public class Importador
{
    //Ruta absoluta del CSV 
    private const string RutaCsv = "C:\\Users\\Pau\\Desktop\\Projecte\\MiniProjecte\\Primera Part\\Arxiu CSV\\mesura.csv";

    public static void Executar()
    {
        try 
        {
            using var db = new EstacionsContext();


        // Comproba si el fitxer existeix abans de intentar llegir-lo
            if (!File.Exists(RutaCsv))
            {
                Console.WriteLine("Fitxer CSV no trobat a la carpeta de l'executable.");
                return;
            }

            var linies = File.ReadAllLines(RutaCsv);

            for (int i = 1; i < linies.Length; i++)
            {
                var fila = linies[i];
                if (string.IsNullOrWhiteSpace(fila)) continue;

                var camps = fila.Split(','); 

                try 
                {
                    string dataTxt = camps[0].Trim();
                    string nomEstacio = camps[1].Trim();

                    var estacio = db.Estacions.FirstOrDefault(e => e.Nom == nomEstacio);
                    
                    if (estacio == null)
                    {
                        estacio = new Estacio { Nom = nomEstacio, Municipi = "Catalunya" };
                        db.Estacions.Add(estacio);
                        db.SaveChanges(); 
                        Console.WriteLine($"🆕 Nova estació creada: {nomEstacio}");
                    }

                    // --- PASO 2: CONVERSIÓN DE DATOS ---
                    // Convertimos la fecha (ajusta el formato si tu CSV usa guiones en vez de barras)
                    DateTime fecha = DateTime.ParseExact(dataTxt, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    // Convertimos números (usamos InvariantCulture para que el punto/coma decimal no de error)
                    // Reemplazamos coma por punto por si acaso el CSV viene con formato europeo
                    decimal nivell = decimal.Parse(camps[2].Replace(',', '.'), CultureInfo.InvariantCulture);
                    decimal percent = decimal.Parse(camps[3].Replace(',', '.'), CultureInfo.InvariantCulture);
                    decimal volum = decimal.Parse(camps[4].Replace(',', '.'), CultureInfo.InvariantCulture);

                    // --- PASO 3: NORMALIZACIÓN (Tabla Mesures) ---
                    // Comprobamos si ya existe esta medida para no duplicar (Normalización)
                    bool duplicat = db.Mesures.Any(m => m.EstacioId == estacio.Id && m.Data == fecha);

                    if (!duplicat)
                    {
                        db.Mesures.Add(new Mesura
                        {
                            EstacioId = estacio.Id, // Usamos la FK de la tabla normalizada
                            Data = fecha,
                            NivellAbsolut = nivell,
                            Percentatge = percent,
                            Volum = volum
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error en la línia {i+1}: {ex.Message}");
                }
            }

            db.SaveChanges(); // Guardamos todas las medidas de golpe
            Console.WriteLine("✅ Procés finalitzat.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR CRÍTIC: {ex.Message}");
        }
    }
}