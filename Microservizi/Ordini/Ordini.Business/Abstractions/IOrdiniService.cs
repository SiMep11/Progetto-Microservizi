using Ordini.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordini.Business.Abstractions
{
    public interface IOrdiniService
    {
        Task<IEnumerable<OrdineDto>> GetAllOrdiniAsync();
        Task<OrdineDto?> GetOrdineByIdAsync(int id);
        Task<OrdineDto> CreaOrdineAsync(OrdineDto nuovoOrdineDto);
        Task<bool> ConfermaOrdineSePagatoAsync(int ordineId, decimal importoPagato, CancellationToken cancellationToken = default);
    }
}
