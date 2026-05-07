using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
public class LiniaCsvAPI
{
    [Name("Dia")] public string Dia { get; set; }
    [Name("Estació")] public string Estacio { get; set; }
    [Name("Nivell absolut (msnm)")] public string Nivell { get; set; }
    [Name("Percentatge volum embassat (%)")] public string Percentatge { get; set; }
    [Name("Volum embassat (hm3)")] public string Volum { get; set; }
}
public class EstacioAPI
{
    [Key]
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public string Municipi { get; set; } = "";
    public List<MesuraAPI> Mesures { get; set; } = new();
}
public class MesuraAPI
{
    [Key]
    public int Id { get; set; }    
    public int EstacioId { get; set; }
    public EstacioAPI? Estacio { get; set; }
    public DateTime Data { get; set; }
    public decimal NivellAbsolut { get; set; }
    public decimal Percentatge { get; set; }
    public decimal Volum { get; set; }
}