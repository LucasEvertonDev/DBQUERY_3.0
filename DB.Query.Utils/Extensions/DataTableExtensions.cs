using DB.Query.Core.Annotations.Entity;
using DB.Query.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace DB.Query.Utils.Extensions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> OfType<T>(this DataTable dt)
        {
            if (dt.Columns.Count == 1)
            {
                return dt.Rows.OfType<DataRow>()
                    .Select(dr => ChangeType(dr[0], typeof(T))).OfType<T>().ToList();
            }

            // Obtenha os nomes das colunas em uma lista.
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var properties = typeof(T).GetProperties();
            var propertyCache = new Dictionary<string, PropertyInfo>();

            // Cache das propriedades e atributos de coluna
            foreach (var prop in properties)
            {
                var columnAttribute = prop.GetCustomAttributes(typeof(ColumnAttribute), false)
                                          .FirstOrDefault() as ColumnAttribute;

                // Use o nome da coluna do atributo ou o nome da propriedade, dependendo do que for encontrado.
                var columnName = columnAttribute != null ? columnAttribute.DisplayName : prop.Name;
                if (columnNames.Contains(columnName))
                {
                    propertyCache[columnName] = prop; // Armazena a propriedade no cache
                }
            }

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var column in propertyCache.Keys)
                {
                    var propertyInfo = propertyCache[column];
                    propertyInfo.SetValue(objT, ChangeType(row[column], propertyInfo.PropertyType));
                }

                return objT;
            }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<object> OfType(this DataTable dt, Type obj)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = obj.GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance(obj);
                for (var i = 0; i < properties.Count(); i++)
                {
                    var pro = properties[i];
                    ColumnAttribute columnAttribute = (ColumnAttribute)pro.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
                    if (columnAttribute != null && columnNames.Contains(columnAttribute.DisplayName))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, ChangeType(row[columnAttribute.DisplayName], pI.PropertyType));
                    }
                    else if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> OfTypeProcedure<T>(this DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                for (var i = 0; i < properties.Count(); i++)
                {
                    var pro = properties[i];
                    ColumnAttribute columnAttribute = (ColumnAttribute)pro.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
                    if (columnAttribute != null && columnNames.Contains(columnAttribute.DisplayName))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, ChangeType(row[columnAttribute.DisplayName], pI.PropertyType));
                    }
                    else if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversion"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type conversion)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (string.IsNullOrEmpty(value?.ToString()))
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="list"></param>
        public static DataTable AddColluns(this DataTable dt, params string[] list)
        {
            list.ToList().ForEach(a =>
            {
                dt.Columns.Add(a);
            });
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> AsDynamicEnumerable(this DataTable table)
        {
            if (table == null)
            {
                yield break;
            }

            foreach (DataRow row in table.Rows)
            {
                IDictionary<string, object> dRow = new ExpandoObject();

                foreach (DataColumn column in table.Columns)
                {
                    var value = row[column.ColumnName];
                    dRow[column.ColumnName] = Convert.IsDBNull(value) ? null : value;
                }

                yield return dRow;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public static void RenameColumn(this DataTable dt, string oldName, string newName)
        {
            dt.Columns[oldName].ColumnName = newName;
        }
    }
}