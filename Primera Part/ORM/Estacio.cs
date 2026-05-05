using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniProjecte.Models
{
    public class Estacio 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nom { get; set; } = "";
        public string Municipi { get; set; } = "";
        public decimal CapacitatMaxima { get; set; }    
        public List<Mesura> Mesures { get; set; } = new();
    }
}