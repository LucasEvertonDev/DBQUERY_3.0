using System.Linq.Expressions;
using System;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.Core.Models;

namespace DB.Query.InterpretCode.Factorys
{
    /// <summary>
    /// 
    /// </summary>
    public class DBQueryLevelModelFactory
    {
        /// <summary>
        /// Prepara a criação de uma etapa de UseAlias. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a USE_ALIAS</returns>
        public DBQueryStepModel PrepareAliasStep(string alias)
        {
            return new DBQueryStepModel()
            {
                StepType = StepType.USE_ALIAS,
                StepValue = alias,
                Documentation = "Responsável por indicar se os apelidos usados nas expressions serão denominados como chaves apelidos 'as'."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de UseAlias. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a USE_ALIAS</returns>
        public DBQueryStepModel PrepareUseAuditLog(long codigoUsuario, Action<Guid> action = null)
        {
            return new DBQueryStepModel()
            {
                StepType = StepType.USE_AUDIT_LOG,
                StepValue = new AuditLogStepValue
                {
                    CodigoUsuario = codigoUsuario,
                    Action = action,
                },
                Documentation = "Responsável por indicar se será criado log de auditoria ou não."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de Insert. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretInsertService{TEntity}.GenerateInsertScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a INSERT</returns>
        public DBQueryStepModel PrepareInsertStep(EntityBase domain)
        {
            return new DBQueryStepModel()
            {
                StepType = StepType.INSERT,
                StepValue = domain,
                Documentation = "Responsável por chamar a etapa de insert. A mesma recebe um objeto preenchido do tipo TEntity para a persistência."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de DELETE. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretDeleteService{TEntity}.GenerateDeleteScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a DELETE</returns>
        public DBQueryStepModel PrepareDeleteStep()
        {
            return new DBQueryStepModel
            {
                StepType = StepType.DELETE,
                Documentation = "Reponsável por chamar a etapa de delete. O controle de propriedades a serem apagadas será realizado na etapa WHERE."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de SELECT. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretSelectService{TEntity}.GenerateSelectScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a SELECT</returns>
        public DBQueryStepModel PrepareSimpleSelectStep()
        {
            return new DBQueryStepModel
            {
                StepType = StepType.SELECT,
                Documentation = "Reponsável por chamar a etapa select de uma consulta SIMPLES. Retornando apenas os dados da entidade passada como tipo na propriedade Query<TEntity>."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de UPDATE. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretUpdateService{TEntity}.GenerateUpdateScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a SELECT</returns>
        public DBQueryStepModel PrepareUpdateStep(EntityBase domain)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.UPDATE,
                StepValue = domain,
                Documentation = "Reponsável por chamar a etapa update. A mesma recebe um objeto preenchido do tipo TEntity para a atualização."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de INSERT_OR_UPDATE. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretUpdateService{TEntity}.GenerateInsertOrUpdateScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a INSERT_OR_UPDATE</returns>
        public DBQueryStepModel PrepareInsertOrUpdateStep(EntityBase domain)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.INSERT_OR_UPDATE,
                StepValue = domain,
                Documentation = "Reponsável por chamar a etapa de insert ou update. A etapa WHERE é de suma importância para essa ação." +
                            " Pois a mesma será a chave para a verificação de existência do objeto ou até mesmo na atualização caso ele já exista."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de DELETE_AND_INSERT. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretDeleteService{TEntity}.GenerateDeleteAndInsertScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a DELETE_AND_INSERT</returns>
        public DBQueryStepModel PrepareDeleteAndInsertStep(EntityBase domain)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.DELETE_AND_INSERT,
                StepValue = domain,
                Documentation = "Reponsável por chamar a etapa delete e insert. Ou seja limpa os registros condizentes com a condição e insere na sequência "
            };
        }


        /// <summary>
        /// Prepara a criação de uma etapa de INSERT_NOT_EXISTS. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretInsertService{TEntity}.GenerateInsertIfNotExistsScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a INSERT_NOT_EXISTS</returns>
        public DBQueryStepModel PrepareInsertIfNotExistsStep(EntityBase domain)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.INSERT_NOT_EXISTS,
                StepValue = domain,
                Documentation = "Responsável por chamar a etapa de insert. A mesma recebe um objeto preenchido do tipo TEntity para a persistência, onde será válidada se a tabela não possui nenhum registro igual, com apenas a chave unica diferente. Sendo esse atributo ignorado na validação, representado com a anotação Identity"
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de EXECUTE. 
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a EXECUTE</returns>
        public DBQueryStepModel PrepareExecuteStep()
        {
            return new DBQueryStepModel
            {
                StepType = StepType.EXECUTE,
                Documentation = "Comando usado para iniciar processe de leitura e execução da query"
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de CUSTOM_SELECT. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretSelectService{TEntity}.GenerateSelectScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a CUSTOM_SELECT</returns>
        public DBQueryStepModel PrepareSelectStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.CUSTOM_SELECT,
                StepExpression = expression,
                Documentation = "Reponsável por chamar a etapa select de uma consulta customizada."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de WHERE. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretSelectService{TEntity}.GenerateSelectScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a WHERE</returns>
        public DBQueryStepModel PrepareWhereStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.WHERE,
                StepExpression = expression,
                Documentation = "Responsável pelas condições da operação. A expressão deve ter um resultado booleano," +
                " porém é de suma importância na comparação de propriedade a mesma possuir dois passos," +
                " mesmo em casos redundantes que são os de propriedades booleanas. Ou seja utilize Entidade.Propriedade == true"
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de Update Set. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretUpdateService{TEntity}.GenerateUpdateScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a Update Set</returns>
        public DBQueryStepModel PrepareUpdateSetStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.UPDATE_SET,
                StepExpression = expression,
                Documentation = "Indica quais são as colunas que serão atualziadas no update"
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de DISTINCT. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretSelectService{TEntity}.GenerateSelectScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a DISTINCT</returns>
        public DBQueryStepModel PrepareDistinctStep()
        {
            return new DBQueryStepModel
            {
                StepType = StepType.DISTINCT,
                Documentation = "Responsável por adicionar a instrução distinct."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de TOP. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretSelectService{TEntity}.GenerateSelectScript">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a TOP</returns>
        public DBQueryStepModel PrepareTopStep(int top)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.TOP,
                StepValue = top,
                Documentation = "Responsável por adicionar a instrução top."
            };
        }


        /// <summary>
        /// Prepara a criação de uma etapa de INNER JOIN. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}.AddJoin(Expression, string)">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a JOIN</returns>
        public DBQueryStepModel PrepareJoinStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.JOIN,
                StepExpression = expression,
                Documentation = "Responsável por adicionar a instrução de INNER JOIN e suas condições('ON')."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de INNER JOIN. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}.AddJoin(Expression, string)">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a JOIN</returns>
        public DBQueryStepModel PrepareJoinEmpresaFilialStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.JOIN_EMPRESA_FILIAL,
                StepExpression = expression,
                Documentation = "Responsável por adicionar a instrução de INNER JOIN e suas condições('ON') e incluir clausulás validando empresa e filial."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de INNER JOIN. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}.AddJoin(Expression, string)">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a JOIN</returns>
        public DBQueryStepModel PrepareLeftJoinEmpresaFilialStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.LEFT_JOIN_EMPRESA_FILIAL,
                StepExpression = expression,
                Documentation = "Responsável por adicionar a instrução de INNER JOIN e suas condições('ON') e incluir clausulás validando empresa e filial."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de LEFT JOIN. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}.AddJoin(Expression, string)">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a LEFT_JOIN</returns>
        public DBQueryStepModel PrepareLeftJoinStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.LEFT_JOIN,
                StepExpression = expression,
                Documentation = "Responsável por adicionar a instrução de INNER JOIN e suas condições('ON')."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de ORDER BY. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}.AddOrderBy(string, Expression, string)">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a ORDER_BY_ASC</returns>
        public DBQueryStepModel PrepareOrderByAscStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.ORDER_BY_ASC,
                StepExpression = expression,
                Documentation = "Responsável por adicionar a instrução de ORDER BY ASC a consulta."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de PAGINATION. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretSelectService{TEntity}.GenerateSelectScript()">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a PAGINATION</returns>
        public DBQueryStepModel PreparePaginationStep(int pageSize, int pageNumber)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.PAGINATION,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Documentation = "Responsável por adicionar a instrução de paginação a consulta."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de ORDER BY DESC. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}.AddOrderBy(string, Expression, string)">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a ORDER_BY_DESC</returns>
        public DBQueryStepModel PrepareOrderByDescStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.ORDER_BY_DESC,
                StepExpression = expression,
                Documentation = "Responsável por adicionar a instrução de ORDER BY DESC a consulta."
            };
        }

        /// <summary>
        /// Prepara a criação de uma etapa de GROUP_BY. 
        /// Que posteriormente será traduzida para SQL <see cref="InterpretService{SignQueryBase}.AddGroupBy(Expression, string)">aqui.</see>
        /// </summary>
        /// <returns>Retorna uma etapa corresponte a GROUP_BY</returns>
        public DBQueryStepModel PrepareGroupByStep(Expression expression)
        {
            return new DBQueryStepModel
            {
                StepType = StepType.GROUP_BY,
                StepExpression = expression,
                Documentation = "Responsável por adicionar a instrução de GROUP BY a consulta."
            };
        }
    }
}

