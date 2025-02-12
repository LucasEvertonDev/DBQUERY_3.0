using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Repositories.Interfaces
{
    public interface IRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        RepositoryAfterAlias<TEntity> UseAlias(string alias);
    }

    public interface IRepository
    {
    }
}