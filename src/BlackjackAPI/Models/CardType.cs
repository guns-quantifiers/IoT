using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlackjackAPI.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CardType
    {
        None = 0,
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
}
