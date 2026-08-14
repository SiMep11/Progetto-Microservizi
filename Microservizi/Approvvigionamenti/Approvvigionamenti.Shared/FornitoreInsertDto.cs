namespace Approvvigionamenti.Shared
{
    /// <summary>
    /// DTO usato SOLO in ingresso per creare un fornitore. I prodotti forniti non compaiono qui perché si associano dopo, con AggiungiProdottoFornito: ognuno
    /// va validato contro il magazzino e controllato per l'unicità.
    /// </summary>
    public class FornitoreInsertDto
    {
        public string RagioneSociale { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}