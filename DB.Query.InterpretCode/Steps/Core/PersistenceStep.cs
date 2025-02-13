using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Services.Others;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.Utils.Extensions;

namespace DB.Query.InterpretCode.Steps.Core
{
    public class PersistenceStep<TEntity> : DBQueryBuilder<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
        /// <summary>
        ///     Retorna a querie montada
        /// </summary>
        /// <returns>
        ///     Retorno do tipo string
        /// </returns>
        public string GetQuery()
        {
            var query = StartTranslateQuery();
            while (query.Contains("  "))
            {
                query = query.Replace("  ", " ");
            }
            ClearOldConfigurations();
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string StartTranslateQuery()
        {
            return Activator.CreateInstance<InterpretService<TEntity>>().StartToInterpret(_steps);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string StartTranslateQuery1()
        {
            return Activator.CreateInstance<InterpretService<TEntity>>().StartToInterpret(_steps);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string StartTranslateQuery2()
        {
            return Activator.CreateInstance<InterpretServiceOldVersion<TEntity>>().StartToInterpret(_steps);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected dynamic ExecuteSql()
        {
            var query = StartTranslateQuery();

            try
            {
                if (_transaction == null)
                {
                    throw new Exception("Transação nula. Setar a transação do repository BindTransaction()");
                }

                VerifyChangeDataBase();

                if (_transaction.ExecutedInDebug())
                {
                    LogService.PrintQuery(query);
                }

                SqlCommand Sql_Comando = new SqlCommand(query, _transaction.GetConnection(), _transaction.GetTransaction()) { CommandType = CommandType.Text };
                if (_steps.Exists(a => a.StepType == StepType.SELECT || a.StepType == StepType.CUSTOM_SELECT))
                {
                    return Sql_Comando.ExecuteSql();
                }
                else if (_steps.Exists(a => a.StepType == StepType.UPDATE || a.StepType == StepType.DELETE))
                {
                    return Sql_Comando.ExecuteNonQuery();
                }
                else
                {
                    return Sql_Comando.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao executar o script ({query}) -> {e.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task<dynamic> ExecuteSqlAsync()
        {
            var query = StartTranslateQuery();
            try
            {
                if (_transaction == null)
                {
                    throw new Exception("Transação nula. Setar a transação do repository BindTransaction()");
                }

                await VerifyChangeDataBaseAsync();

                if (_transaction.ExecutedInDebug())
                {
                    LogService.PrintQuery(query);
                }

                SqlCommand Sql_Comando = new SqlCommand(query, _transaction.GetConnection(), _transaction.GetTransaction()) { CommandType = CommandType.Text };
                if (_steps.Exists(a => a.StepType == StepType.SELECT || a.StepType == StepType.CUSTOM_SELECT))
                {
                    return await Sql_Comando.ExecuteSqlAsync();
                }
                else if (_steps.Exists(a => a.StepType == StepType.UPDATE || a.StepType == StepType.DELETE))
                {
                    return await Sql_Comando.ExecuteNonQueryAsync();
                }
                else
                {
                    return await Sql_Comando.ExecuteScalarAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao executar o script ({query}) -> {e.Message}");
            }
        }
    }
}
