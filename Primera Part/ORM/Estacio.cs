using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniProjecte.Models
{
    public class Estacio 
    {
        [Key]
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Municipi { get; set; } = "";
        public List<Mesura> Mesures { get; set; } = new();
    }
}