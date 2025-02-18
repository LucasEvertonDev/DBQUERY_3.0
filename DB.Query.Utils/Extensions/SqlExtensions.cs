using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DB.Query.Utils.Extensions
{
    public static class SqlExtensions
    {
        /// <summary>
        /// Excuta o sql e retorna a datatable 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static DataTable ExecuteSql(this SqlCommand Sql_Comando)
        {
            DataTable dataDados = new DataTable();
            SqlDataAdapter sql_Ada = new SqlDataAdapter(Sql_Comando);
            sql_Ada.Fill(dataDados);

            return dataDados;
        }

        /// <summary>
        /// Excuta o sql e retorna a datatable 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async static Task<DataTable> ExecuteSqlAsync(this SqlCommand sqlComando)
        {
            DataTable dataDados = new DataTable();
            using (var reader = await sqlComando.ExecuteReaderAsync())
            {
                dataDados.Load(reader);
            }

            return dataDados;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameterName"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="parameterDirection"></param>
        public static void AddParameter(this SqlCommand sqlCommand, string parameterName, SqlDbType sqlDbType, object value, int? size = null, ParameterDirection parameterDirection = ParameterDirection.Input)
        {
            if (size.HasValue)
            {
                sqlCommand.Parameters.Add(new SqlParameter(parameterName, sqlDbType, size.Value) { Value = value == null ? DBNull.Value : value, Direction = parameterDirection });
            }
            else
            {
                sqlCommand.Parameters.Add(new SqlParameter(parameterName, sqlDbType) { Value = value == null ? DBNull.Value : value, Direction = parameterDirection });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static string PrintSql(this SqlCommand sc)
        {
            StringBuilder sql = new StringBuilder();
            bool FirstParam = true;

            sql.AppendLine("use " + sc.Connection?.Database + ";");
            switch (sc.CommandType)
            {
                case CommandType.StoredProcedure:
                    sql.AppendLine("declare @return_value int;");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if (sp.Direction == ParameterDirection.InputOutput || sp.Direction == ParameterDirection.Output)
                        {
                            sql.Append("declare " + sp.ParameterName + "\t" + sp.SqlDbType.ToString() + "\t= ");

                            sql.AppendLine((sp.Direction == ParameterDirection.Output ? "null" : sp.ParameterValueForSQL()) + ";");

                        }
                    }

                    sql.AppendLine("exec [" + sc.CommandText + "]");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if (sp.Direction != ParameterDirection.ReturnValue)
                        {
                            sql.Append(FirstParam ? "\t" : "\t, ");

                            if (FirstParam) FirstParam = false;

                            if (sp.Direction == ParameterDirection.Input)
                            {
                                if (sp == null)
                                {
                                    sql.AppendLine(
                                        $"{GetParamterName(sp)} = {"null"}");
                                }
                                else
                                {
                                    var valor = sp.ParameterValueForSQL() == null ? "null" : sp.ParameterValueForSQL();

                                    sql.AppendLine(
                                        $"{GetParamterName(sp)} = {valor}");
                                }
                            }
                            else

                                sql.AppendLine(GetParamterName(sp) + " = " + sp.ParameterName + " output");
                        }
                    }
                    sql.AppendLine(";");

                    sql.AppendLine("select 'Return Value' = convert(varchar, @return_value);");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if (sp.Direction == ParameterDirection.InputOutput || sp.Direction == ParameterDirection.Output)
                        {
                            sql.AppendLine("select '" + sp.ParameterName + "' = convert(varchar, " + sp.ParameterName + ");");
                        }
                    }
                    break;
                case CommandType.Text:
                    sql.AppendLine(sc.CommandText);
                    break;
            }

            return sql.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static string GetParamterName(SqlParameter sqlParameter)
        {
            if (sqlParameter.ParameterName.Contains("@"))
            {
                return sqlParameter.ParameterName;
            }
            return string.Concat("@", sqlParameter.ParameterName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static string ParameterValueForSQL(this SqlParameter sp)
        {
            string retval = "";

            if (sp.Value == DBNull.Value)
            {
                return null;
            }

            switch (sp.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.DateTimeOffset:
                    retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                    break;
                case SqlDbType.Bit:
                    retval = sp.Value.ToBooleanOrDefault(false).HasValue ? sp.Value.ToBooleanOrDefault(false).Value ? "1" : "0" : null;
                    break;
                case SqlDbType.Date:
                case SqlDbType.SmallDateTime:
                    retval = "'" + Convert.ToDateTime(sp.Value.ToString()).Date.ToString("yyyy-MM-dd") + "'";
                    break;
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    retval = "'" + Convert.ToDateTime(sp.Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    break;
                default:
                    retval = sp.Value == null ? null : sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool? ToBooleanOrDefault(this object o, bool defaultValue = false)
        {
            if (o == null)
                return null;
            string value = o.ToString().ToLower();
            switch (value)
            {
                case "yes":
                case "true":
                case "ok":
                case "y":
                    return true;
                case "no":
                case "false":
                case "n":
                    return false;
                default:
                    bool b;
                    if (bool.TryParse(o.ToString(), out b))
                        return b;
                    break;
            }
            return defaultValue;
        }
    }
}

