using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.TwelveData.Model
{
    public class TDRequest
    {

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("params")]
        public TDSymbol Parameters { get; set; } = new ();
    }
}

