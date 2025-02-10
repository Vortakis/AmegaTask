using FinPriceFeed.Domain.Model;
using Refit;

namespace FinPriceFeed.ExternalProviders
{
    public interface IExternalProviderClient
    {
        [Get("/{path}")]
        Task<T> GetFinInstrumentsAsync<T>([AliasAs("path")] string path);

        [Get("/{path}")]
        Task<T> GetCurrentPriceAsync<T>([AliasAs("path")] string path, [Query] IDictionary<string,string> queryParams);
    }
}
