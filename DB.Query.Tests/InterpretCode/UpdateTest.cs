using DB.Query.Core;
using DB.Query.Tests.Domain.AuthDb;
using DB.Query.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DB.Query.Tests.InterpretCode
{
    [TestClass]
    public class UpdateTest : DBQueryTransaction
    {
        public UpdateTest()
        {
            _transaction = new Query.InterpretCode.Transaction.DBTransaction();
        }

        [TestMethod]
        public void Update()
        {
            var userToSave = new User()
            {
                Email = "dbquery@query.com",
                Name = "dbquery"
            };

            var query = _transaction.Query<User>()
                .Update(userToSave)
                .SetColumns(User => new
                {
                    User.Name,
                    User.Email
                })
                .Where(User => User.Email == userToSave.Email)
                .GetQuery();

            var expectedQuery = @"
                UPDATE
                    AuthDb..Users
                SET
                    Name = 'dbquery',
                    Email = 'dbquery@query.com'
                WHERE
                    Users.Email = 'dbquery@query.com'";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }
    }
}
