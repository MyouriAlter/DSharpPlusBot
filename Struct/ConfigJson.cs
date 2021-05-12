using Newtonsoft.Json;

namespace DSharpPlusBot.Struct
{
    public struct ConfigJson
    {
        [JsonProperty("Token")] public string Token { get; private set; }
        [JsonProperty("Prefix")] public string Prefix { get; private set; }
    }
}