using System.Runtime.Serialization;

namespace FinPriceFeed.Domain.Enum
{
    public enum SubscriptionType
    {
        [EnumMember(Value = "subscribe")]
        Subscribe,

        [EnumMember(Value = "unsubscribe")]
        Unsubscribe
    }
}
