using System;

namespace DB.Query.Core.Annotations.StoredProcedure
{
    public partial class ProcedureAttribute : Attribute
    {
        public string ProcedureName { get; set; }

        public ProcedureAttribute(string procedure)
        {
            ProcedureName = procedure;
        }
    }
}
