﻿using DB.Query.Core.Models;
using System.Data;
using System.Threading.Tasks;
using System;
using DB.Query.InterpretCode.Transaction;
using DB.Query.InterpretCode.Transaction.Functions;

namespace DB.Query.Core
{
    public class DBQueryTransaction : DBFunctions
    {
        /// <summary>
        ///  Variável geral no escopo da classe. Deve ser manipulada com cuidado foram do contexto de uma transaction 
        ///  ela não funcionará deverá ser acionada em algum onTransaction
        /// </summary>
        protected DBTransaction _transaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        public DBQueryTransaction(DBTransaction transaction)
        {
        }

        public DBQueryTransaction()
        {
        }

        /// <summary>
        /// 
        /// <param name="func"></param>
        protected void OnTransaction(Action<DBTransaction> func)
        {
            bool alreadyOpen = false;
            var _dbTransaction = InstanceDbTransaction(out alreadyOpen);
            try
            {
                _dbTransaction.OpenTransaction(DbQueryConfiguration.SqlConnection);
                func(_dbTransaction);

                if (!_dbTransaction.HasCommited() && !alreadyOpen)
                {
                    _dbTransaction.Commit();
                }
            }
            catch (Exception)
            {
                if (!_dbTransaction.HasReversed())
                {
                    _dbTransaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (!alreadyOpen)
                {
                    _transaction = null;
                    if (_dbTransaction.GetConnection() != null)
                        _dbTransaction.GetConnection().Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected T OnTransaction<T>(Func<DBTransaction, T> func)
        {
            T retorno;
            if (typeof(T) == typeof(string))
            {
                retorno = (dynamic)string.Empty;
            }
            else
            {
                retorno = Activator.CreateInstance<T>();
            }
            bool alreadyOpen = false;
            var _dbTransaction = InstanceDbTransaction(out alreadyOpen);
            try
            {
                _dbTransaction.OpenTransaction(DbQueryConfiguration.SqlConnection);

                retorno = func(_dbTransaction);


                if (!_dbTransaction.HasCommited() && !alreadyOpen)
                {
                    _dbTransaction.Commit();
                }
            }
            catch (Exception)
            {
                if (!_dbTransaction.HasReversed())
                {
                    _dbTransaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (!alreadyOpen)
                {
                    _transaction = null;
                    if (_dbTransaction.GetConnection() != null)
                        _dbTransaction.GetConnection().Close();
                }
            }
            return retorno;
        }

        /// <summary>
        /// 
        /// </summary
        /// <param name="func"></param>
        protected Task OnTransactionAsync(Action<DBTransaction> func)
        {
            bool alreadyOpen = false;
            var _dbTransaction = InstanceDbTransaction(out alreadyOpen);
            try
            {
                _dbTransaction.OpenTransaction(DbQueryConfiguration.SqlConnection);
                func(_dbTransaction);

                if (!_dbTransaction.HasCommited() && !alreadyOpen)
                {
                    _dbTransaction.Commit();
                }
            }
            catch (Exception)
            {
                if (!_dbTransaction.HasReversed())
                {
                    _dbTransaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (!alreadyOpen)
                {
                    _transaction = null;
                    if (_dbTransaction.GetConnection() != null)
                        _dbTransaction.GetConnection().Close();
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Realizo o set da transação corrente na classe base
        /// </summary>
        protected void BindTransaction(DBTransaction transaction)
        {
            _transaction = transaction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <returns></returns>
        protected T InstanceService<T>(DBTransaction transaction) where T : DBQueryTransaction
        {
            var inst = Activator.CreateInstance<T>();
            inst.BindTransaction(transaction);
            return inst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DBTransaction InstanceDbTransaction(out bool alreadyOpen)
        {
            if (_transaction != null && _transaction.GetConnection() != null && _transaction.GetTransaction() != null
                && _transaction.GetConnection().State != ConnectionState.Closed)
            {
                alreadyOpen = true;
                return _transaction;
            }
            else
            {
                alreadyOpen = false;
                return Activator.CreateInstance<DBTransaction>();
            }
        }
    }

    public class DBQuery
    {
        /// <summary>
        /// 
        /// <param name="func"></param>
        public static void OnTransaction(Action<DBTransaction> func)
        {
            bool alreadyOpen = false;
            var _dbTransaction =  Activator.CreateInstance<DBTransaction>();
            try
            {
                _dbTransaction.OpenTransaction(DbQueryConfiguration.SqlConnection);
                func(_dbTransaction);

                if (!_dbTransaction.HasCommited())
                {
                    _dbTransaction.Commit();
                }
            }
            catch (Exception)
            {
                if (!_dbTransaction.HasReversed())
                {
                    _dbTransaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (!alreadyOpen)
                {
                    if (_dbTransaction.GetConnection() != null)
                        _dbTransaction.GetConnection().Close();
                }
            }
        }
    }
}
