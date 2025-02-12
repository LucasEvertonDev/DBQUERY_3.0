using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ResultStep<TEntity> : DBQuery<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected dynamic _databaseRetorno { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseRetorno"></param>
        public ResultStep(dynamic databaseRetorno)
        {
            _databaseRetorno = databaseRetorno;
        }

        /// <summary>
        /// 
        /// </summary>
        public ResultStep()
        {
        }
    }
}
