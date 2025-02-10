using FinPriceFeed.Configuration;
using FinPriceFeed.Configuration.Section;
using FinPriceFeed.Domain.Enum;
using FinPriceFeed.Domain.Model;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using FinPriceFeed.ExternalProviders.Tiingo.Model;
using FinPriceFeed.ExternalProviders.TwelveData.Model;
using Microsoft.Extensions.Logging;

namespace FinPriceFeed.ExternalProviders.TwelveData.Service
{
    public class TwelveDataWebSocketService : IExternalProviderWebSocketService
    {
        public event Func<SubscriptionType, string[], Task> OnSubscriptionUpdate = async (_, _) => { };
        public event Func<string, decimal, Task> OnPriceUpdate = async (_, _) => { };

        private readonly ILogger<TwelveDataWebSocketService> _logger;
        private readonly ApiSection _extProviderEndpoint;

        private ClientWebSocket _webSocket = new();


        public TwelveDataWebSocketService(
            ExternalProviderSettings extProviderSettings,
            ILogger<TwelveDataWebSocketService> logger)
        {
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

                    if (message.Contains("subscribe-status"))
                    {
                        await HandleSubscription(message);
                        continue;
                    }
                    else if (message.Contains("price"))
                    {
                        await HandlePriceUpdate(message);
                        continue;
                    }
                }
            }
        }

        private async Task HandleSubscription(string message)
        {
            _logger.LogInformation($"Successfully subscribed to ''.");

            var response = Deserialise<TDSubscriptionResponse>(message);
            if (response == null)
            {
                _logger.LogError($"Deserialising Subscribe/Unsubscribe response failed. Message: {message}");
                return;
            }
            var subscriptionType = response.Event.Contains("unsubscribe", StringComparison.OrdinalIgnoreCase) ?
                                    SubscriptionType.Unsubscribe : SubscriptionType.Subscribe;

            if (response.Fails != null && response.Fails.Length > 0)
            {
                var failedSymbols = response.Fails.Select(s => s.Symbol).ToArray();

                await OnSubscriptionUpdate.Invoke(subscriptionType, failedSymbols);
            }
        }

        private async Task HandlePriceUpdate(string message)
        {
            var response = Deserialise<TDPriceResponse>(message);
            if (response == null)
                return;

            await OnPriceUpdate.Invoke(response.Symbol, response.Price);
        }

        private T? Deserialise<T>(string message)
        {
            T? response;
            try
            {
                response = JsonSerializer.Deserialize<T>(message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Parsing External Message threw exception: {ex.Message} ");
                return default;
            }
        }
    }
}
