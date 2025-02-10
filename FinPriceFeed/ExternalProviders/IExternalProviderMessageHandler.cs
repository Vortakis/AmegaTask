using FinPriceFeed.Domain.Enum;

namespace FinPriceFeed.ExternalProviders
{
    public interface IExternalProviderMessageHandler
    {
        event Func<SubscriptionType, string[], Task> OnSubscriptionUpdate;
        event Func<string, decimal, Task> OnPriceUpdate;

        Task ProcessMessage(string message);



    }
}
