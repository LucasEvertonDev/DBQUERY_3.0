using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.InterpretCode.Factorys;
using DB.Query.InterpretCode.Services.Others;
using DB.Query.InterpretCode.Transaction;
using DB.Query.Utils.Extensions;
using SIGN.Query.Models.PersistenceContext.Entities.SignCi;
using System.Linq;
using System.Text;

namespace DB.Query.InterpretCode.Services.InterpretServices
{
    /// <summary>
    /// Serviço para interpretar operações de inserção de entidades.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade que será manipulada.</typeparam>
    public class InterpretInsertService<TEntity> : InterpretService<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// Transação associada ao serviço.
        /// </summary>
        private DBTransaction _transaction;

        /// <summary>
        /// Controla se o log de auditoria deve ser utilizado.
        /// </summary>
        private bool _useAuditLog;

        /// <summary>
        /// Construtor para inicializar o serviço de inserção.
        /// </summary>
        /// <param name="DBTransaction">Transação a ser utilizada.</param>
        /// <param name="useAuditLog">Indica se o log de auditoria deve ser utilizado.</param>
        public InterpretInsertService(DBTransaction DBTransaction, bool useAuditLog = true)
        {
            _transaction = DBTransaction;
            _useAuditLog = useAuditLog;
        }

        /// <summary>
        /// Executa a interpretação da operação de inserção.
        /// </summary>
        /// <returns>Consulta SQL gerada para a operação.</returns>
        protected override string RunInterpret()
        {
            var step = _levelModels.First(); // Obtém o primeiro passo do modelo.
            _domain = step.StepValue; // Armazena o domínio atual.

            IsValid(_domain); // Valida o domínio.

            return step.StepType switch // Utiliza switch para determinar o tipo de operação.
            {
                StepType.INSERT => GenerateInsertScript(),
                StepType.INSERT_OR_UPDATE => GenerateInsertScript(),
                StepType.DELETE_AND_INSERT => GenerateInsertScript(),
                StepType.INSERT_NOT_EXISTS => GenerateInsertIfNotExistsScript(),
                _ => base.RunInterpret() // Chama a implementação base para outras operações.
            };
        }

        protected string GenerateInsertScript()
        {
            _entityContext = new EntityAttributesModelFactory<TEntity>().InterpretEntity(_domain, true, _entityContext); // Interpreta a entidade.

            var props = _entityContext.Props.Where(a => !a.Identity); // Obtém as propriedades não identificadoras.

            var columns = props.Select(a => a.GetFullName(_entityContext.Name)); // Obtém os nomes das colunas.
            var identityAttribute = _entityContext.Props.FirstOrDefault(a => a.Identity && a.PrimaryKey); // Obtém o atributo de identidade, se existir.

            var identity = identityAttribute == null ? string.Empty : string.Concat(DBKeysConstants.OUTPUT_INSERTED, identityAttribute.Name); // Monta a string para a saída da identidade.
            var valores = props.Select(a => TreatValue(a.Valor, true)); // Trata os valores das propriedades.

            var queryBuilder = new StringBuilder(); // Usando StringBuilder para melhor performance
            queryBuilder.AppendFormat(
                DBKeysConstants.INSERT,
                _entityContext.FullName,
                string.Join(", ", columns),
                identity,
                string.Join(", ", valores)); // Monta a consulta de inserção.

            // Verifica se o log de auditoria deve ser utilizado.
            var useAuditLogStep = _levelModels.FirstOrDefault(step => step.StepType == StepType.USE_AUDIT_LOG);
            if (useAuditLogStep != null && _useAuditLog
                && typeof(AuditLogs) != typeof(TEntity) && _transaction.GetTransaction() != null)
            {
                var entity = new EntityAttributesModelFactory<TEntity>().InterpretEntity(_domain, true); // Interpreta a entidade para auditoria.

                AuditLogs auditoria = AuditService.GenerateInsertAudit<TEntity>(_entityContext,
                    useAuditLogStep.StepValue?.CodigoUsuario.ToString()); // Gera o log de auditoria.

                _transaction.GetRepository<AuditLogs>().Insert(auditoria).Execute(); // Insere o log de auditoria.

                // Executa a ação associada ao log, se houver.
                useAuditLogStep.StepValue?.Action?.Invoke(auditoria.Id);
            }

            return queryBuilder.ToString(); // Retorna a consulta gerada.
        }

        /// <summary>
        /// Gera o script SQL para uma operação de inserção que só ocorre se não existir.
        /// </summary>
        /// <returns>Consulta SQL de inserção se não existir.</returns>
        protected string GenerateInsertIfNotExistsScript()
        {
            _entityContext = new EntityAttributesModelFactory<TEntity>().InterpretEntity(_domain, true, _entityContext); // Interpreta a entidade.

            var insert = GenerateInsertScript(); // Gera a consulta de inserção.
            var whereStep = _levelModels.FirstOrDefault(step => step.StepType == StepType.WHERE)?.StepExpression; // Obtém a cláusula WHERE, se existir.

            // Monta as cláusulas para a condição de não existência.
            var objectClausules = _entityContext.Props.Where(a => !a.Identity).Select(a =>
            {
                var value = TreatValue(a.Valor, true);
                return !DBKeysConstants.NULL.Equals(value)
                    ? string.Concat(a.Name, DBKeysConstants.EQUALS_WITH_SPACE, value)
                    : string.Concat(a.Name, DBKeysConstants.ISNULL_VALUE_WITH_SPACE);
            });

            var whereSql = whereStep is null
                ? DBKeysConstants.WHERE_WITH_SPACE + string.Join(DBKeysConstants.AND_WITH_SPACE, objectClausules)
                : AddWhere(whereStep); // Monta a cláusula WHERE.

            var queryBuilder = new StringBuilder(); // Usando StringBuilder para melhor performance
            queryBuilder.AppendFormat(
                DBKeysConstants.INSERT_NOT_EXISTS,
                _entityContext.FullName,
                whereSql,
                insert); // Monta a consulta final de inserção se não existir.

            // Verifica se o log de auditoria deve ser utilizado.
            var useAuditLogStep = _levelModels.FirstOrDefault(step => step.StepType == StepType.USE_AUDIT_LOG);
            if (useAuditLogStep != null && _useAuditLog
                && typeof(AuditLogs) != typeof(TEntity) && _transaction.GetTransaction() != null)
            {
                var sql = _transaction.GetRepository<TEntity>()
                    .Select()
                    .GetQuery() + whereSql; // Gera a consulta de verificação de existência.

                var entidade = _transaction.ExecuteQuery(sql).OfType<TEntity>().FirstOrDefault(); // Verifica se a entidade já existe.

                // Se não existir, gera e registra o log de auditoria.
                if (entidade is null)
                {
                    var entity = new EntityAttributesModelFactory<TEntity>().InterpretEntity(_domain, true);

                    AuditLogs auditoria = AuditService.GenerateInsertAudit<TEntity>(_entityContext,
                        useAuditLogStep.StepValue?.CodigoUsuario.ToString());

                    _transaction.GetRepository<AuditLogs>().Insert(auditoria).Execute(); // Insere o log de auditoria.

                    // Executa a ação associada ao log, se houver.
                    useAuditLogStep.StepValue?.Action?.Invoke(auditoria.Id);
                }
            }

            return queryBuilder.ToString(); // Retorna a consulta gerada.
        }
    }
}
