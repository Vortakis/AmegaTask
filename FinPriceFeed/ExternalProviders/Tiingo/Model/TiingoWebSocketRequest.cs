using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.Tiingo.Model
{
    public class TiingoWebSocketRequest
    {
        [JsonPropertyName("eventName")]
        public string EventName { get; set; }

        [JsonPropertyName("authorization")]
        public string Authorization { get; set; }

        [JsonPropertyName("eventData")]
        public TiingoEventData EventData { get; set; } = new TiingoEventData();
    }

    public class TiingoEventData
    {
        [JsonPropertyName("thresholdLevel")]
        public int ThresholdLevel { get; set; } = 6;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tickers")]
        public string[] Tickers { get; set; } = ["*"];
    }

}
