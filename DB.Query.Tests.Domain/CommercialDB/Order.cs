using DB.Query.Core.Annotations.Entity;
using System;

namespace DB.Query.Tests.Domain.CommercialDB
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
