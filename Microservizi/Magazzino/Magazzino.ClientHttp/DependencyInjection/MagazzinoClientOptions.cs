namespace Magazzino.ClientHttp.DependencyInjection;

public class MagazzinoClientOptions
{
    // Il nome della sezione che andrai a scrivere nell'appsettings.json di Ordini o Pagamenti
    public const string SectionName = "MagazzinoClient";
    public string BaseAddress { get; set; } = string.Empty;
}