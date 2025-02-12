using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using System.Collections.Generic;

namespace DB.Query.Core.Models
{
    public class EntityAttributesModel<TEntity> where TEntity : EntityBase
    {
        public EntityAttributesModel()
        {
            Props = new List<PropsAttributesModel<TEntity>>();
        }

        public string Name { get; set; }
        public string Database { get; set; }
        public List<PropsAttributesModel<TEntity>> Props { get; set; }

        public string FullName
        {
            get
            {
                return string.Concat(Database, DBKeysConstants.T_A, Name);
            }
        }
    }
}
