using Magazzino.Business;
using Magazzino.Business.Abstraction;
using Magazzino.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace Magazzino.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdottiController : ControllerBase
    {
        private readonly IProdottiManager _prodottiManager;

        public ProdottiController(IProdottiManager prodottiManager)
        {
            _prodottiManager = prodottiManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProdotti()
        {
            var prodotti = await _prodottiManager.GetAllAsync();
            return Ok(prodotti);
        }

        [HttpPost]
        public async Task<IActionResult> AggiungiProdotto([FromBody] Magazzino.Shared.ProdottoDto prodotto) // <-- Usa il DTO!
        {
            var nuovo = await _prodottiManager.CreaProdottoAsync(prodotto);
            return Ok(nuovo);
        }
    }
}
