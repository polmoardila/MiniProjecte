using EmbassamentDB.Models;

namespace Web.Models;

public class EstacioDetall
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public string Municipi { get; set; } = "";
    public Mesura? UltimaMesura { get; set; }
    public List<Mesura> Historic { get; set; } = new();
}