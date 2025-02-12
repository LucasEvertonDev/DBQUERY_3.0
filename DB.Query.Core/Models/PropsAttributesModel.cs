using DB.Query.Core.Entities;
using System;

namespace DB.Query.Core.Models
{
    public class PropsAttributesModel<TEntity> where TEntity : EntityBase
    {
        public PropsAttributesModel()
        {
        }

        public Type Type { get; set; }

        public string Name { get; set; }

        public bool Identity { get; set; }

        public bool PrimaryKey { get; set; }

        public object Valor { get; set; }

        public string GetFullName(string TableName)
        {
            return string.Concat(TableName, ".", Name);
        }
    }
}
