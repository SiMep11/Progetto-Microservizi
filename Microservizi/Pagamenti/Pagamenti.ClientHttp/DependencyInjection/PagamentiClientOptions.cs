namespace Pagamenti.ClientHttp.DependencyInjection;

public class PagamentiClientOptions
{
    // Il nome della sezione che andrai a scrivere nell'appsettings.json di chi usa il client
    public const string SectionName = "PagamentiClient";
    public string BaseAddress { get; set; } = string.Empty;
}
