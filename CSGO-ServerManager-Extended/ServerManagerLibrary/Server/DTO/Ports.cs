using Newtonsoft.Json;

namespace CSGOServerInterface.Server.DTO;

public class Ports
{
    [JsonProperty("game")]
    public int Game { get; set; }

    [JsonProperty("gotv")]
    public int? Gotv { get; set; }

    [JsonProperty("gotv_secondary")]
    public int? GotvSecondary { get; set; }
}