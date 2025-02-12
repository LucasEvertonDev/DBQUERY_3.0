using DB.Query.Core.Enuns;
using SIGN.Query.Models.PersistenceContext.Entities.SignCi;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DB.Query.Core.Models
{
    public class AuditEntry
    {
        public string UserId { get; set; }

        public string TableName { get; set; }

        public Dictionary<string, object> KeyValues { get; } = new();

        public Dictionary<string, object> OldValues { get; } = new();

        public Dictionary<string, object> NewValues { get; } = new();

        public AuditType AuditType { get; set; }

        public List<string> ChangedColumns { get; } = new();

        public AuditEntry() { }

        public AuditEntry(AuditLogs auditLogs)
        {
            UserId = auditLogs.UserId;
            TableName = auditLogs.TableName;
            KeyValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLogs.PrimaryKey);
            OldValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLogs.OldValues);
            NewValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLogs.NewValues);
            ChangedColumns = JsonSerializer.Deserialize<List<string>>(auditLogs.AffectedColumns);
        }

        public AuditLogs ToAudit()
        {
            var audit = new AuditLogs
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                Type = AuditType.ToString(),
                TableName = TableName,
                DateTime = DateTime.Now,
                PrimaryKey = JsonSerializer.Serialize(KeyValues),
                OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
                AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns)
            };

            return audit;
        }
    }
}
