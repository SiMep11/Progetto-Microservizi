using Magazzino.Business;
using Magazzino.Business.Abstraction;
using Magazzino.Shared;
using Magazzino.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace Magazzino.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    // Costruttore primario per essere perfettamente allineati allo stile di C# 12
    public class ProdottiController(IProdottiManager prodottiManager) : ControllerBase
    {
        [HttpGet(Name = "GetAllProdotti")]
        public async Task<IActionResult> GetProdotti(CancellationToken cancellationToken)
        {
            var prodotti = await prodottiManager.GetAllAsync(cancellationToken);
            return Ok(prodotti);
        }

        [HttpPost(Name = "CreaProdotto")]
        public async Task<IActionResult> AggiungiProdotto([FromBody] ProdottoDto prodotto, CancellationToken cancellationToken)
        {
            var nuovo = await prodottiManager.CreaProdottoAsync(prodotto, cancellationToken);
            return Ok(nuovo);
        }
      
        [HttpGet(Name = "GetProdottoById")]
        public async Task<IActionResult> GetProdottoById([FromQuery] int id, CancellationToken cancellationToken)
        {
            var prodotto = await prodottiManager.GetByIdAsync(id, cancellationToken);

            if (prodotto == null)
            {
                return NotFound();
            }

            return Ok(prodotto);
        }
    }
}