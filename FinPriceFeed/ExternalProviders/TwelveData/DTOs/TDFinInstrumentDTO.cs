using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.TwelveData.DTOs
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "marketType")]
    [JsonDerivedType(typeof(TDForexFI), typeDiscriminator: "forex")]
    [JsonDerivedType(typeof(TDCryptoFI), typeDiscriminator: "crypto")]
    [JsonDerivedType(typeof(TDStockFI), typeDiscriminator: "stock")]
    public class TDFinInstrumentDTO
    {
        [JsonPropertyName("data")]
        public List<TDData> Data { get; set; } = new List<TDData>();

    }

    public class TDData
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("currency_base")]
        public string CurrencyBase { get; set; }

        [JsonPropertyName("currency_quote")]
        public string CurrencyQuote { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }
    }

    public class TDForexFI : TDFinInstrumentDTO;
    public class TDStockFI : TDFinInstrumentDTO;
    public class TDCryptoFI : TDFinInstrumentDTO;

}
