using Newtonsoft.Json;

namespace SMTP.Entities
{
    public class Setting
    {
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }

        [JsonProperty(PropertyName = "relay")]
        public bool Relay { get; set; }

        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }
    }
}
