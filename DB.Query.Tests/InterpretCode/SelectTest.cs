using DB.Query.Core;
using DB.Query.Tests.Domain.AuthDb;
using DB.Query.Tests.Domain.CommercialDB;
using DB.Query.Tests.Domain.FinanceDB;
using DB.Query.Tests.Domain.LogisticDB;
using DB.Query.Tests.Helpers;
using DB.Query.Utils.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DB.Query.Tests.InterpretCode
{
    [TestClass]
    public class SelectTest : DBQueryTransaction
    {
        public SelectTest() 
        {
            _transaction = new Query.InterpretCode.Transaction.DBTransaction();
        }

        [TestMethod]
        public void Select()
        {
            var query = _transaction.Query<User>()
                .Select<User, PaymentInfo, Order>(
                    (User, PaymentInfo, Order) => new 
                    {
                        Order.Date,
                        User.Id,
                        AllColumns = PaymentInfo.AllColumns()
                    })
                .Distinct()
                .LeftJoin<User, ShipmentInfo>(
                    (User, ShipmentInfo) => User.Id == ShipmentInfo.Id)
                .Join<User, PaymentInfo>(
                    (User, PaymentInfo) => User.Id == PaymentInfo.UserId)
                .Join<PaymentInfo, Order>(
                    (PaymentInfo, Order) => PaymentInfo.OrderId == Order.Id)
                .Where<User, PaymentInfo, ShipmentInfo>(
                    (User, PaymentInfo, ShipmentInfo) => User.Name == "UserName"
                        && PaymentInfo.Currency.IN("USD", "BRL")
                        && User.Email.LIKE("%dbquery.com")
                        && PaymentInfo.PaymentDate == null
                        && ShipmentInfo.Id == null)
                .GetQuery();

            var expectedQuery = @"
                SELECT
                    DISTINCT Order.Date AS Date,
                    Users.Id AS Id,
                    Payment_Info.*
                FROM
                    AuthDb..Users
                    LEFT JOIN LogisticDB..ShipmentInfo ON Users.Id = ShipmentInfo.Id
                    INNER JOIN AppDb..Payment_Info ON Users.Id = Payment_Info.User_ID
                    INNER JOIN CommercialDB..Order ON Payment_Info.Order_ID = Order.Id
                WHERE
                    (
                        (
                            (
                                (
                                    Users.Name = 'UserName'
                                    AND Payment_Info.Currency IN ('USD', 'BRL')
                                )
                                AND Users.Email LIKE '%dbquery.com'
                            )
                            AND Payment_Info.PaymentDate IS NULL
                        )
                        AND ShipmentInfo.Id IS NULL
                    )";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }

        [TestMethod]
        public void Select2()
        {
            var query = _transaction.Query<User>()
                .Select<User, OrderItem>(
                    (User, OrderItem) => new
                    {
                        UserId = User.Id,
                        Value = SUM<decimal>($"{Name<OrderItem>(_ => _.ProductPrice)} * {Name<OrderItem>(_ => _.Quantity)}")
                    })
                .Distinct()
                .LeftJoin<User, ShipmentInfo>(
                    (User, ShipmentInfo) => User.Id == ShipmentInfo.Id)
                .Join<User, PaymentInfo>(
                    (User, PaymentInfo) => User.Id == PaymentInfo.UserId)
                .Join<PaymentInfo, Order>(
                    (PaymentInfo, Order) => PaymentInfo.OrderId == Order.Id)
                .Join<Order, OrderItem>(
                    (Order, OrderItem) => Order.Id == OrderItem.Id)
                .Where<User, PaymentInfo, ShipmentInfo>(
                    (User, PaymentInfo, ShipmentInfo) => 
                        PaymentInfo.Currency.IN("USD", "BRL")
                        && User.Email.LIKE("%dbquery.com")
                        && PaymentInfo.PaymentDate == null
                        && ShipmentInfo.Id == null)
                .GroupBy<User>(
                    (User) => User.Id)
                .GetQuery();

            var expectedQuery = @"
                SELECT
                    DISTINCT Users.Id AS UserId,
                    SUM(OrderItem.ProductPrice * OrderItem.Quantity) AS Value
                FROM
                    AuthDb..Users
                    LEFT JOIN LogisticDB..ShipmentInfo ON Users.Id = ShipmentInfo.Id
                    INNER JOIN AppDb..Payment_Info ON Users.Id = Payment_Info.User_ID
                    INNER JOIN CommercialDB..Order ON Payment_Info.Order_ID = Order.Id
                    INNER JOIN CommercialDB..OrderItem ON Order.Id = OrderItem.Id
                WHERE
                    (
                        (
                            (
                                Payment_Info.Currency IN ('USD', 'BRL')
                                AND Users.Email LIKE '%dbquery.com'
                            )
                            AND Payment_Info.PaymentDate IS NULL
                        )
                        AND ShipmentInfo.Id IS NULL
                    )
                GROUP BY
                    Users.Id";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }

        [TestMethod]
        public void Select3()
        {
            var query = _transaction.Query<User>()
                .Select(User => Count())
                .LeftJoin<User, ShipmentInfo>(
                    (User, ShipmentInfo) => User.Id == ShipmentInfo.Id)
                .Join<User, PaymentInfo>(
                    (User, PaymentInfo) => User.Id == PaymentInfo.UserId)
                .Join<PaymentInfo, Order>(
                    (PaymentInfo, Order) => PaymentInfo.OrderId == Order.Id)
                .Join<Order, OrderItem>(
                    (Order, OrderItem) => Order.Id == OrderItem.Id)
                .Where<User, PaymentInfo, ShipmentInfo>(
                    (User, PaymentInfo, ShipmentInfo) =>
                        PaymentInfo.Currency.IN("USD", "BRL")
                        && User.Email.LIKE("%dbquery.com")
                        && PaymentInfo.PaymentDate == null
                        && ShipmentInfo.Id == null)
                .GetQuery();

            var expectedQuery = @"
                SELECT
                    COUNT(*)
                FROM
                    AuthDb..Users
                    LEFT JOIN LogisticDB..ShipmentInfo ON Users.Id = ShipmentInfo.Id
                    INNER JOIN AppDb..Payment_Info ON Users.Id = Payment_Info.User_ID
                    INNER JOIN CommercialDB..Order ON Payment_Info.Order_ID = Order.Id
                    INNER JOIN CommercialDB..OrderItem ON Order.Id = OrderItem.Id
                WHERE
                    (
                        (
                            (
                                Payment_Info.Currency IN ('USD', 'BRL')
                                AND Users.Email LIKE '%dbquery.com'
                            )
                            AND Payment_Info.PaymentDate IS NULL
                        )
                        AND ShipmentInfo.Id IS NULL
                    )";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }

        [TestMethod]
        public void Select4()
        {
            var query = _transaction.Query<User>()
                .Select<User>(User => new
                {
                    User.Name,
                    TotalOrders = SQL<int>(
                        _transaction.Query<Order>()
                            .Select<User>(User => Count())
                            .Where<Order, User>(
                                (Order, User) => Order.UserId == User.Id)
                            .GetQuery())
                })
                .LeftJoin<User, ShipmentInfo>(
                    (User, ShipmentInfo) => User.Id == ShipmentInfo.Id)
                .Join<User, PaymentInfo>(
                    (User, PaymentInfo) => User.Id == PaymentInfo.UserId)
                .Join<PaymentInfo, Order>(
                    (PaymentInfo, Order) => PaymentInfo.OrderId == Order.Id)
                .Join<Order, OrderItem>(
                    (Order, OrderItem) => Order.Id == OrderItem.Id)
                .Where<User, PaymentInfo, ShipmentInfo>(
                    (User, PaymentInfo, ShipmentInfo) =>
                        PaymentInfo.Currency.IN("USD", "BRL")
                        && User.Email.LIKE("%dbquery.com")
                        && PaymentInfo.PaymentDate == null
                        && ShipmentInfo.Id == null)
                .GetQuery();

            var expectedQuery = @"
                SELECT
                    Users.Name AS Name,
                    (
                        SELECT
                            COUNT(*)
                        FROM
                            CommercialDB..Order
                        WHERE
                            Order.User_ID = Users.Id
                    ) AS TotalOrders
                FROM
                    AuthDb..Users
                    LEFT JOIN LogisticDB..ShipmentInfo ON Users.Id = ShipmentInfo.Id
                    INNER JOIN AppDb..Payment_Info ON Users.Id = Payment_Info.User_ID
                    INNER JOIN CommercialDB..Order ON Payment_Info.Order_ID = Order.Id
                    INNER JOIN CommercialDB..OrderItem ON Order.Id = OrderItem.Id
                WHERE
                    (
                        (
                            (
                                Payment_Info.Currency IN ('USD', 'BRL')
                                AND Users.Email LIKE '%dbquery.com'
                            )
                            AND Payment_Info.PaymentDate IS NULL
                        )
                        AND ShipmentInfo.Id IS NULL
                    )";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }

    }
}
