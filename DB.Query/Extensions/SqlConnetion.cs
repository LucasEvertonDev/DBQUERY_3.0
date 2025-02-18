using DB.Query.InterpretCode.Transaction;
using Microsoft.Data.SqlClient;

namespace DB.Query.Extensions
{
    public static class SqlConnetion
    {
        public static DBTransaction InstanceDbQueryTransaction(this SqlConnection sqlConnection, SqlTransaction transaction)
        {
            DBTransaction dbTransaction = new DBTransaction();
            dbTransaction.SetDbTransaction(sqlConnection, transaction);
            return dbTransaction;
        }
    }
}
