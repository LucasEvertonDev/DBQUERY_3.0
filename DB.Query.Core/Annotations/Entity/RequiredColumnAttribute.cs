using System.ComponentModel.DataAnnotations;

namespace DB.Query.Core.Annotations.Entity
{
    public partial class RequiredColumnAttribute : ValidationAttribute
    {
        private static string errorMessage { get; set; } = "A coluna {0} não pode ser persistida com valor nulo.";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        public RequiredColumnAttribute(string columnName) : base(string.Format(errorMessage, columnName))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            return new RequiredAttribute().IsValid(value);
        }
    }
}
