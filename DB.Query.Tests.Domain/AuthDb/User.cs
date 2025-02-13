using DB.Query.Core.Annotations.Entity;

namespace DB.Query.Tests.Domain.AuthDb
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
