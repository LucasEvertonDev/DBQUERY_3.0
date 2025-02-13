using DB.Query.Core.Annotations.StoredProcedure;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Services.Others;
using SIGN.Query.Models.PersistenceContext.Entities.SignCi;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Linq;
using DB.Query.InterpretCode.Steps.Repositories;
using DB.Query.Core.Models;
using DB.Query.Core.Annotations;
using DB.Query.Core.Entities;
using DB.Query.Utils.Extensions;
using DB.Query.InterpretCode.Factorys;

namespace DB.Query.InterpretCode.Transaction
{
    public class DBTransaction
    {
        protected bool hasCommit { get; set; }
        protected bool hasRoolback { get; set; }
        protected SqlConnection _sqlConnection { get; set; }
        protected SqlTransaction _sqlTransaction { get; set; }
        protected bool _onDebug { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedures"></param>
        public virtual void OpenTransaction(string conexao)
        {
            /// Apenas se não tiver transação corrente
            if ((_sqlConnection == null && _sqlTransaction == null) || _sqlConnection.State == ConnectionState.Closed)
            {
                _sqlConnection = new SqlConnection(conexao);
                _sqlConnection.Open();
                _sqlTransaction = _sqlConnection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 2));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedures"></param>
        public virtual async Task OpenTransactionAsync(string conexao)
        {
            /// Apenas se não tiver transação corrente
            if ((_sqlConnection == null && _sqlTransaction == null) || _sqlConnection.State == ConnectionState.Closed)
            {
                _sqlConnection = new SqlConnection(conexao);
                await _sqlConnection.OpenAsync();
                _sqlTransaction = _sqlConnection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 2));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SqlTransaction GetTransaction()
        {
            return _sqlTransaction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection()
        {
            return _sqlConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        public void SetDbTransaction(SqlConnection connection, SqlTransaction transaction)
        {
            _sqlConnection = connection;
            _sqlTransaction = transaction;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Commit()
        {
            hasCommit = true;
            if (_sqlTransaction != null)
                _sqlTransaction.Commit();
            if (_sqlConnection != null)
                _sqlConnection.Close();

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Rollback()
        {
            hasRoolback = true;
            if (_sqlTransaction != null)
                _sqlTransaction.Rollback();
            if (_sqlConnection != null)
                _sqlConnection.Close();

            return 0;
        }

        public async Task<int> CommitAsync()
        {
            hasCommit = true;
            if (_sqlTransaction != null)
            {
                await _sqlTransaction.CommitAsync();
            }

            if (_sqlConnection != null)
            {
                await _sqlConnection.CloseAsync();
            }

            return 0;
        }

        public async Task<int> RollbackAsync()
        {
            hasRoolback = true;
            if (_sqlTransaction != null)
            {
                await _sqlTransaction.RollbackAsync();
            }

            if (_sqlConnection != null)
            {
                await _sqlConnection.CloseAsync();
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public void ChangeDatabase(string database)
        {
            _sqlConnection.ChangeDatabase(database);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public async Task ChangeDatabaseAsync(string database)
        {
            await _sqlConnection.ChangeDatabaseAsync(database);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasCommited()
        {
            return hasCommit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasReversed()
        {
            return hasRoolback;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Query<T> Query<T>() where T : EntityBase
        {
            var query = Activator.CreateInstance<Query<T>>();
            query.BindTransaction(this);
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Query<T> Query<T>(string tableName) where T : EntityBase
        {
            var query = Activator.CreateInstance<Query<T>>();
            query.BindTransaction(this);
            return query;
        }

        /// <summary>
        /// Utilizado para repositorios Especificos(funções unicas por tabela)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetRepository<T>()
        {
            var repository = Activator.CreateInstance<T>();
            MethodInfo m = repository.GetType().GetMethod("BindTransaction");
            m.Invoke(repository, new object[] { this });
            return (T)repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedureBase"></param>
        private void VerifyDatabaseStoredProcedure(StoredProcedureBase storedProcedureBase)
        {
            var type = storedProcedureBase.GetType();
            var database = type.GetCustomAttributes(typeof(DatabaseAttribute), true).FirstOrDefault() as DatabaseAttribute;
            ChangeDatabase(database.DatabaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedureBase"></param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(StoredProcedureBase storedProcedureBase)
        {
            var command = CreateStoredProcedureCommand(storedProcedureBase);
            try
            {
                VerifyDatabaseStoredProcedure(storedProcedureBase);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao executat a query -> {command.PrintSql()}", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedureBase"></param>
        /// <returns></returns>
        public virtual object ExecuteScalar(StoredProcedureBase storedProcedureBase)
        {
            SqlCommand sqlCommand = CreateStoredProcedureCommand(storedProcedureBase);
            try
            {
                VerifyDatabaseStoredProcedure(storedProcedureBase);

                var ret = sqlCommand.ExecuteScalar();

                return ret;
            }
            catch (Exception innerException)
            {
                throw new Exception("Erro ao executat a query -> " + sqlCommand.PrintSql(), innerException);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedureBase"></param>
        /// <returns></returns>
        public virtual DataTable ExecuteSql(StoredProcedureBase storedProcedureBase)
        {
            var command = CreateStoredProcedureCommand(storedProcedureBase);
            try
            {
                VerifyDatabaseStoredProcedure(storedProcedureBase);
                return command.ExecuteSql();
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao executat a query -> {command.PrintSql()}", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedureBase"></param>
        /// <returns></returns>
        public virtual List<T> ExecuteSql<T>(StoredProcedureBase storedProcedureBase)
        {
            var command = CreateStoredProcedureCommand(storedProcedureBase);
            try
            {
                VerifyDatabaseStoredProcedure(storedProcedureBase);
                return command.ExecuteSql().OfTypeProcedure<T>();
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao executat a query -> {command.PrintSql()}", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedureDTO"></param>
        /// <param name="dataBaseService"></param>
        /// <returns></returns>
        public virtual SqlCommand CreateStoredProcedureCommand(StoredProcedureBase storedProcedure)
        {
            var type = storedProcedure.GetType();
            var procedure = type.GetCustomAttributes(typeof(ProcedureAttribute), true).FirstOrDefault() as ProcedureAttribute;
            var timeout = type.GetCustomAttributes(typeof(TimeoutAttribute), true).FirstOrDefault() as TimeoutAttribute;

            var sqlCommad = new SqlCommand(procedure.ProcedureName, _sqlConnection, _sqlTransaction) { CommandType = CommandType.StoredProcedure };

            foreach (var inf in type.GetProperties().ToList())
            {
                if (inf.GetCustomAttributes(typeof(IgnoreAttribute), false).Count() == 0)
                {
                    var attr = inf.GetCustomAttributes(typeof(ParemeterAttribute), false).FirstOrDefault() as ParemeterAttribute;
                    if (attr != null)
                    {
                        sqlCommad.AddParameter(string.IsNullOrEmpty(attr.ParameterName) ? inf.Name : attr.ParameterName, attr.ParameterType, GetInputValue(inf, storedProcedure), attr.ParameterSize, attr.ParameterDirection);
                    }
                }
            }

            if (timeout != null)
            {
                sqlCommad.CommandTimeout = timeout.TimeOut;
            }

            if (_onDebug)
            {
                LogService.PrintQuery(sqlCommad.PrintSql());
            }

            return sqlCommad;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ExecutedInDebug()
        {
            return _onDebug;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteQuery(string query)
        {
            SqlCommand myCommand = new SqlCommand(query, _sqlConnection, _sqlTransaction) { CommandType = CommandType.Text };

            return myCommand.ExecuteSql();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void RestoreAuditLog<TEntity>(Guid id) where TEntity : EntityBase
        {
            AuditLogs audit = this.Query<AuditLogs>()
                                .Select()
                                .Where(audit => audit.Id == id)
                                .FirstOrDefault();

            if (audit == null)
                throw new KeyNotFoundException($"Não foi encontrada nenhuma auditoria com o id {id}");

            AuditEntry entry = new AuditEntry(audit);

            string query =
                $"UPDATE {entry.TableName} SET {this.RestoreAuditLog_GetListOfValues<TEntity>(entry.OldValues, ", ")} WHERE {this.RestoreAuditLog_GetListOfValues<TEntity>(entry.KeyValues, " AND ")};";

            this.ExecuteQuery(query);
        }

        private string RestoreAuditLog_GetListOfValues<TEntity>(Dictionary<string, object> keyValuePairs, string separator) where TEntity : EntityBase
        {
            List<string> setClausule = new List<string>();

            foreach (KeyValuePair<string, object> kvPair in keyValuePairs)
            {
                setClausule.Add($"{kvPair.Key}{RestoreAuditLog_GetValueCondition<TEntity>(kvPair)}");
            }

            return string.Join(separator, setClausule);
        }

        private string RestoreAuditLog_GetValueCondition<TEntity>(KeyValuePair<string, object> kvp, bool isWhereClausule = false) where TEntity : EntityBase
        {
            EntityAttributesModel<TEntity> entity = new EntityAttributesModelFactory<TEntity>().InterpretEntity(Activator.CreateInstance<TEntity>(), true);

            var propertyInfo = entity.Props.Where(p => p.Name == kvp.Key).FirstOrDefault();
            if (propertyInfo == null)
                throw new KeyNotFoundException($"Não foi encontrada a propriedade '{kvp.Key}' no tipo '{typeof(TEntity)}'");

            object value = ChangeType(kvp.Value?.ToString(), propertyInfo.Type);

            string valueInterpreted = new InterpretService<TEntity>().TreatValue(value, true).ToString();

            return isWhereClausule && valueInterpreted.Equals("NULL") ? "IS NULL" : $" = {valueInterpreted}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversion"></param>
        /// <returns></returns>
        private static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (string.IsNullOrEmpty(value?.ToString()))
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inf"></param>
        /// <returns></returns>
        protected virtual object GetInputValue(PropertyInfo inf, StoredProcedureBase storedProcedure)
        {
            return inf.GetValue(storedProcedure);
        }
    }
}
