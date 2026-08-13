namespace Approvvigionamenti.Shared
{
    public class FornitoreDto
    {
        public int Id { get; set; }
        public string RagioneSociale { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<ProdottoFornitoDto> ProdottiForniti { get; set; } = new List<ProdottoFornitoDto>();
    }
}