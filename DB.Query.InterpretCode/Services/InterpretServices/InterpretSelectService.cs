using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using System.Text;

namespace DB.Query.InterpretCode.Services.InterpretServices
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InterpretSelectService<TEntity> : InterpretService<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        public InterpretSelectService() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string RunInterpret()
        {
            return GenerateSelectScript();
        }
        protected string GenerateSelectScript()
        {
            var queryBuilder = new StringBuilder(); // Usando StringBuilder para melhor performance.
            bool adicionadoOrdenacao = false; // Marca se a ordenação já foi adicionada à consulta.

            foreach (var step in _levelModels) // Itera sobre cada passo do modelo.
            {
                switch (step.StepType)
                {
                    case StepType.SELECT:
                    case StepType.CUSTOM_SELECT:
                        // Adiciona a cláusula SELECT à consulta.
                        queryBuilder.AppendFormat(
                            DBKeysConstants.SELECT,
                            DBKeysConstants.ALL_COLUMNS,
                            GetFullName(typeof(TEntity)),
                            string.IsNullOrEmpty(_alias) ? string.Empty : DBKeysConstants.AS_WITH_SPACE + _alias + " "
                        );

                        // Se for uma seleção personalizada, ajusta os campos selecionados.
                        if (step.StepType == StepType.CUSTOM_SELECT)
                        {
                            var props = GetPropertiesExpression(step.StepExpression, useAlias: true, fromSelect: true); // Obtém as propriedades a serem selecionadas.
                            queryBuilder.Replace(DBKeysConstants.SELECT_ALL, DBKeysConstants.SELECT_KEY + " " + string.Join(", ", props)); // Substitui "SELECT *" pelos campos específicos.
                        }
                        break;

                    case StepType.DISTINCT:
                        ReplaceFirst(queryBuilder, DBKeysConstants.SELECT_KEY, DBKeysConstants.SELECT_DISTINCT); // Substitui SELECT por SELECT DISTINCT.
                        break;

                    case StepType.TOP:
                        // Ajusta a consulta para incluir TOP, se necessário.
                        if (queryBuilder.ToString().Contains(DBKeysConstants.SELECT_DISTINCT))
                        {
                            ReplaceFirst(queryBuilder, DBKeysConstants.SELECT_DISTINCT, string.Format(DBKeysConstants.SELECT_DISTINCT_TOP, step.StepValue));
                        }
                        else
                        {
                            queryBuilder.Replace(DBKeysConstants.SELECT_KEY, string.Format(DBKeysConstants.SELECT_TOP, step.StepValue));
                        }
                        break;

                    case StepType.WHERE:
                        AddWhere(step.StepExpression, queryBuilder); // Adiciona a cláusula WHERE à consulta.
                        break;

                    case StepType.JOIN:
                        AddJoin(step.StepExpression, queryBuilder, DBKeysConstants.INNER_JOIN); // Adiciona um INNER JOIN.
                        break;

                    case StepType.JOIN_EMPRESA_FILIAL:
                        AddJoinEmpresaFilial(step.StepExpression, queryBuilder, DBKeysConstants.INNER_JOIN); // Adiciona um INNER JOIN específico.
                        break;

                    case StepType.LEFT_JOIN_EMPRESA_FILIAL:
                        AddJoinEmpresaFilial(step.StepExpression, queryBuilder, DBKeysConstants.LEFT_JOIN); // Adiciona um LEFT JOIN específico.
                        break;

                    case StepType.LEFT_JOIN:
                        AddJoin(step.StepExpression, queryBuilder, DBKeysConstants.LEFT_JOIN); // Adiciona um LEFT JOIN.
                        break;

                    case StepType.ORDER_BY_ASC:
                        AddOrderBy(DBKeysConstants.ASC, step.StepExpression, queryBuilder, adicionadoOrdenacao); // Adiciona ordenação ascendente.
                        adicionadoOrdenacao = true; // Marca que a ordenação foi adicionada.
                        break;

                    case StepType.ORDER_BY_DESC:
                        AddOrderBy(DBKeysConstants.DESC, step.StepExpression, queryBuilder, adicionadoOrdenacao); // Adiciona ordenação descendente.
                        adicionadoOrdenacao = true; // Marca que a ordenação foi adicionada.
                        break;

                    case StepType.GROUP_BY:
                        AddGroupBy(step.StepExpression, queryBuilder); // Adiciona a cláusula GROUP BY.
                        break;

                    case StepType.PAGINATION:
                        queryBuilder.AppendFormat(DBKeysConstants.OFFSET, step.PageSize * (step.PageNumber - 1), step.PageSize); // Adiciona paginação à consulta.
                        break;
                }
            }

            return queryBuilder.ToString(); // Retorna a consulta gerada.
        }

        // Método auxiliar para substituir a primeira ocorrência
        private void ReplaceFirst(StringBuilder sb, string oldValue, string newValue)
        {
            int pos = sb.ToString().IndexOf(oldValue);
            if (pos < 0) return; // Se não encontrar, não faz nada.
            sb.Remove(pos, oldValue.Length);
            sb.Insert(pos, newValue);
        }
    }
}
