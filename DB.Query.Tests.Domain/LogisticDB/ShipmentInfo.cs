using DB.Query.Core.Annotations.Entity;

namespace DB.Query.Tests.Domain.LogisticDB
{
    public class ShipmentInfo : LogisticDB
    {
        [PrimaryKey(Identity = true)]
        public int? Id { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        [Column("User_ID")]
        public int UserId { get; set; }
    }
}
