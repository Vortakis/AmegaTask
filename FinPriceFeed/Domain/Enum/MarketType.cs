using System.Runtime.Serialization;

namespace FinPriceFeed.Domain.Enum
{
    public enum MarketType
    {
        [EnumMember(Value = "Undefined")]
        Undefined = 1,

        [EnumMember(Value = "Forex")]
        Forex = 2,

        [EnumMember(Value = "Crypto")]
        Crypto = 3,

        [EnumMember(Value = "Stock")]
        Stock = 4
    }
}
