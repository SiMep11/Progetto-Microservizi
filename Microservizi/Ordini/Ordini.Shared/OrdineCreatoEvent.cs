using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Shared
{
    public class OrdineCreatoEvent
    {
        public int OrdineId { get; set; }
        // Passiamo i prodotti venduti così il magazzino saprà cosa scalare!
        public List<ProdottoOrdinatoDto> Prodotti { get; set; } = new List<ProdottoOrdinatoDto>();
    }
}
