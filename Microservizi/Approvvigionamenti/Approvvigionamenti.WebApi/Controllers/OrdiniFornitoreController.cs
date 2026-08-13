using Approvvigionamenti.Business.Abstraction;
using Approvvigionamenti.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Approvvigionamenti.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OrdiniFornitoreController(IApprovvigionamentiManager manager, ILogger<OrdiniFornitoreController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetAllOrdiniFornitore")]
        public async Task<ActionResult<IEnumerable<OrdineFornitoreDto>>> GetAll(CancellationToken cancellationToken)
        {
            var ordini = await manager.GetAllOrdiniFornitoreAsync(cancellationToken);
            return Ok(ordini);
        }

        [HttpGet("{id}", Name = "GetOrdineFornitoreById")]
        public async Task<ActionResult<OrdineFornitoreDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var ordine = await manager.GetOrdineFornitoreByIdAsync(id, cancellationToken);

            if (ordine == null)
            {
                logger.LogWarning("Ordine fornitore con ID {id} non trovato.", id);
                return NotFound();
            }

            return Ok(ordine);
        }

        /// <summary>
        /// Ordine di acquisto creato a mano. Il fornitore e il prezzo li ricava il Business dalla configurazione del prodotto, non li decide il chiamante.
        /// </summary>
        [HttpPost(Name = "CreateOrdineFornitore")]
        public async Task<ActionResult<OrdineFornitoreDto>> Create([FromQuery] int prodottoId, [FromQuery] int quantita, CancellationToken cancellationToken)
        {
            try
            {
                var creato = await manager.CreaOrdineFornitoreAsync(prodottoId, quantita, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = creato.Id }, creato);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Richiesta di ordine al fornitore non valida");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// La merce è arrivata: chiude l'ordine e fa ricaricare la giacenza al magazzino.
        /// </summary>
        [HttpPost("{id}", Name = "ConfermaRicezione")]
        public async Task<ActionResult> ConfermaRicezione(int id, CancellationToken cancellationToken)
        {
            var esito = await manager.ConfermaRicezioneAsync(id, cancellationToken);

            if (!esito)
            {
                logger.LogWarning("Ordine fornitore con ID {id} non trovato.", id);
                return NotFound();
            }

            return Ok();
        }
    }
}