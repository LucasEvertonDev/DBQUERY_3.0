using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace DB.Query.Cli.CodeForge.Stored
{
    public static class SqlClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameterName"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="parameterDirection"></param>
        public static void AddParameter(this SqlCommand sqlCommand, string parameterName, SqlDbType sqlDbType, object value, int? size = null, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            if (size.HasValue)
            {
                sqlCommand.Parameters.Add(new SqlParameter(parameterName, sqlDbType, size.Value) { Value = value == null ? DBNull.Value : value, Direction = parameterDirection });
            }
            else
            {
                sqlCommand.Parameters.Add(new SqlParameter(parameterName, sqlDbType) { Value = value == null ? DBNull.Value : value, Direction = parameterDirection });
            }
        }
    }
}
