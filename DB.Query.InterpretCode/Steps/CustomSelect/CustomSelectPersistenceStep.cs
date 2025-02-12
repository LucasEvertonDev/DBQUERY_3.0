
using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DB.Query.InterpretCode.Steps.CustomSelect
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CustomSelectPersistenceStep<TEntity> : PersistenceStep<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
        /// <summary>
        ///     Realiza a execução de toda a querie montada
        /// </summary>
        /// <returns>
        ///     Retorno do tipo ResultStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação
        /// </returns>
        public CustomSelectResultStep<TEntity> Execute()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res);
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
                return new CustomSelectResultStep<TEntity>(res).ToDataTable();
            }
            else if (typeof(T).IsSubclassOf(typeof(EntityBase)))
            {
                return new CustomSelectResultStep<TEntity>(res).ToList<T>();
            }
            return new CustomSelectResultStep<TEntity>(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TEntity First()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).First<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<TEntity> FirstAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).First<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T First<T>()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).First<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<T> FirstAsync<T>()
        {
            var res = await ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).First<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public dynamic FirstOrDefault()
        {
            var res = ExecuteSql();
            var type = _steps.Where(st => st.StepType == Query.Core.Enuns.StepType.CUSTOM_SELECT).First().ReturnType;
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).FirstOrDefault(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> FirstOrDefaultAsync()
        {
            var res = await ExecuteSqlAsync();
            var type = _steps.Where(st => st.StepType == Query.Core.Enuns.StepType.CUSTOM_SELECT).First().ReturnType;
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).FirstOrDefault(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T FirstOrDefault<T>()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).FirstOrDefault<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync<T>()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).FirstOrDefault<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TEntity> ToList()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).ToList<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<TEntity>> ToListAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).ToList<TEntity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<T> ToList<T>()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).ToList<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> ToListAsync<T>()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).ToList<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            var res = ExecuteSql();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).ToDataTable();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> ToDataTableAsync()
        {
            var res = await ExecuteSqlAsync();
            ClearOldConfigurations();
            return new CustomSelectResultStep<TEntity>(res).ToDataTable();
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
