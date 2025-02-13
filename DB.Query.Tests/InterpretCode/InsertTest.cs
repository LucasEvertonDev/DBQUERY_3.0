using DB.Query.Core;
using DB.Query.Tests.Domain.AuthDb;
using DB.Query.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DB.Query.Tests.InterpretCode
{
    [TestClass]
    public class InsertTest : DBQueryTransaction
    {
        public InsertTest()
        {
            _transaction = new Query.InterpretCode.Transaction.DBTransaction();
        }


        [TestMethod]
        public void Insert()
        {
            var userToSave = new User()
            {
                Email = "dbquery@query.com",
                Name = "dbquery"
            };

            var query = _transaction.Query<User>()
                .Insert(userToSave)
                .GetQuery();

            var expectedQuery = @"
                INSERT INTO
                    AuthDb..Users (Users.Name, Users.Email) OUTPUT Inserted.Id
                VALUES
                    ('dbquery', 'dbquery@query.com')";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }

        [TestMethod]
        public void InsertIfNotExists()
        {
            var userToSave = new User()
            {
                Email = "dbquery@query.com",
                Name = "dbquery"
            };

            var query = _transaction.Query<User>()
                .InsertIfNotExists(userToSave)
                .Where(User =>
                    User.Email == userToSave.Email)
                .GetQuery();

            var expectedQuery = @"
                IF NOT EXISTS(
                    SELECT
                        *
                    FROM
                        AuthDb..Users
                    WHERE
                        Users.Email = 'dbquery@query.com'
                ) BEGIN
                INSERT INTO
                    AuthDb..Users (Users.Name, Users.Email) OUTPUT Inserted.Id
                VALUES
                    ('dbquery', 'dbquery@query.com')
                END";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }


        [TestMethod]
        public void InsertOrUpdate()
        {
            var userToSave = new User()
            {
                Email = "dbquery@query.com",
                Name = "dbquery"
            };

            var query = _transaction.Query<User>()
                .InsertOrUpdate(userToSave)
                .SetColumns(User => User.Name)
                .Where(User =>
                    User.Email == userToSave.Email)
                .GetQuery();

            var expectedQuery = @"
                IF NOT EXISTS(
                    SELECT
                        *
                    FROM
                        AuthDb..Users
                    WHERE
                        Users.Email = 'dbquery@query.com'
                ) BEGIN
                INSERT INTO
                    AuthDb..Users (Users.Name, Users.Email) OUTPUT Inserted.Id
                VALUES
                    ('dbquery', 'dbquery@query.com')
                END
                ELSE BEGIN
                UPDATE
                    AuthDb..Users
                SET
                    Name = 'dbquery'
                WHERE
                    Users.Email = 'dbquery@query.com'
                END";

            Assert.AreEqual(StringHelper.NormalizeQuery(query), StringHelper.NormalizeQuery(expectedQuery));
        }
    }
}
