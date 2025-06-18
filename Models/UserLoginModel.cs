using Newtonsoft.Json;
namespace MovieTicketWebsite.Models
{
    public class UserLoginModel
    {
        [JsonProperty("username")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
