using System.Threading.Tasks;
using DB.Query.InterpretCode.Steps.Core;
using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Services.InterpretServices;

namespace DB.Query.InterpretCode.Steps.Delete
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DeletePersistenceStep<TEntity> : PersistenceStep<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        ///   Retorna o numero de registros afetados
        /// </returns>
        public int Execute()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new DeleteResultStep<TEntity>(res).GetNumeroRegistrosAfetados();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExecuteAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new DeleteResultStep<TEntity>(res).GetNumeroRegistrosAfetados();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string StartTranslateQuery()
        {
            return new InterpretDeleteService<TEntity>(_transaction).StartToInterpret(_steps);
        }
    }
}
