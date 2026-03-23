using Magazzino.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magazzino.Repository.Abstraction
{
    public interface IMagazzinoRepository
    {
        Task<IEnumerable<Prodotto>> GetAllProdottiAsync();
        Task<Prodotto?> GetProdottoByIdAsync(int id);
        Task<Prodotto> CreaProdottoAsync(Prodotto prodotto);
    }
}
