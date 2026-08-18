namespace Magazzino.ClientHttp.DependencyInjection;

public class MagazzinoClientOptions
{
    // Il nome della sezione che scrivo nell'appsettings.json di Ordini o Pagamenti
    public const string SectionName = "MagazzinoClient";
    public string BaseAddress { get; set; } = string.Empty;
}