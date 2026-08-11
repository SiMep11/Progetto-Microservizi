using Pagamenti.Shared; // Per usare i DTO dei versamenti

namespace Pagamenti.ClientHttp.Abstraction;

public interface IPagamentiClient
{
    /// <summary>
    /// Registra un versamento per un ordine. Il servizio Pagamenti si occuperà poi
    /// di notificare l'incasso su Kafka.
    /// </summary>
    Task CreateVersamentoAsync(VersamentoInsertDto versamento, CancellationToken cancellationToken = default);
}
