using DB.Query.Core.Annotations;
using DB.Query.Core.Annotations.Entity;

namespace DB.Query.Tests.Domain.CommercialDB
{
    [Table("OrderItem")]
    public class OrderItem : CommercialDB
    {
        [PrimaryKey(Identity = true)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal ProductPrice { get; set; }

        [Column("Order_ID")]
        public int OrderId { get; set; }

        [Ignore]
        public decimal Subtotal => Quantity * ProductPrice;
    }
}
