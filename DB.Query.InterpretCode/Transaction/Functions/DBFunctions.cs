using System.Linq.Expressions;
using System;
using DB.Query.Core.Models;
using DB.Query.Utils.Helpers;

namespace DB.Query.Core.Functions
{
    public class DBFunctions
    {
        /// <summary>
        /// Trata a lista de colunas a ser usada
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        protected dynamic[] Columns(params dynamic[] array)
        {
            return array;
        }

        /// <summary>
        /// Função para obter o Top da consulta
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected int? Top(int i)
        {
            return i;
        }

        /// <summary>
        ///  Função para obter o Count
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected Count Count(dynamic prop)
        {
            return null;
        }

        /// <summary>
        ///  Função para obter o Count
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected Count Count()
        {
            return null;
        }

        /// <summary>
        ///  Função para obter o Count
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected T Count<T>(dynamic prop)
        {
            return default;
        }

        /// <summary>
        ///  Função para obter o Count
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        protected T Count<T>()
        {
            return default;
        }

        /// <summary>
        ///  Função para obter o Max
        /// </summary>
        /// <returns></returns>
        protected Max Max(dynamic prop)
        {
            return null;
        }

        /// <summary>
        ///  Função para obter o Max
        /// </summary>
        /// <returns></returns>
        protected T Max<T>(dynamic prop)
        {
            return default;
        }

        /// <summary>
        ///  Upper
        /// </summary>
        /// <returns></returns>
        protected Upper Upper(dynamic prop)
        {
            return null;
        }

        /// <summary>
        ///  Upper
        /// </summary>
        /// <returns></returns>
        protected T Upper<T>(dynamic prop)
        {
            return default;
        }

        /// <summary>
        ///  Função para obter o Min
        /// </summary>
        /// <returns></returns>
        protected Min Min(dynamic prop)
        {
            return null;
        }

        /// <summary>
        ///  Função para obter o Min
        /// </summary>
        /// <returns></returns>
        protected T Min<T>(dynamic prop)
        {
            return default;
        }

        /// <summary>
        ///  Função para obter o Sum
        /// </summary>
        /// <returns></returns>
        protected Sum Sum(dynamic prop)
        {
            return null;
        }

        /// <summary>
        ///  Função para obter o Sum
        /// </summary>
        /// <returns></returns>
        protected T Sum<T>(dynamic prop)
        {
            return default;
        }

        /// <summary>
        /// Função para aplicar o alias "AS"
        /// </summary>
        /// <returns></returns>
        protected Alias Alias(object prop, string Name)
        {
            return null;
        }

        /// <summary>
        /// Função para aplicar o alias "AS"
        /// </summary>
        /// <returns></returns>
        protected T Alias<T>(object prop, string Name)
        {
            return default;
        }

        /// <summary>
        /// Função para aplicar o alias "IsNull"
        /// </summary>
        /// <returns></returns>
        protected IsNull IsNull(object prop, dynamic Name)
        {
            return null;
        }

        /// <summary>
        /// Função para aplicar o alias "IsNull"
        /// </summary>
        /// <returns></returns>
        protected T IsNull<T>(object prop, dynamic Name)
        {
            return default;
        }

        /// <summary>
        /// Função para o Concat
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        protected string Concat(params dynamic[] props)
        {
            return null;
        }

        /// <summary>
        /// Escrever consultas sql Favor usar name of para impedir problemas
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected dynamic Sql(string sql)
        {
            return null;
        }

        /// <summary>
        /// Escrever consultas sql Favor usar name of para impedir problemas
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected T Sql<T>(string sql)
        {
            return default;
        }

        /// <summary>
        /// Injeta a query assim como o sql
        ///  $"(CASE {string.Join(" ", when)} END)"
        /// </summary>
        /// <param name="when">WhenSql(string codition, string value) / ElseSql(string value)</param>
        /// <returns></returns>
        protected T CASE<T>(params string[] when) => default;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="when"></param>
        /// <returns></returns>
        protected string CASE(params string[] when) => $"(CASE {string.Join(" ", when)} END)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codition"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string WHEN(string codition, string value) => $"WHEN ({codition}) THEN {value}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ELSE(string value) => $"ELSE {value}";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codition"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        protected T IIF<T>(string codition, string trueValue, string falseValue) => default;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codition"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        protected string IIF(string codition, string trueValue, string falseValue) => $"IIF({codition}, {trueValue} , {falseValue})";

        /// <summary>
        /// Injeta a query assim como o sql
        ///   $"ISNULL({value}, {defaultValue})";
        /// </summary>
        /// <returns></returns>
        protected T IS_NULL<T>(string value, string defaultValue) => default;

        /// <summary>
        /// Injeta a query assim como o sql
        ///   $"ISNULL({value}, {defaultValue})";
        /// </summary>
        /// <returns></returns>
        protected string IS_NULL(string value, string defaultValue) => $"ISNULL({value}, {defaultValue})";

        /// <summary>
        /// Injeta a query assim como o sql
        ///   $"Convert({type}, {value});
        /// </summary>
        /// <returns></returns>
        protected T CONVERT<T>(string type, string value) => default;

        /// <summary>
        /// Injeta a query assim como o sql
        ///  $"Convert({type}, {value})
        /// </summary>
        /// <returns></returns>
        protected string CONVERT(string type, string value) => $"Convert({type}, {value})";

        /// <summary>
        /// Injeta a query assim como o sql
        ///  $"STRING_AGG({expression}, {separator})";
        /// </summary>
        /// <returns></returns>
        protected string STRING_AGG(string expression, string separator) => $"STRING_AGG({expression}, {separator})";

        /// <summary>
        /// Injeta a query assim como o sql
        ///  $"STRING_AGG({expression}, {separator})";
        /// </summary>
        /// <returns></returns>
        protected T STRING_AGG<T>(string expression, string separator) => default;

        /// <summary>
        /// Injeta a query assim como o sql
        ///  $"CONCAT({string.Join(", ", param)})";
        /// </summary>
        /// <returns></returns>
        protected string CONCAT(params string[] param) => $"CONCAT({string.Join(", ", param)})";

        /// <summary>
        /// Injeta a query assim como o sql
        ///  $"CONCAT({string.Join(", ", param)})";
        /// </summary>
        /// <returns></returns>
        protected T CONCAT<T>(params string[] param) => default;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected static string Name<T>(Expression<Func<T, dynamic>> func)
        {
            return ExpressionHelper.TranslateLambda(func, func.Parameters[0].Name != "_", true);
        }
    }
}
