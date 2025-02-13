using DB.Query.Core.Models;

namespace DB.Query
{
    public static class DBQuery
    {
        public static void Use(string dbConnection, string auditLogsTable = null)
        {
            DbQueryConfiguration.SqlConnection = dbConnection;
            DbQueryConfiguration.AuditLogsDatabase = auditLogsTable;
        }
    }
}
