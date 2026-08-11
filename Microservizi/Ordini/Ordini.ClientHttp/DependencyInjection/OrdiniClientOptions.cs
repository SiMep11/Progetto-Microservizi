namespace Ordini.ClientHttp.DependencyInjection
{
    public class OrdiniClientOptions
    {
        // Il nome della sezione che andrai a scrivere nell'appsettings.json di chi usa il client
        public const string SectionName = "OrdiniClient";
        public string BaseAddress { get; set; } = string.Empty;
    }
}
