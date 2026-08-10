using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Repository.Model
{
    public class Versamento
    {
        public int Id { get; set; }
        public DateTime DataVersamento { get; set; }
        public DateTime DataContabile { get; set; }
        public decimal Importo { get; set; }
        public string? Informazioni { get; set; }
        public int ordineId { get; set; }
    }
}
