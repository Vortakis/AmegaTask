using FinPriceFeed.Core.Configuration.Settings;
using FinPriceFeed.Core.Configuration.Settings.Section;
using FinPriceFeed.Domain.Enum;
using FinPriceFeed.ExternalProviders.TwelveData.Model;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace FinPriceFeed.ExternalProviders.TwelveData.Service
{
    public class TwelveDataWebSocketService : IExternalProviderWebSocketService
    {
        private readonly IExternalProviderMessageHandler _extProviderMessageHandler;
        private readonly ILogger<TwelveDataWebSocketService> _logger;
        private readonly ApiSection _extProviderEndpoint;

        private ClientWebSocket _webSocket = new();

        public TwelveDataWebSocketService(
            IExternalProviderMessageHandler extProviderMessageHandler,
            ExternalProviderSettings extProviderSettings,
            ILogger<TwelveDataWebSocketService> logger)
        {
            _extProviderMessageHandler = extProviderMessageHandler;
            _extProviderEndpoint = extProviderSettings.Connections["TwelveData"];
            _logger = logger;
        }

        public async Task ConnectToWebSocket()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                return;

            var marketUrl = $"{_extProviderEndpoint.WebSocketUrl}/{TwelveDataEndpoints.WebSocket}?{_extProviderEndpoint.AuthType}={_extProviderEndpoint.ApiKey}";
            var webSocket = new ClientWebSocket();
            try
            {
                await webSocket.ConnectAsync(new Uri(marketUrl), CancellationToken.None);
                if (webSocket.State == WebSocketState.Open)
                {
                    _webSocket = webSocket;
                    _logger.LogInformation($"Successfully connected to WebSocket.");
                }
                else
                {
                    _logger.LogError($"WebSocket connection failed. State: {webSocket.State}");
                }

                _ = ListenForMessages(webSocket);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error connecting to WebSocket: {ex.Message}");
            }
        }

        public async Task SubscribeToLivePrices(string symbolString)
        {
            await ConnectToWebSocket();

            TDRequest wsRequest = new TDRequest
            {
                Action = SubscriptionType.Subscribe.ToString().ToLower(),
                Parameters = new TDSymbol { Symbols = symbolString }
            };

            await RequestToWebSocketAsync(_webSocket, wsRequest);
        }

        public async Task UnsubscribeFromLivePrices(string symbolString)
        {
            await ConnectToWebSocket();

            TDRequest wsRequest = new TDRequest
            {
                Action = SubscriptionType.Unsubscribe.ToString().ToLower(),
                Parameters = new TDSymbol { Symbols = symbolString }
            };

            await RequestToWebSocketAsync(_webSocket, wsRequest);
        }

        private async Task RequestToWebSocketAsync(ClientWebSocket ws, TDRequest wsRequest)
        {
            try
            {
                var message = JsonSerializer.Serialize(wsRequest);
                var bytes = Encoding.UTF8.GetBytes(message);
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error connecting to WebSocket: {ex.Message}");
            }
        }

        private async Task ListenForMessages(ClientWebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await _extProviderMessageHandler.ProcessMessage(message);
                }
            }
        }
    }
}
