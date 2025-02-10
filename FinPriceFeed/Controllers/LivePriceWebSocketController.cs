using FinPriceFeed.Configuration;
using FinPriceFeed.ExternalProviders;
using FinPriceFeed.ExternalProviders.Tiingo;
using FinPriceFeed.Service;
using Microsoft.AspNetCore.Mvc;

namespace FinPriceFeed.Controllers
{
    [Route("api/live-prices")]
    [ApiController]
    public class LivePriceWebSocketController : ControllerBase
    {
        private readonly ILivePriceWebSocketService _webSocketService;
        private readonly ExternalProviderSettings _extProviderSettings;
        public LivePriceWebSocketController(
            ILivePriceWebSocketService webSocketService, 
            ExternalProviderSettings extProviderSettings)
        {
            _webSocketService = webSocketService;
            _extProviderSettings = extProviderSettings;
        }

        [HttpGet("connect")]
        public IActionResult Connect()
        {
            if (_extProviderSettings.Selected.Equals("tiingo", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound("Tiingo WebSocket is not implemented yet!");
            }

            _webSocketService.AddSubscriber(HttpContext.Connection.Id);
            return Ok();
        }

        [HttpGet("disconnect")]
        public IActionResult Disconnect()
        {
            if (_extProviderSettings.Selected.Equals("tiingo", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound("Tiingo WebSocket is not implemented yet!");
            }

            _webSocketService.RemoveSubscriber(HttpContext.Connection.Id);
            return Ok();
        }

        [HttpGet("subscribe/{symbol}")]
        public async Task<IActionResult> Subscribe(string symbol)
        {
            if (_extProviderSettings.Selected.Equals("tiingo", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound("Tiingo WebSocket is not implemented yet!");
            }

            _webSocketService.AddSubscriber(HttpContext.Connection.Id);

            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol is required.");

            await _webSocketService.SubscribeToSymbols(HttpContext.Connection.Id, symbol);
            return Ok();
        }

        [HttpGet("unsubscribe/{symbol}")]
        public async Task<IActionResult> Unsubscribe(string symbol)
        {
            if (_extProviderSettings.Selected.Equals("tiingo", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound("Tiingo WebSocket is not implemented yet!");
            }

            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol is required.");

            await _webSocketService.UnsubscribeFromSymbols(HttpContext.Connection.Id, symbol);
            return Ok();
        }
    }
}
