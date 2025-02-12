using DB.Query.Core.Entities;
using DB.Query.Core.Functions;
using DB.Query.Core.Models;
using DB.Query.InterpretCode.Factorys;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.InterpretCode.Transaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB.Query.InterpretCode.Steps.Core
{
    public class DBQuery<TEntity> : DBFunctions, IDBQuery where TEntity : EntityBase
    {
        protected List<DBQueryStepModel> _steps { get; set; }
        protected DBTransaction _transaction { get; set; }
        protected DBQueryLevelModelFactory _levelFactory { get; set; }

        public DBQuery()
        {
            _transaction = new DBTransaction();
            _steps = new List<DBQueryStepModel>();
            _levelFactory = new DBQueryLevelModelFactory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected TNextStep InstanceNextLevel<TNextStep>(DBQueryStepModel levelModel) where TNextStep : DBQuery<TEntity>
        {
            var dbQuery = Activator.CreateInstance<TNextStep>();
            dbQuery._steps = _steps;
            dbQuery.IncludeStep(levelModel);
            dbQuery._transaction = _transaction;
            dbQuery.StartLevel();
            return dbQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void StartLevel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dBQueryLevels"></param>
        protected void IncludeStep(DBQueryStepModel dBQueryLevels)
        {
            _steps.Add(dBQueryLevels);
        }

        /// <summary>
        /// /
        /// </summary>
        protected void VerifyChangeDataBase()
        {
            if (_transaction != null)
            {
                var databaseName = new InterpretService<TEntity>().GetDatabaseName(typeof(TEntity));
                if (!databaseName.Equals(_transaction.GetConnection().Database))
                {
                    _transaction.ChangeDatabase(databaseName);
                }
            }
        }

        /// <summary>
        /// /
        /// </summary>
        protected async Task VerifyChangeDataBaseAsync()
        {
            if (_transaction != null)
            {
                var databaseName = new InterpretService<TEntity>().GetDatabaseName(typeof(TEntity));
                if (!databaseName.Equals(_transaction.GetConnection().Database))
                {
                    await _transaction.ChangeDatabaseAsync(databaseName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ClearOldConfigurations()
        {
            _transaction = null;
            _steps.Clear();
        }
    }
}
