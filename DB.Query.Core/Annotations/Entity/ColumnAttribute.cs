using System.ComponentModel;

namespace DB.Query.Core.Annotations.Entity
{
    public partial class ColumnAttribute : DisplayNameAttribute
    {
        public ColumnAttribute(string displayName) : base(displayName)
        {

        }
    }
}
