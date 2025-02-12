using System;

namespace DB.Query.Core.Annotations.Entity
{
    public partial class TableAttribute : Attribute
    {
        public string TableName { get; set; }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
