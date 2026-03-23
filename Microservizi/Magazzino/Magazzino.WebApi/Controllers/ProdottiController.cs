using Magazzino.Business;
using Magazzino.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace Magazzino.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdottiController : ControllerBase
    {
        private readonly ProdottiManager _manager;

        public ProdottiController(ProdottiManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProdotti()
        {
            var prodotti = await _manager.RecuperaTuttiAsync();
            return Ok(prodotti);
        }

        [HttpPost]
        public async Task<IActionResult> AggiungiProdotto([FromBody] Prodotto prodotto)
        {
            var nuovo = await _manager.CreaProdottoAsync(prodotto);
            return Ok(nuovo);
        }
    }
}
