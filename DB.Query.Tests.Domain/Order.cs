using DB.Query.Core.Annotations.Entity;
using DB.Query.Tests.Domain.Databases;
using System;

namespace DB.Query.Tests.Domain
{
    [Table("Order")]
    public class Order : CommercialDB
    {
        [PrimaryKey(Identity = true)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Column("User_ID")]
        public int UserId { get; set; }
    }
}
