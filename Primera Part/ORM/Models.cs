using CsvHelper.Configuration.Attributes;

public class LiniaCsv
{
    [Name("Dia")] 
    public string Dia { get; set; } = "";

    [Name("Estació")] 
    public string Estacio { get; set; } = "";

    [Name("Nivell absolut (msnm)")] 
    public string Nivell { get; set; } = "";

    [Name("Percentatge volum embassat (%)")] 
    public string Percentatge { get; set; } = "";

    [Name("Volum embassat (hm3)")] 
    public string Volum { get; set; } = "";
}