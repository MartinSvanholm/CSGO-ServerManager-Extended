using Newtonsoft.Json;

namespace CSGOServerInterface.Server.DTO;

public class Teamspeak3Settings
{
    [JsonProperty("slots")]
    public int Slots { get; set; }

    [JsonProperty("ts_admin_token")]
    public string TsAdminToken { get; set; }

    [JsonProperty("ts_server_id")]
    public string TsServerId { get; set; }
}