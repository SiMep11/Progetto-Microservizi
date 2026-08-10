using System;
using System.Collections.Generic;
using System.Text;

namespace Pagamenti.Shared
{
    public class VersamentoInsertDto
    {
        public DateTime DataVersamento { get; set; }
        public DateTime DataContabile { get; set; }
        public decimal Importo { get; set; }
        public string? Informazioni { get; set; }
        public int ordineId { get; set; }
    }
}