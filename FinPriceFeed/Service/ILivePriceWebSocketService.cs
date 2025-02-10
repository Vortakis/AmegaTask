using FinPriceFeed.Domain.Enum;

namespace FinPriceFeed.Service
{
    public interface ILivePriceWebSocketService
    {
        void AddSubscriber(string connectionId);

        void RemoveSubscriber(string connectionId);

        Task SubscribeToSymbols(string connectionId, string symbolString);

        Task UnsubscribeFromSymbols(string connectionId, string symbolString);

        Task BroadcastPriceUpdate(string symbol, decimal price);

        Task BroadcastSubscriptionFailed(SubscriptionType type, string[] failedSymbols);
    }
}
