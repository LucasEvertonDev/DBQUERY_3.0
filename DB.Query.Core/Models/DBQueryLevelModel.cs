using DB.Query.Core.Enuns;
using System;
using System.Linq.Expressions;

namespace DB.Query.Core.Models
{
    public class DBQueryStepModel
    {
        public StepType StepType { get; set; }
        public dynamic StepValue { get; set; }
        public Expression StepExpression { get; set; }
        public string Documentation { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Type ReturnType { get; set; }
    }
}
