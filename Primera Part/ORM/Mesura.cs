using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniProjecte.Models
{
    public class Mesura
    {
        [Key]
        public int Id { get; set; }
        public int EstacioId { get; set; }
        [ForeignKey("EstacioId")]
        public Estacio? Estacio { get; set; }
        public DateTime Data { get; set; }
        public decimal NivellAbsolut { get; set; }
        public decimal Percentatge { get; set; }
        public decimal Volum { get; set; }
    }
}