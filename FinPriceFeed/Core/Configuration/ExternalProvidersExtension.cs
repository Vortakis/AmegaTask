using FinPriceFeed.Core.Configuration.Settings;
using FinPriceFeed.Core.Configuration.Settings.Section;
using FinPriceFeed.Core.Exceptions;
using FinPriceFeed.ExternalClients;
using FinPriceFeed.ExternalProviders;
using Refit;

namespace FinPriceFeed.Core.Configuration
{
    public static class ExternalProvidersExtension
    {
        public static IServiceCollection AddExternalProviders(this IServiceCollection services, RootSettings settings)
        {
            string selectedExt = settings.ExternalProviderSettings.Selected;
            ApiSection extProviderEndpoint = settings.ExternalProviderSettings.Connections
                .TryGetValue(selectedExt, out var client) ? client : settings.ExternalProviderSettings.Connections.FirstOrDefault().Value;

            if (extProviderEndpoint == null)
                throw new ConfigurationException(settings.ExternalProviderSettings.GetType().Name, "Missing Endpoint settings.");

            services.AddRefitClient<IExternalProviderClient>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(extProviderEndpoint.ApiUrl);
                })
                .AddHttpMessageHandler(() => new CustomMessageHandler(extProviderEndpoint.AuthType, extProviderEndpoint.ApiKey));

            if (selectedExt.Equals("tiingo", StringComparison.OrdinalIgnoreCase))
                services.AddSingleton<IExternalProviderService, TiingoApiService>();
            else if (selectedExt.Equals("twelvedata", StringComparison.OrdinalIgnoreCase))
                services.AddSingleton<IExternalProviderService, TwelveDataApiService>();
            else
                throw new ConfigurationException(settings.ExternalProviderSettings.GetType().Name, $"Selected Provider is Invalid: {selectedExt}");

            return services;
        }
    }
}
