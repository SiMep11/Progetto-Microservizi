namespace Ordini.Shared
{
    /// <summary>
    /// DTO usato SOLO in ingresso per creare un ordine. Id, DataOrdine, Stato e Totale non compaiono perché li determina il server.
    /// </summary>
    public class OrdineInsertDto
    {
        public string Cliente { get; set; } = string.Empty;

        public List<ProdottoOrdinatoInsertDto> Prodotti { get; set; } = new List<ProdottoOrdinatoInsertDto>();
    }
}