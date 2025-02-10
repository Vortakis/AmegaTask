using System.Text.Json.Serialization;

namespace FinPriceFeed.Domain.Model
{
    public class FinInstrumentPrice
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        [JsonPropertyName("lastSaleTimestamp")]
        public DateTime? LastSaleTimestamp { get; set; }

        [JsonPropertyName("quoteTimestamp")]
        public DateTime? QuoteTimestamp { get; set; }

        [JsonPropertyName("bidPrice")]
        public decimal? BidPrice { get; set; }

        [JsonPropertyName("askPrice")]
        public decimal? AskPrice { get; set; }

        [JsonPropertyName("midPrice")]
        public decimal? MidPrice { get; set; }

        [JsonPropertyName("tngoLast")]
        public decimal? TngoLast { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }


    }
}
