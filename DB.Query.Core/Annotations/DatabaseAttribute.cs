using System;

namespace DB.Query.Core.Annotations
{
    public partial class DatabaseAttribute : Attribute
    {
        public string DatabaseName { get; set; }

        public DatabaseAttribute(string databaseName)
        {
            DatabaseName = databaseName;
        }
    }
}
