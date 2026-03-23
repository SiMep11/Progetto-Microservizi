namespace Magazzino.Shared
{
    public class ProdottoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;

        // Questo è il prezzo ufficiale che l'OrdiniService dovrà leggere!
        public decimal Prezzo { get; set; }

        public int QuantitaDisponibile { get; set; }
    }
}
