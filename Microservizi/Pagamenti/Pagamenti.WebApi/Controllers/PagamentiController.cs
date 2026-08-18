using Microsoft.AspNetCore.Mvc;
using Pagamenti.Business.Abstraction;
using Pagamenti.Shared;

namespace Pagamenti.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PagamentiController(IBusiness business, ILogger<PagamentiController> logger) : ControllerBase
    {
        [HttpPost(Name = "CreateVersamento")]
        public async Task<ActionResult> CreateVersamento(VersamentoInsertDto versamentoInsertDto)
        {
            try
            {
                await business.CreateVersamentoAsync(versamentoInsertDto);
            }
            catch (ArgumentException ex)
            {
                // Versamento non valido (ordine inesistente, importo errato...): è un errore del chiamante, non del server.=
                logger.LogWarning(ex, "Richiesta di versamento non valida");
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
