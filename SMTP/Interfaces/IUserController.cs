using SMTP.Entities;
using System.Collections.Generic;

namespace SMTP.Interfaces
{
    public interface IUserController
    {
        IEnumerable<User> GetUsers();
    }
}
