using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Shared.Events
{
    /// <summary>
    /// Pubblicato quando un ordine passa a "Confermato" perché il pagamento è stato
    /// incassato per l'importo corretto. È il segnale con cui il magazzino scarica
    /// definitivamente la giacenza.
    /// </summary>
    public class OrdineConfermatoEvent
    {
        public int OrdineId { get; set; }
        // Passiamo i prodotti venduti così il magazzino saprà cosa scalare!
        public List<ProdottoOrdinatoDto> Prodotti { get; set; } = new List<ProdottoOrdinatoDto>();
    }
}
