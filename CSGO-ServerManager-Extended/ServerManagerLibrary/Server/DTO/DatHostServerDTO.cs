using Newtonsoft.Json;

namespace CSGOServerInterface.Server.DTO;

public class DatHostServerDTO
{
    [JsonProperty("booting")]
    public bool Booting { get; set; }

    [JsonProperty("confirmed")]
    public bool Confirmed { get; set; }

    [JsonProperty("csgo_settings")]
    public CsgoSettings CsgoSettings { get; set; }

    [JsonProperty("custom_domain")]
    public string CustomDomain { get; set; }

    [JsonProperty("duplicate_source_server")]
    public string DuplicateSourceServer { get; set; }

    [JsonProperty("game")]
    public string Game { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("ip")]
    public string Ip { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }

    [JsonProperty("match_id")]
    public string MatchId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("on")]
    public bool On { get; set; }

    [JsonProperty("players_online")]
    public int PlayersOnline { get; set; }

    [JsonProperty("ports")]
    public Ports Ports { get; set; }

    [JsonProperty("prefer_dedicated")]
    public bool PreferDedicated { get; set; }

    [JsonProperty("private_ip")]
    public string PrivateIp { get; set; }

    [JsonProperty("raw_ip")]
    public string RawIp { get; set; }

    [JsonProperty("reboot_on_crash")]
    public bool RebootOnCrash { get; set; }

    [JsonProperty("server_error")]
    public string ServerError { get; set; }

    [JsonProperty("server_image")]
    public string ServerImage { get; set; }

    [JsonProperty("teamspeak3_settings")]
    public Teamspeak3Settings Teamspeak3Settings { get; set; }
}