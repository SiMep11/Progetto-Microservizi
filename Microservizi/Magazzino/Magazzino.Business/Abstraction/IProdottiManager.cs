using Magazzino.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magazzino.Business.Abstraction
{
    public interface IProdottiManager
    {
        Task<IEnumerable<ProdottoDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProdottoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProdottoDto> CreaProdottoAsync(ProdottoDto prodottoDto, CancellationToken cancellationToken = default);
        Task ScaricaGiacenzaAsync(int id, int quantita, CancellationToken cancellationToken = default);
    }
}
