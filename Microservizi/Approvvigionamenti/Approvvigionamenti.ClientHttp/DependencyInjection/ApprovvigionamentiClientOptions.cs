namespace Approvvigionamenti.ClientHttp.DependencyInjection;

public class ApprovvigionamentiClientOptions
{
    // Il nome della sezione che scrivo nell'appsettings.json di chi usa il client
    public const string SectionName = "ApprovvigionamentiClient";
    public string BaseAddress { get; set; } = string.Empty;
}