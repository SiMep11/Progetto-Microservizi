using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordini.Business.Abstractions;
using Ordini.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordini.WebApi.Controllers 
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OrdiniController(IOrdiniService ordiniService, ILogger<OrdiniController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetAllOrdini")]
        public async Task<ActionResult<IEnumerable<OrdineDto>>> GetAll()
        {
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
        public async Task<ActionResult<OrdineDto>> Create([FromBody] OrdineInsertDto nuovoOrdine)
        {
            logger.LogInformation("Richiesta creazione nuovo ordine ricevuta.");
            try
            {
                var ordineCreato = await ordiniService.CreaOrdineAsync(nuovoOrdine);
                return CreatedAtAction(nameof(GetById), new { id = ordineCreato.Id }, ordineCreato);
            }
            catch (ArgumentException ex)
            {
                // Prodotto inesistente, ordine vuoto, quantità non valida: sono errori del chiamante, non del server.
                logger.LogWarning(ex, "Richiesta di creazione ordine non valida");
                return BadRequest(ex.Message);
            }
        }
    }
}