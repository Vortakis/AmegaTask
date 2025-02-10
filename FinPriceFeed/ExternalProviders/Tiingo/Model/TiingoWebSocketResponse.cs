using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.Tiingo.Model
{
    public class TiingoWebSocketResponse
    {
        [JsonPropertyName("messageType")]
        public char MessageType { get; set; }

        [JsonPropertyName("data")]
        public TiingoData Data { get; set; } = new TiingoData();


        [JsonPropertyName("response")]
        public TiingoResponseData ResponseData { get; set; } = new TiingoResponseData();
    }

    public class TiingoData
    {
        [JsonPropertyName("subscriptionId")]
        public int SubscriptionId { get; set; }
    }

    public class TiingoResponseData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
