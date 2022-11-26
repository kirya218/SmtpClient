using Newtonsoft.Json;

namespace SMTP.Entities
{
    public class User
    {
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
    }
}