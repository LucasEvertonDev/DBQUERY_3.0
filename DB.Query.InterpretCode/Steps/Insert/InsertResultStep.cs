using DB.Query.InterpretCode.Steps.Core;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Insert
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InsertResultStep<TEntity> : ResultStep<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseRetorno"></param>
        public InsertResultStep(dynamic databaseRetorno) : base()
        {
            _databaseRetorno = databaseRetorno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public dynamic GetIdentityId()
        {
            if (_databaseRetorno != null)
            {
                if (_databaseRetorno.GetType() == typeof(int))
                {
                    return (int)_databaseRetorno;
                }
                else
                {
                    return _databaseRetorno;
                }
            }
            return 0;
        }
    }
}
