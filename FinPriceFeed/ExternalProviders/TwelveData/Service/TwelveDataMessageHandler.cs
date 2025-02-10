using FinPriceFeed.Domain.Enum;
using FinPriceFeed.ExternalProviders.TwelveData.Model;
using System.Text.Json;

namespace FinPriceFeed.ExternalProviders.TwelveData.Service
{
    public class TwelveDataMessageHandler : IExternalProviderMessageHandler
    {
        public event Func<SubscriptionType, string[], Task> OnSubscriptionUpdate = async (_, _) => { };
        public event Func<string, decimal, Task> OnPriceUpdate = async (_, _) => { };
        private readonly ILogger<TwelveDataMessageHandler> _logger;

        public TwelveDataMessageHandler(ILogger<TwelveDataMessageHandler> logger)
        {
            _logger = logger;
        }

        public async Task ProcessMessage(string message)
        {
            if (message.Contains("subscribe-status"))
            {
                await HandleSubscription(message);
            }
            else if (message.Contains("price"))
            {
                await HandlePriceUpdate(message);
            }
        }

        private async Task HandleSubscription(string message)
        {
            _logger.LogInformation($"Successfully subscribed.");

            var response = Deserialize<TDSubscriptionResponse>(message);
            if (response == null)
            {
                _logger.LogError($"Deserializing Subscribe/Unsubscribe response failed. Message: {message}");
                return;
            }

            var subscriptionType = response.Event.Contains("unsubscribe", StringComparison.OrdinalIgnoreCase)
                ? SubscriptionType.Unsubscribe
                : SubscriptionType.Subscribe;

            if (response.Fails?.Length > 0)
            {
                var failedSymbols = response.Fails.Select(s => s.Symbol).ToArray();
                await OnSubscriptionUpdate.Invoke(subscriptionType, failedSymbols);
            }
        }

        private async Task HandlePriceUpdate(string message)
        {
            var response = Deserialize<TDPriceResponse>(message);
            if (response == null)
                return;

            await OnPriceUpdate.Invoke(response.Symbol, response.Price);
        }

        private T? Deserialize<T>(string message)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Parsing External Message threw exception: {ex.Message}");
                return default;
            }
        }
    }
}
