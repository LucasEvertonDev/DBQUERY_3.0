using DB.Query.InterpretCode.Steps.Core;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Delete
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DeleteResultStep<TEntity> : ResultStep<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseRetorno"></param>
        public DeleteResultStep(dynamic databaseRetorno) : base()
        {
            _databaseRetorno = databaseRetorno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetNumeroRegistrosAfetados()
        {
            if (_databaseRetorno != null)
            {
                if (_databaseRetorno.GetType() == typeof(int))
                {
                    return (int)_databaseRetorno;
                }
            }
            return 0;
        }
    }
}
