using FinPriceFeed.Domain.Enum;
using FinPriceFeed.Service;
using Microsoft.AspNetCore.Mvc;

namespace FinPriceFeed.Controllers
{
    [ApiController]
    [Route("api/fin-instruments/{ticker}/price")]
    public class FinInstrumentPriceController : ControllerBase
    {
        private readonly IFinInstrumentService _finInstrumentService;

        public FinInstrumentPriceController(IFinInstrumentService finInstrumentService)
        {
            _finInstrumentService = finInstrumentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentPrice(string ticker, [FromQuery] MarketType marketType = MarketType.Undefined)
        {
            var finInstrumnetPrices = await _finInstrumentService.GetCurrentFinInstrumentPriceAsync(ticker, marketType);
            
            return Ok(finInstrumnetPrices);
        }
    }
}
