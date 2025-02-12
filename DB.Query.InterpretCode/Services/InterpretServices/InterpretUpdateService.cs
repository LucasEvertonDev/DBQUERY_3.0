using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.InterpretCode.Factorys;
using DB.Query.InterpretCode.Services.Others;
using DB.Query.InterpretCode.Transaction;
using DB.Query.Utils.Extensions;
using SIGN.Query.Models.PersistenceContext.Entities.SignCi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB.Query.InterpretCode.Services.InterpretServices
{
    /// <summary>
    /// Serviço para interpretar operações de atualização de entidades.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade que será manipulada.</typeparam>
    public class InterpretUpdateService<TEntity> : InterpretService<TEntity> where TEntity : EntityBase
    {
        private DBTransaction _transaction;

        /// <summary>
        /// Controla se o log de auditoria deve ser utilizado.
        /// </summary>
        private bool _useAuditLog;

        /// <summary>
        /// Construtor para inicializar o serviço de atualização.
        /// </summary>
        /// <param name="DBTransaction">Transação a ser utilizada.</param>
        /// <param name="useAuditLog">Indica se o log de auditoria deve ser utilizado.</param>
        public InterpretUpdateService(DBTransaction DBTransaction, bool useAuditLog = true)
        {
            _transaction = DBTransaction;
            _useAuditLog = useAuditLog;
        }

        /// <summary>
        /// Executa a interpretação da operação de atualização.
        /// </summary>
        /// <returns>Consulta SQL gerada para a operação.</returns>
        protected override string RunInterpret()
        {
            var step = _levelModels.First(); // Obtém o primeiro passo do modelo.
            _domain = step.StepValue; // Armazena o domínio atual.

            return step.StepType switch // Utiliza switch para determinar o tipo de operação.
            {
                StepType.UPDATE => GenerateUpdateScript(),
                StepType.INSERT_OR_UPDATE => GenerateInsertOrUpdateScript(),
                _ => base.RunInterpret() // Chama a implementação base para outras operações.
            };
        }

        protected string GenerateUpdateScript(bool useAuditLogs = true)
        {
            var columnsSet = new List<string>();
            var queryBuilder = new StringBuilder(); // Usando StringBuilder para melhor performance

            var stepSet = _levelModels.FirstOrDefault(step => step.StepType == StepType.UPDATE_SET);
            _entityContext = new EntityAttributesModelFactory<TEntity>().InterpretEntity(_domain, true, _entityContext);

            var useAuditLog = _levelModels.FirstOrDefault(step => step.StepType == StepType.USE_AUDIT_LOG);

            if (stepSet != null) // Se há um conjunto de atualizações definido.
            {
                var sets = GetPropertiesExpression(stepSet.StepExpression);
                columnsSet = sets.Select(s => s.Split('.')[1]).ToList();

                var objectClausules = _entityContext.Props
                    .Where(a => !a.Identity && columnsSet.Contains(a.Name))
                    .Select(a => $"{a.Name} {DBKeysConstants.EQUALS_WITH_SPACE} {TreatValue(a.Valor, true)}");

                queryBuilder.AppendFormat(DBKeysConstants.UPDATE, _entityContext.FullName, string.Join(", ", objectClausules), string.Empty);
            }
            else // Se não houver um conjunto de atualizações, usa todas as propriedades não identificadoras.
            {
                IsValid(_domain);
                var objectClausules = _entityContext.Props
                    .Where(a => !a.Identity)
                    .Select(a => $"{a.Name} {DBKeysConstants.EQUALS_WITH_SPACE} {TreatValue(a.Valor, true)}");

                queryBuilder.AppendFormat(DBKeysConstants.UPDATE, _entityContext.FullName, string.Join(", ", objectClausules), string.Empty);
            }

            // Adiciona a cláusula WHERE, se existir.
            var where = _levelModels.FirstOrDefault(step => step.StepType == StepType.WHERE);
            if (where != null)
            {
                AddWhere(where.StepExpression, queryBuilder); // Usa Append para concatenar
            }

            // Processa o log de auditoria, se necessário.
            if (useAuditLog != null && useAuditLogs && typeof(AuditLogs) != typeof(TEntity) && _transaction.GetTransaction() != null)
            {
                if (where == null)
                {
                    throw new Exception("Só é possível registrar log de auditoria para updates com a cláusula WHERE");
                }

                var sql = _transaction.GetRepository<TEntity>()
                    .Select()
                    .GetQuery() + AddWhere(where.StepExpression);

                var oldEntities = _transaction.ExecuteQuery(sql).OfType<TEntity>();

                var auditorias = AuditService.GenerateUpdateAudit<TEntity>(_entityContext, oldEntities, columnsSet, useAuditLog.StepValue?.CodigoUsuario.ToString());

                // Insere os logs de auditoria.
                foreach (var auditoria in auditorias)
                {
                    _transaction.GetRepository<AuditLogs>().Insert(auditoria).Execute();

                    useAuditLog.StepValue?.Action?.Invoke(auditoria.Id);
                }
            }

            return queryBuilder.ToString(); // Retorna a consulta gerada.
        }

        /// <summary>
        /// Gera o script SQL para uma operação de inserção ou atualização.
        /// </summary>
        /// <returns>Consulta SQL de inserção ou atualização.</returns>
        protected string GenerateInsertOrUpdateScript()
        {
            bool isEdit = false;
            _entityContext = new EntityAttributesModelFactory<TEntity>().InterpretEntity(_domain, true, _entityContext);

            // Gera a consulta de inserção.
            var insertQuery = new InterpretInsertService<TEntity>(_transaction, useAuditLog: false).StartToInterpret(_levelModels);

            var useAuditLog = _levelModels.FirstOrDefault(step => step.StepType == StepType.USE_AUDIT_LOG);

            if (useAuditLog != null && _useAuditLog && _transaction.GetTransaction() != null)
            {
                var whereStep = _levelModels.First(step => step.StepType == StepType.WHERE).StepExpression;
                var sql = _transaction.GetRepository<TEntity>()
                    .Select()
                    .GetQuery() + AddWhere(whereStep);

                var dbEntities = _transaction.ExecuteQuery(sql).OfType<TEntity>();
                isEdit = dbEntities != null && dbEntities.Any();

                // Se for um novo registro (inserção).
                if (!isEdit && typeof(AuditLogs) != typeof(TEntity))
                {
                    var entity = new EntityAttributesModelFactory<TEntity>().InterpretEntity(_domain, true);
                    AuditLogs auditoria = AuditService.GenerateInsertAudit<TEntity>(_entityContext, useAuditLog.StepValue?.CodigoUsuario.ToString());
                    _transaction.GetRepository<AuditLogs>().Insert(auditoria).Execute();
                    useAuditLog.StepValue?.Action?.Invoke(auditoria.Id);
                }
            }

            // Gera a consulta final de inserção ou atualização.
            var whereClause = AddWhere(_levelModels.First(step => step.StepType == StepType.WHERE).StepExpression);
            var query = string.Format(
                DBKeysConstants.INSERT_NOT_EXISTS_ELSE_ACTION,
                _entityContext.FullName,
                whereClause,
                insertQuery,
                GenerateUpdateScript(isEdit)
            );

            return query; // Retorna a consulta gerada.
        }
    }
}
