using Approvvigionamenti.Business.Abstraction;
using Approvvigionamenti.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Approvvigionamenti.WebApi.Controllers
{
    [ApiController]
    // Stile Prof: rotta basata sul nome del controller e del metodo (es. /Fornitori/GetAll)
    [Route("[controller]/[action]")]
    public class FornitoriController(IApprovvigionamentiManager manager, ILogger<FornitoriController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetAllFornitori")]
        public async Task<ActionResult<IEnumerable<FornitoreDto>>> GetAll(CancellationToken cancellationToken)
        {
            var fornitori = await manager.GetAllFornitoriAsync(cancellationToken);
            return Ok(fornitori);
        }

        [HttpGet("{id}", Name = "GetFornitoreById")]
        public async Task<ActionResult<FornitoreDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var fornitore = await manager.GetFornitoreByIdAsync(id, cancellationToken);

            if (fornitore == null)
            {
                logger.LogWarning("Fornitore con ID {id} non trovato.", id);
                return NotFound();
            }

            return Ok(fornitore);
        }

        [HttpPost(Name = "CreateFornitore")]
        public async Task<ActionResult<FornitoreDto>> Create([FromBody] FornitoreInsertDto fornitore, CancellationToken cancellationToken)
        {
            try
            {
                var creato = await manager.CreaFornitoreAsync(fornitore, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = creato.Id }, creato);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Richiesta di creazione fornitore non valida");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost(Name = "AggiungiProdottoFornito")]
        public async Task<ActionResult<ProdottoFornitoDto>> AggiungiProdottoFornito([FromBody] ProdottoFornitoDto prodottoFornito, CancellationToken cancellationToken)
        {
            try
            {
                var aggiunto = await manager.AggiungiProdottoFornitoAsync(prodottoFornito, cancellationToken);
                return Ok(aggiunto);
            }
            catch (ArgumentException ex)
            {
                // Fornitore inesistente, prodotto non a catalogo, prodotto già assegnato: sono tutti errori del chiamante, non del server.
                logger.LogWarning(ex, "Richiesta di associazione prodotto-fornitore non valida");
                return BadRequest(ex.Message);
            }
        }
    }
}