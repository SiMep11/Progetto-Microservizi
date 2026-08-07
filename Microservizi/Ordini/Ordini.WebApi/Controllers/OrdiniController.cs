using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordini.Business.Abstractions;
using Ordini.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordini.WebApi.Controllers // Corretto togliendo la 'S' maiuscola finale se c'era
{
    [ApiController]
    // Stile Prof: Rotta basata sul nome del controller e del metodo (es. /Ordini/GetAll)
    [Route("[controller]/[action]")]
    public class OrdiniController(IOrdiniService ordiniService, ILogger<OrdiniController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetAllOrdini")]
        public async Task<ActionResult<IEnumerable<OrdineDto>>> GetAll()
        {
            // Possiamo usare il logger che abbiamo iniettato!
            logger.LogInformation("Richiesta lista di tutti gli ordini ricevuta.");

            var ordini = await ordiniService.GetAllOrdiniAsync();
            return Ok(ordini);
        }

        [HttpGet("{id}", Name = "GetOrdineById")]
        public async Task<ActionResult<OrdineDto>> GetById(int id)
        {
            var ordine = await ordiniService.GetOrdineByIdAsync(id);

            if (ordine == null)
            {
                logger.LogWarning($"Ordine con ID {id} non trovato.");
                return NotFound();
            }

            return Ok(ordine);
        }

        [HttpPost(Name = "CreateOrdine")]
        public async Task<ActionResult<OrdineDto>> Create([FromBody] OrdineDto nuovoOrdine)
        {
            logger.LogInformation("Richiesta creazione nuovo ordine ricevuta.");
            var ordineCreato = await ordiniService.CreaOrdineAsync(nuovoOrdine);

            return CreatedAtAction(nameof(GetById), new { id = ordineCreato.Id }, ordineCreato);
        }
    }
}