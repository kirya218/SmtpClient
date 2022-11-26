using SMTP.Entities;
using System.Collections.Generic;

namespace SMTP.Interfaces
{
    public interface IDomainController
    {
        IEnumerable<Domain> GetDomains();
    }
}
