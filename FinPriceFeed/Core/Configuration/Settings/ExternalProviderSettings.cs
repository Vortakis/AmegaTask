using FinPriceFeed.Configuration.Section;

namespace FinPriceFeed.Configuration
{
    public class ExternalProviderSettings
    {
        public string Selected { get; set; } = string.Empty;

        public Dictionary<string, ApiSection> Connections { get; set; } = new Dictionary<string, ApiSection>();
    }
}
