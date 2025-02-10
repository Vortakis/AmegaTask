using FinPriceFeed.Service;
using Microsoft.AspNetCore.Mvc;

namespace FinPriceFeed.Controllers
{
    [ApiController]
    [Route("api/fin-instruments")]
    public class FinInstrumentController : ControllerBase
    {
        private readonly IFinInstrumentService _finInstrumentService;

        public FinInstrumentController(IFinInstrumentService finInstrumentService)
        {
            _finInstrumentService = finInstrumentService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableFinInstruments([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var finInstrumnets = await _finInstrumentService.GetAvailableFinInstrumentsAsync(page, pageSize);

            return Ok(finInstrumnets);
        }
    }
}
