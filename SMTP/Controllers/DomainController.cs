using Newtonsoft.Json;
using SMTP.Entities;
using SMTP.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace SMTP.Controllers
{
    public class DomainController : IDomainController
    {
        private readonly IEnumerable<Domain> _domains;

        public DomainController()
        {
            var text = File.ReadAllText("Files/domains.json");
            _domains = JsonConvert.DeserializeObject<IEnumerable<Domain>>(text);
        }

        public IEnumerable<Domain> GetDomains() => _domains;
    }
}
