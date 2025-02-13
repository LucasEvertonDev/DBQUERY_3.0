using DB.Query.Core;
using DB.Query.Tests.Domain;
using DB.Query.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DB.Query.Tests.InterpretCode
{
    [TestClass]
    public class DeleteTest : DBQueryTransaction
    {
        public DeleteTest()
        {
            _transaction = new Query.InterpretCode.Transaction.DBTransaction();
        }

        [TestMethod]
        public void Delete()
        {
            var userToSave = new User()
            {
                Email = "dbquery@query.com",
                Name = "dbquery"
            };

            var query = _transaction.Query<User>()
                .Delete()
                .Where(User => User.Email == userToSave.Email)
                .GetQuery();

            var expectedQuery = @"
                DELETE FROM
                    AuthDb..Users
                WHERE
                    Users.Email = 'dbquery@query.com'";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }
    }
}
