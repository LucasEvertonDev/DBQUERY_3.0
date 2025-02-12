using System.Linq.Expressions;
using System.Reflection;

namespace DB.Query.Utils.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string GetDebugView(this Expression exp)
        {
            if (exp == null)
                return null;

            var propertyInfo = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
            return propertyInfo.GetValue(exp) as string;
        }
    }
}
