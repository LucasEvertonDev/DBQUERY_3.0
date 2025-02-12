using System.Collections.Generic;
using System.Data;
using System;
using System.Threading.Tasks;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.InterpretCode.Steps.Core;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Select
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SelectPersistenceStep<TEntity> : PersistenceStep<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        ///     Retorno do tipo ResultStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação
        /// </returns>
        public SelectResultStep<TEntity> Execute()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res);
        }

        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        ///     Retorno do tipo ResultStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação
        /// </returns>
        public dynamic Execute<T>()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            if (typeof(DataTable) == typeof(T))
            {
                return new SelectResultStep<TEntity>(res).ToDataTable();
            }
            else if (typeof(T).IsSubclassOf(typeof(EntityBase)))
            {
                return new SelectResultStep<TEntity>(res).ToList<T>();
            }
            return new SelectResultStep<TEntity>(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TEntity First()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<TEntity> FirstAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TEntity FirstOrDefault()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<TEntity> FirstOrDefaultAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TEntity> ToList()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<TEntity>> ToListAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TRet> ToList<TRet>()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).ToList<TRet>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<TRet>> ToListAsync<TRet>()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).ToList<TRet>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).ToDataTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> ToDataTableAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new SelectResultStep<TEntity>(res).ToDataTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string StartTranslateQuery()
        {
            return Activator.CreateInstance<InterpretSelectService<TEntity>>().StartToInterpret(_steps);
        }
    }
}
