using Newtonsoft.Json;

public class UserRegisterModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("dob")]
    public DateTime Dob { get; set; }

    [JsonProperty("gender")]
    public bool Gender { get; set; }

    [JsonProperty("zoneAddress")]
    public string ZoneAddress { get; set; } = "";
}
