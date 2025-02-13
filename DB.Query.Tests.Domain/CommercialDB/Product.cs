using DB.Query.Core.Annotations.Entity;

namespace DB.Query.Tests.Domain.CommercialDB
{
    [Table("Product")]
    public class Product : CommercialDB
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public decimal ProductPrice { get; set; }

        public int StockQuantity { get; set; }

        public string Category { get; set; }

        public string ProductCode { get; set; }
    }
}
