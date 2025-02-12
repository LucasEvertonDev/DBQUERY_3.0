using System;

namespace DB.Query.Core.Annotations.Entity
{
    public partial class PrimaryKeyAttribute : Attribute
    {
        public bool Identity { get; set; }

        public PrimaryKeyAttribute()
        {
        }
    }
}
