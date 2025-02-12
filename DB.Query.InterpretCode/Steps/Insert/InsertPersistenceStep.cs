using System.Threading.Tasks;
using DB.Query.InterpretCode.Steps.Core;
using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Services.InterpretServices;

namespace DB.Query.InterpretCode.Steps.Insert
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InsertPersistenceStep<TEntity> : PersistenceStep<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        /// Retorna o Identity id inserido
        /// </returns>
        public dynamic Execute()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new InsertResultStep<TEntity>(res).GetIdentityId();
        }

        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        /// Retorna o Identity id inserido
        /// </returns>
        public async Task<dynamic> ExecuteAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new InsertResultStep<TEntity>(res).GetIdentityId();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string StartTranslateQuery()
        {
            return new InterpretInsertService<TEntity>(_transaction).StartToInterpret(_steps);
        }
    }
}
