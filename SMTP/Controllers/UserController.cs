using Newtonsoft.Json;
using SMTP.Entities;
using SMTP.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace SMTP.Controllers
{
    public class UserController : IUserController
    {
        private readonly IEnumerable<User> _users;

        public UserController()
        {
            var text = File.ReadAllText("Files/users.json");
            _users = JsonConvert.DeserializeObject<IEnumerable<User>>(text);
        }

        public IEnumerable<User> GetUsers() => _users;
    }
}
