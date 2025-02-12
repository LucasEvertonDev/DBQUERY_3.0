using System.ComponentModel.DataAnnotations;

namespace DB.Query.Core.Annotations.Entity
{
    public partial class StringColumnLengthAttribute : ValidationAttribute
    {
        private static string _errorMessage { get; set; } = "A coluna {0} não pode possuir mais que {1}.";

        private int _lenght { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        public StringColumnLengthAttribute(string columnName, int lenght) : base(string.Format(_errorMessage, columnName, lenght))
        {
            _lenght = lenght;
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
                return true;
            }
            return new StringLengthAttribute(_lenght).IsValid(value);
        }
    }
}
