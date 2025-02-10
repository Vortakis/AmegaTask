using FinPriceFeed.Domain.Enum;

namespace FinPriceFeed.ExternalProviders
{
    public interface IExternalProviderWebSocketService
    {
        Task SubscribeToLivePrices(string symbolString);

        Task UnsubscribeFromLivePrices(string symbol);

        event Func<string, decimal, Task> OnPriceUpdate;

        event Func<SubscriptionType, string[], Task> OnSubscriptionUpdate;

    }
}
