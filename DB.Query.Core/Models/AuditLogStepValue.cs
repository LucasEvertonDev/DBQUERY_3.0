using System;

namespace DB.Query.Core.Models
{
    public class AuditLogStepValue
    {
        public long CodigoUsuario { get; set; }
        public Action<Guid> Action { get; set; }
    }
}
