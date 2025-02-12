using System.Threading.Tasks;
using DB.Query.InterpretCode.Steps.Core;
using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Services.InterpretServices;

namespace DB.Query.InterpretCode.Steps.Update
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class UpdatePersistenceStep<TEntity> : PersistenceStep<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        ///    Retorna o númeto de registros afetados
        /// </returns>
        public int Execute()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new UpdateResultStep<TEntity>(res).GetNumeroRegistrosAfetados();
        }


        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        ///    Retorna o númeto de registros afetados
        /// </returns>
        public async Task<int> ExecuteAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new UpdateResultStep<TEntity>(res).GetNumeroRegistrosAfetados();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string StartTranslateQuery()
        {
            return new InterpretUpdateService<TEntity>(_transaction).StartToInterpret(_steps);
        }
    }
}
