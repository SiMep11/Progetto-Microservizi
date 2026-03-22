using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magazzino.Repository.Models
{
    public class Prodotto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        public string Descrizione { get; set; }

        public decimal Prezzo { get; set; }

        public int QuantitaInMagazzino { get; set; }
    }
}
