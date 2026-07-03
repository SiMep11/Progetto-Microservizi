using Microsoft.AspNetCore.Mvc;
using Ordini.Business.Abstractions;
using Ordini.Shared;

namespace Ordini.WebApi.ControllerS
{
    [ApiController]
    [Route("api/[controller]")] // La rotta diventerà in automatico /api/ordini
    public class OrdiniController : ControllerBase
    {
        private readonly IOrdiniService _ordiniService;

        // Chiediamo a .NET di "iniettare" il nostro servizio
        public OrdiniController(IOrdiniService ordiniService)
        {
            _ordiniService = ordiniService;
        }

        // GET: api/ordini
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdineDto>>> GetAll()
        {
            var ordini = await _ordiniService.GetAllOrdiniAsync();
            return Ok(ordini); // Restituisce HTTP 200 OK con la lista
        }

        // GET: api/ordini/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdineDto>> GetById(int id)
        {
            var ordine = await _ordiniService.GetOrdineByIdAsync(id);

            if (ordine == null)
            {
                return NotFound(); // Restituisce HTTP 404 se non esiste
            }

            return Ok(ordine);
        }

        // POST: api/ordini
        [HttpPost]
        public async Task<ActionResult<OrdineDto>> Create([FromBody] OrdineDto nuovoOrdine)
        {
            var ordineCreato = await _ordiniService.CreaOrdineAsync(nuovoOrdine);

            // Restituisce HTTP 201 Created e dice dove trovare il nuovo ordine
            return CreatedAtAction(nameof(GetById), new { id = ordineCreato.Id }, ordineCreato);
        }
    }
}
