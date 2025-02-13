using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Repositories.Interfaces
{
    public interface IQuery<TEntity> : IQueryBase<TEntity> where TEntity : EntityBase
    {
        QueryAfterAlias<TEntity> UseAlias(string alias);
    }

    public interface IQuery
    {
    }
}