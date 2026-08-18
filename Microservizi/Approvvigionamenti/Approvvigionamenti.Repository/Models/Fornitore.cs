namespace Approvvigionamenti.Repository.Models
{
    public class Fornitore
    {
        public int Id { get; set; }
        public string RagioneSociale { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Uso ICollection per Entity Framework
        public ICollection<ProdottoFornito> ProdottiForniti { get; set; } = new List<ProdottoFornito>();
    }
}