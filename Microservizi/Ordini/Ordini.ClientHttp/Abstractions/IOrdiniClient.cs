using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ordini.Shared; // Uso i DTO di Ordini

namespace Ordini.ClientHttp.Abstraction 
{
    public interface IOrdiniClient
    {
        // Ottiene tutti gli ordini
        Task<IEnumerable<OrdineDto>> GetAllOrdiniAsync(CancellationToken cancellationToken = default);

        // Ottiene un ordine specifico tramite ID
        Task<OrdineDto?> GetOrdineByIdAsync(int id, CancellationToken cancellationToken = default);

        // Invia un nuovo ordine da creare
        Task<OrdineDto> CreaOrdineAsync(OrdineInsertDto ordine, CancellationToken cancellationToken = default);
    }
}
