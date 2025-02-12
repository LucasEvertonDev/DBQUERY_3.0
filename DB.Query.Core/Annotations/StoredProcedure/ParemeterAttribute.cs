using System;
using System.Data;

namespace DB.Query.Core.Annotations.StoredProcedure
{
    public partial class ParemeterAttribute : Attribute
    {
        public string ParameterName { get; set; }

        public SqlDbType ParameterType { get; set; }

        public int? ParameterSize { get; set; }

        public ParameterDirection ParameterDirection { get; set; }

        public ParemeterAttribute(string parameter, SqlDbType type, int size = 0, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            ParameterName = parameter;
            ParameterType = type;
            if (size > 0)
            {
                ParameterSize = size;
            }
            ParameterDirection = parameterDirection;
        }

        public ParemeterAttribute(SqlDbType type, int size = 0, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            ParameterType = type;
            if (size > 0)
            {
                ParameterSize = size;
            }
            ParameterDirection = parameterDirection;
        }
    }
}
