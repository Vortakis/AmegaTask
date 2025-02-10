using FinPriceFeed.Core.Configuration.Settings.Section;

namespace FinPriceFeed.Core.Configuration.Settings
{
    public class ExternalProviderSettings
    {
        public string Selected { get; set; } = string.Empty;

        public Dictionary<string, ApiSection> Connections { get; set; } = new Dictionary<string, ApiSection>();
    }
}
