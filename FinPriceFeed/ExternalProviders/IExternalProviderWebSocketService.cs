namespace FinPriceFeed.ExternalProviders
{
    public interface IExternalProviderWebSocketService
    {
        Task SubscribeToLivePrices(string symbolString);

        Task UnsubscribeFromLivePrices(string symbol);
    }
}
