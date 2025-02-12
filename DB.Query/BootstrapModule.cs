using DB.Query.Core.Models;

namespace DB.Query
{
    public static class BootstrapModule
    {
        public static void UseDbQuery(string dbConnection, string auditLogsTable)
        {
            DbQueryConfiguration.SqlConnection = dbConnection;
            DbQueryConfiguration.AuditLogsDatabase = auditLogsTable;
        }
    }
}
