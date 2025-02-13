using DB.Query.Core.Annotations.Entity;
using System;

namespace DB.Query.Tests.Domain.FinanceDB
{
    [Table("Payment_Info")]
    public class PaymentInfo : FinanceDb
    {
        [PrimaryKey(Identity = true)]
        public int Id { get; set; }

        public int PaymentId { get; set; }

        [Column("Full_Name")]
        public string FullName { get; set; }

        public string PaymentMethod { get; set; }

        public string TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Column("User_ID")]
        public int UserId { get; set; }

        [Column("Order_ID")]
        public int OrderId { get; set; }
    }
}
