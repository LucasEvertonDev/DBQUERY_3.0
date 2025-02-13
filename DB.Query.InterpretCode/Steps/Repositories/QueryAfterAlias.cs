using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Steps.Repositories.Interfaces;

namespace DB.Query.InterpretCode.Steps.Repositories
{
    public class QueryAfterAlias<TEntity> : QueryBase<TEntity>, IQuery where TEntity : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void StartLevel()
        {
            base.StartLevel();
        }
    }
}
