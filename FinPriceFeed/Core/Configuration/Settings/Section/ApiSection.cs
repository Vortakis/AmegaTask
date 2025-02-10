namespace FinPriceFeed.Core.Configuration.Settings.Section
{
    public class ApiSection
    {
        public string ApiUrl { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public string WebSocketUrl { get; set; } = string.Empty;

        public string AuthType { get; set; } = string.Empty;
    }
}
