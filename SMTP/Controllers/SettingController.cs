using Newtonsoft.Json;
using SMTP.Entities;
using SMTP.Interfaces;
using System.IO;

namespace SMTP.Controllers
{
    public class SettingController : ISettingController
    {
        private readonly Setting _settings;

        public SettingController()
        {
            var text = File.ReadAllText("Files/settings.json");
            _settings = JsonConvert.DeserializeObject<Setting>(text);
        }

        public Setting GetSettings() => _settings;
    }
}
