using Magazzino.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magazzino.Business.Abstraction
{
    public interface IProdottiManager
    {
        Task<IEnumerable<ProdottoDto>> GetAllAsync();
        Task<ProdottoDto?> GetByIdAsync(int id);
        Task<ProdottoDto> CreaProdottoAsync(ProdottoDto prodottoDto);
    }
}
