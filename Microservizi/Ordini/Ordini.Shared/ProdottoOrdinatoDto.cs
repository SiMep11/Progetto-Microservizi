using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Ordini.Shared
{
    public class ProdottoOrdinatoDto
    {
        // Riferimento al Magazzino (NON è una vera chiave esterna nel DB degli Ordini)
        public int ProdottoId { get; set; }

        public int Quantita { get; set; }

        // "fotografia" del prezzo al momento dell'acquisto
        public decimal PrezzoUnitario { get; set; }
    }
}
