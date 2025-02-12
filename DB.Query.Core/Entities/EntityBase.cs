using DB.Query.Core.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.Query.Core.Entities
{
    [Database("Master")]
    [Table("main")]
    public partial class EntityBase : IEntityBase
    {
        /// <summary>
        /// *
        /// </summary>
        /// <returns></returns>
        [Ignore]
        public object AllColumns()
        {
            return null;
        }
    }
}
