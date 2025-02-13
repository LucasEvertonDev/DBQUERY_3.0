using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.InterpretCode.Services.Others;
using DB.Query.InterpretCode.Transaction;
using DB.Query.Utils.Extensions;
using SIGN.Query.Models.PersistenceContext.Entities.SignCi;
using System;
using System.Linq;
using System.Text;

namespace DB.Query.InterpretCode.Services.InterpretServices
{
    /// <summary>
    /// Serviço para interpretar operações de exclusão de entidades.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade que será manipulada.</typeparam>
    public class InterpretDeleteService<TEntity> : InterpretService<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// Transação associada ao serviço.
        /// </summary>
        private DBTransaction _transaction;

        /// <summary>
        /// Construtor para inicializar o serviço de exclusão.
        /// </summary>
        /// <param name="DBTransaction">Transação a ser utilizada.</param>
        public InterpretDeleteService(DBTransaction DBTransaction)
        {
            _transaction = DBTransaction;
        }

        /// <summary>
        /// Executa a interpretação da operação de exclusão.
        /// </summary>
        /// <returns>Consulta SQL gerada para a operação.</returns>
        protected override string RunInterpret()
        {
            var step = _levelModels.First(); // Obtém o primeiro passo do modelo.
            _domain = step.StepValue; // Armazena o domínio atual.

            switch (step.StepType) // Utiliza switch para determinar o tipo de operação.
            {
                case StepType.DELETE:
                    return GenerateDeleteScript(); // Gera o script de exclusão.

                case StepType.DELETE_AND_INSERT:
                    IsValid(_domain); // Valida o domínio antes da operação.
                    return GenerateDeleteAndInsertScript(); // Gera o script de exclusão e inserção.

                default:
                    return base.RunInterpret(); // Chama a implementação base para outras operações.
            }
        }

        /// <summary>
        /// Gera o script SQL para a operação de exclusão.
        /// </summary>
        /// <returns>Consulta SQL de exclusão.</returns>
        protected string GenerateDeleteScript()
        {
            var queryBuilder = new StringBuilder(); // Usando StringBuilder para melhor performance

            // Inicializa a consulta de exclusão
            queryBuilder.AppendFormat(DBKeysConstants.DELETE, GetFullName(typeof(TEntity)), string.Empty);

            // Obtém a cláusula WHERE
            var where = _levelModels.FirstOrDefault(step => step.StepType == StepType.WHERE);
            if (where != null)
            {
                AddWhere(where.StepExpression, queryBuilder); // Adiciona a cláusula WHERE à consulta
            }

            // Verifica se o log de auditoria deve ser utilizado
            var useAuditLog = _levelModels.FirstOrDefault(step => step.StepType == StepType.USE_AUDIT_LOG);

            if (useAuditLog != null && typeof(AuditLogs) != typeof(TEntity) && _transaction.GetTransaction() != null)
            {
                if (where == null)
                {
                    throw new Exception("Só é possível registrar log de auditoria para updates com a cláusula WHERE");
                }

                var sql = _transaction.Query<TEntity>()
                    .Select()
                    .GetQuery() + AddWhere(where.StepExpression);

                var oldEntities = _transaction.ExecuteQuery(sql).OfType<TEntity>(); // Obtém as entidades antigas

                var auditorias = AuditService.GenerateDeleteAudit<TEntity>(oldEntities, useAuditLog.StepValue?.CodigoUsuario.ToString()); // Gera os logs de auditoria

                foreach (var auditoria in auditorias)
                {
                    _transaction.Query<AuditLogs>().Insert(auditoria).Execute(); // Insere o log de auditoria

                    useAuditLog.StepValue?.Action?.Invoke(auditoria.Id); // Executa a ação associada ao log, se houver
                }
            }

            return queryBuilder.ToString(); // Retorna a consulta gerada
        }

        /// <summary>
        /// Gera o script SQL para uma operação de exclusão seguida de inserção.
        /// </summary>
        /// <returns>Consulta SQL de exclusão e inserção.</returns>
        protected string GenerateDeleteAndInsertScript()
        {
            var insertQuery = new InterpretInsertService<TEntity>(_transaction, useAuditLog: true).StartToInterpret(_levelModels); // Gera a consulta de inserção.
            return string.Concat(GenerateDeleteScript(), " ", insertQuery); // Retorna a consulta de exclusão seguida da de inserção.
        }
    }
}
