using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.Core.Models;
using DB.Query.InterpretCode.Factorys;
using SIGN.Query.Models.PersistenceContext.Entities.SignCi;
using System.Collections.Generic;
using System.Linq;

namespace DB.Query.InterpretCode.Services.Others
{
    public static class AuditService
    {
        /// <summary>
        /// Gera registros de auditoria para atualizações de entidades.
        /// </summary>
        /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
        /// <param name="currentEntity">Atributos da entidade atual.</param>
        /// <param name="oldEntities">Lista de entidades antigas.</param>
        /// <param name="columnsSet">Colunas que foram modificadas.</param>
        /// <param name="usuarioCorrente">ID do usuário atual.</param>
        /// <returns>Lista de registros de auditoria.</returns>
        public static List<AuditLogs> GenerateUpdateAudit<TEntity>(
            EntityAttributesModel<TEntity> currentEntity,
            List<TEntity> oldEntities,
            List<string> columnsSet,
            string usuarioCorrente) where TEntity : EntityBase
        {
            var auditorias = new List<AuditLogs>();

            foreach (var oldEntity in oldEntities)
            {
                var entityModel = new EntityAttributesModelFactory<TEntity>().InterpretEntity(oldEntity, true);
                var auditEntry = new AuditEntry
                {
                    TableName = currentEntity.FullName,
                    AuditType = AuditType.Update,
                    UserId = usuarioCorrente
                };

                foreach (var prop in currentEntity.Props)
                {
                    var propOld = entityModel.Props.FirstOrDefault(p => p.Name == prop.Name);

                    if (prop.PrimaryKey)
                    {
                        auditEntry.KeyValues.Add(prop.Name, propOld.Valor);
                    }

                    // Verifica se a coluna foi modificada
                    if (columnsSet.Any() && !columnsSet.Contains(prop.Name) ||
                        Equals(propOld.Valor, prop.Valor))
                    {
                        continue; // Ignora colunas não alteradas
                    }

                    // Adiciona valores antigos e novos
                    auditEntry.OldValues.Add(propOld.Name, propOld.Valor);
                    auditEntry.ChangedColumns.Add(prop.Name);
                    auditEntry.NewValues.Add(prop.Name, prop.Valor);
                }

                if (auditEntry.ChangedColumns.Count > 0)
                {
                    auditorias.Add(auditEntry.ToAudit());
                }
            }
            return auditorias;
        }

        /// <summary>
        /// Gera registros de auditoria para exclusões de entidades.
        /// </summary>
        /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
        /// <param name="oldEntities">Lista de entidades antigas.</param>
        /// <param name="usuarioCorrente">ID do usuário atual.</param>
        /// <returns>Lista de registros de auditoria.</returns>
        public static List<AuditLogs> GenerateDeleteAudit<TEntity>(
            List<TEntity> oldEntities,
            string usuarioCorrente) where TEntity : EntityBase
        {
            var auditorias = new List<AuditLogs>();

            foreach (var oldEntity in oldEntities)
            {
                var entityModel = new EntityAttributesModelFactory<TEntity>().InterpretEntity(oldEntity, true);
                var auditEntry = new AuditEntry
                {
                    TableName = entityModel.FullName,
                    AuditType = AuditType.Delete,
                    UserId = usuarioCorrente
                };

                foreach (var prop in entityModel.Props)
                {
                    if (prop.PrimaryKey)
                    {
                        auditEntry.KeyValues.Add(prop.Name, prop.Valor);
                    }

                    auditEntry.OldValues.Add(prop.Name, prop.Valor);
                    auditEntry.ChangedColumns.Add(prop.Name);
                }

                auditorias.Add(auditEntry.ToAudit());
            }
            return auditorias;
        }

        /// <summary>
        /// Gera registro de auditoria para inserções de entidades.
        /// </summary>
        /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
        /// <param name="currentEntity">Atributos da entidade atual.</param>
        /// <param name="usuarioCorrente">ID do usuário atual.</param>
        /// <returns>Registro de auditoria.</returns>
        public static AuditLogs GenerateInsertAudit<TEntity>(
            EntityAttributesModel<TEntity> currentEntity,
            string usuarioCorrente) where TEntity : EntityBase
        {
            var auditEntry = new AuditEntry
            {
                TableName = currentEntity.FullName,
                AuditType = AuditType.Create,
                UserId = usuarioCorrente
            };

            foreach (var prop in currentEntity.Props)
            {
                if (prop.PrimaryKey)
                {
                    auditEntry.KeyValues.Add(prop.Name, prop.Valor);
                }

                auditEntry.ChangedColumns.Add(prop.Name);
                auditEntry.NewValues.Add(prop.Name, prop.Valor);
            }

            return auditEntry.ToAudit();
        }
    }
}
