using System.Data;
using System;

namespace DB.Query.Cli.CodeForge.Stored
{
    public class ParameterTypeMap
    {
        public Type ClrType;
        public SqlDbType DbType;
        public int? LengthDivisor = null;
    }
}
