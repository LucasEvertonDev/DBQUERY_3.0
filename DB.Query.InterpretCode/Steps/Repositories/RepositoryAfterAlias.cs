using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Steps.Repositories.Interfaces;

namespace DB.Query.InterpretCode.Steps.Repositories
{
    public class RepositoryAfterAlias<TEntity> : RepositoryBase<TEntity>, IRepository where TEntity : EntityBase
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
