using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;

namespace DB.Query.InterpretCode.Services.InterpretServices
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InterpretSelectServiceOldVersion<TEntity> : InterpretServiceOldVersion<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        public InterpretSelectServiceOldVersion() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string RunInterpret()
        {
            return GenerateSelectScript();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GenerateSelectScript()
        {
            var query = string.Empty;
            bool adicionadoOrdenacao = false;
            foreach (var step in _levelModels)
            {
                if (step.StepType == StepType.SELECT || step.StepType == StepType.CUSTOM_SELECT)
                {
                    query += string.Format(
                        DBKeysConstants.SELECT,
                        DBKeysConstants.ALL_COLUMNS,
                        GetFullName(typeof(TEntity)),
                        string.IsNullOrEmpty(_alias) ? string.Empty : DBKeysConstants.AS_WITH_SPACE + _alias + " ");

                    if (step.StepType == StepType.CUSTOM_SELECT)
                    {
                        var props = GetPropertiesExpression(step.StepExpression, useAlias: true, fromSelect: true);
                        query = query.Replace(DBKeysConstants.SELECT_ALL, DBKeysConstants.SELECT_KEY + " " + string.Join(", ", props));
                    }
                }
                else if (step.StepType == StepType.DISTINCT)
                {
                    query = ReplaceFirst(query, DBKeysConstants.SELECT_KEY, DBKeysConstants.SELECT_DISTINCT);
                }
                else if (step.StepType == StepType.TOP)
                {
                    if (query.Contains(DBKeysConstants.SELECT_DISTINCT))
                    {
                        query = ReplaceFirst(query, DBKeysConstants.SELECT_DISTINCT, string.Format(DBKeysConstants.SELECT_DISTINCT_TOP, step.StepValue));
                    }
                    else
                    {
                        query = query.Replace(DBKeysConstants.SELECT_KEY, string.Format(DBKeysConstants.SELECT_TOP, step.StepValue));
                    }
                }
                else if (step.StepType == StepType.WHERE)
                {
                    query += AddWhere(step.StepExpression);
                }
                else if (step.StepType == StepType.JOIN)
                {
                    query += AddJoin(step.StepExpression, DBKeysConstants.INNER_JOIN);
                }
                else if (step.StepType == StepType.JOIN_EMPRESA_FILIAL)
                {
                    query += AddJoinEmpresaFilial(step.StepExpression, DBKeysConstants.INNER_JOIN);
                }
                else if (step.StepType == StepType.LEFT_JOIN_EMPRESA_FILIAL)
                {
                    query += AddJoinEmpresaFilial(step.StepExpression, DBKeysConstants.LEFT_JOIN);
                }
                else if (step.StepType == StepType.LEFT_JOIN)
                {
                    query += AddJoin(step.StepExpression, DBKeysConstants.LEFT_JOIN);
                }
                else if (step.StepType == StepType.ORDER_BY_ASC)
                {
                    query = AddOrderBy(DBKeysConstants.ASC, step.StepExpression, query, adicionadoOrdenacao);
                    adicionadoOrdenacao = true;
                }
                else if (step.StepType == StepType.ORDER_BY_DESC)
                {
                    query = AddOrderBy(DBKeysConstants.DESC, step.StepExpression, query, adicionadoOrdenacao);
                    adicionadoOrdenacao = true;
                }
                else if (step.StepType == StepType.GROUP_BY)
                {
                    query = AddGroupBy(step.StepExpression, query);
                }
                else if (step.StepType == StepType.PAGINATION)
                {
                    query += string.Format(DBKeysConstants.OFFSET, step.PageSize * (step.PageNumber - 1), step.PageSize);
                }
            }
            return query;
        }

        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
