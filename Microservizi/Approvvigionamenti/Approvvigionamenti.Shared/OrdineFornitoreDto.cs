namespace Approvvigionamenti.Shared
{
	public class OrdineFornitoreDto
	{
		public int Id { get; set; }
		public int FornitoreId { get; set; }
		public int ProdottoId { get; set; }
		public int Quantita { get; set; }
		public decimal PrezzoUnitario { get; set; }
		public DateTime DataOrdine { get; set; }
		public DateTime? DataRicezione { get; set; }
		public string Stato { get; set; } = string.Empty;
	}
}