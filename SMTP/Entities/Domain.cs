using Newtonsoft.Json;

namespace SMTP.Entities
{
    public class Domain
    {
        [JsonProperty(PropertyName = "domain")]
        public string Dom { get; set; }

        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
