using DB.Query.Core.Annotations;
using DB.Query.Core.Entities;

namespace DB.Query.Tests.Domain.Databases
{
    [Database("AuthDb")]
    public partial class AuthDb : EntityBase
    {
    }
}
