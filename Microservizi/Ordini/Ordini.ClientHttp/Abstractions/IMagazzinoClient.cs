using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Magazzino.Shared; // Usiamo il DTO ufficiale del Magazzino!

namespace Ordini.ClientHttp.Abstractions
{
    public interface IMagazzinoClient
    {
        // Chiede al magazzino i dettagli di un prodotto
        Task<ProdottoDto?> GetProdottoByIdAsync(int id);
    }
}
