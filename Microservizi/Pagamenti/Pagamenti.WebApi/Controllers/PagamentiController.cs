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
            await business.CreateVersamentoAsync(versamentoInsertDto);

            return Ok();
        }
    }
}
