using System;

namespace DB.Query.Core.Annotations.Entity
{
    public partial class PrimaryKeyNameAttribute : Attribute
    {
        public string PrimaryKeyName { get; set; }

        public PrimaryKeyNameAttribute(string primaryKey)
        {
            PrimaryKeyName = primaryKey;
        }
    }
}
