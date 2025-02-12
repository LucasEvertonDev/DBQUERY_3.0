using System;

namespace DB.Query.Core.Annotations.Entity
{
    public class ForeignKeyAttribute : Attribute
    {
        private Type _table { get; set; }

        public ForeignKeyAttribute(Type type)
        {
            _table = type;
        }
    }

    public class ReferencedByForeignKeyAttribute : Attribute
    {
        public Type _table { get; set; }

        public ReferencedByForeignKeyAttribute(Type type)
        {
            _table = type;
        }
    }
}
