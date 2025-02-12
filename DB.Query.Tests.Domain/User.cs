using DB.Query.Core.Annotations.Entity;
using DB.Query.Tests.Domain.Databases;

namespace DB.Query.Tests.Domain
{
    [Table("Users")]
    public class User : AuthDb
    {
        [PrimaryKey(Identity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
