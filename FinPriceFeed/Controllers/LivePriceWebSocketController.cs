using FinPriceFeed.Core.Configuration.Settings;
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

        [HttpGet]
        public IActionResult GetSetting()
        {
            bool showWebSocket = true;
            if (_extProviderSettings.Selected.Equals("tiingo", StringComparison.OrdinalIgnoreCase))
                showWebSocket = false;

            return Ok(new { showWebSocket });
        }

        [HttpGet("connect")]
        public IActionResult Connect()
        {
            var result = Unavailable();
            if (result != null) return result;

            _webSocketService.AddSubscriber(HttpContext.Connection.Id);
            return Ok();
        }

        [HttpGet("disconnect")]
        public IActionResult Disconnect()
        {
            var result = Unavailable();
            if (result != null) return result;

            _webSocketService.RemoveSubscriber(HttpContext.Connection.Id);
            return Ok();
        }

        [HttpGet("subscribe/{symbol}")]
        public async Task<IActionResult> Subscribe(string symbol)
        {
            var result = Unavailable();
            if (result != null) return result;

            _webSocketService.AddSubscriber(HttpContext.Connection.Id);

            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol is required.");

            await _webSocketService.SubscribeToSymbols(HttpContext.Connection.Id, symbol);
            return Ok();
        }

        [HttpGet("unsubscribe/{symbol}")]
        public async Task<IActionResult> Unsubscribe(string symbol)
        {
            var result = Unavailable();
            if (result != null) return result;

            if (string.IsNullOrEmpty(symbol))
                return BadRequest("Symbol is required.");

            await _webSocketService.UnsubscribeFromSymbols(HttpContext.Connection.Id, symbol);
            return Ok();
        }

        private NotFoundObjectResult Unavailable()
        {
            if (_extProviderSettings.Selected.Equals("tiingo", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound("Tiingo WebSocket not implemented yet. Switch to TwelveData external provider for WebSocket use.");
            }
            else return null;
        }
    }
}
