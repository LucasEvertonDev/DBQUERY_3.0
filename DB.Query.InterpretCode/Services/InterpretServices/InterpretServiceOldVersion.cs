using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DB.Query.Core.Models;
using DB.Query.Core.Annotations;
using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.Core.Annotations.Entity;

namespace DB.Query.InterpretCode.Services.InterpretServices
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InterpretServiceOldVersion<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected TEntity _domain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected EntityAttributesModel<TEntity> _entityContext { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, string> _expressions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected Expression _currentExpression { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected List<DBQueryStepModel> _levelModels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected bool _useAlias => _levelModels.Exists(a => a.StepType == StepType.USE_ALIAS);
        /// <summary>
        /// 
        /// </summary>
        protected string _alias => _levelModels.Where(a => a.StepType == StepType.USE_ALIAS).FirstOrDefault()?.StepValue;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected bool ContainsProperty(object obj, string name) => obj.GetType().GetProperty(name) != null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetFullName(Type type) => string.Concat(GetDatabaseName(type), DBKeysConstants.T_A, GetTableName(type));
        /// <summary>
        /// 
        /// </summary>
        protected List<string> _defaultFunctions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InterpretServiceOldVersion()
        {
            _defaultFunctions = new List<string>()
            {
                DBQueryConstants.SUM_FUNCTION,
                DBQueryConstants.MAX_FUNCTION,
                DBQueryConstants.MIN_FUNCTION,
                DBQueryConstants.COUNT_FUNCTION,
                DBQueryConstants.ALIAS_FUNCTION,
                DBQueryConstants.CONCAT_FUNCTION,
                DBQueryConstants.ALL_COLUMNS,
                DBQueryConstants.UPPER_FUNCTION,
                DBQueryConstants.ISNULL_FUNCTION,
                DBQueryConstants.SQL_FUNCTION,
                DBQueryConstants.NAME_FUNCTION
            };
            _expressions = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelModels"></param>
        /// <returns></returns>
        public string StartToInterpret(List<DBQueryStepModel> levelModels)
        {
            _levelModels = levelModels;
            return RunInterpret();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string RunInterpret()
        {
            return "";
        }

        #region Interpret Expressions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="_query"></param>
        /// <returns></returns>
        protected string AddGroupBy(Expression expressions, string _query)
        {
            bool contains = _query.Contains(DBKeysConstants.GROUP_BY);
            if (!contains)
            {
                _query += DBKeysConstants.GROUP_BY_WITH_SPACE;
            }
            if (expressions != null)
            {
                var properties = GetPropertiesExpression(expressions);
                if (contains && properties.Count > 0)
                {
                    _query += ", " + string.Join(", ", properties);
                }
                else
                {
                    _query += string.Join(", ", properties);
                }
            }

            return _query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="expressions"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        protected string AddOrderBy(string tipo, Expression expressions, string query, bool adicionadaOrdenacao)
        {
            if (!adicionadaOrdenacao)
            {
                query += DBKeysConstants.ORDER_BY_WITH_SPACE;
            }
            if (expressions != null)
            {
                var properties = GetPropertiesExpression(expressions);
                if (adicionadaOrdenacao && properties.Count > 0)
                {
                    query += ", " + string.Join($" {tipo}, ", properties) + $" {tipo}";
                }
                else
                {
                    query += string.Join($" {tipo}, ", properties) + $" {tipo}";
                }
            }

            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected string AddWhere(Expression expression)
        {
            string condition = string.Empty;
            _currentExpression = expression;
            _expressions = new Dictionary<string, string>();
            if (expression != null)
            {
                var splitCriteria = new List<string> { DBQueryConstants.EXPRESSION_DIVISOR };
                string exp = ((Expression)((dynamic)expression).Body).ToString();

                DememberExpression(((dynamic)expression).Body);

                var keys = _expressions.Keys.ToList();

                if (keys.Exists(a => exp.Contains(a)))
                {
                    for (var i = 0; i < keys.Count(); i++)
                    {
                        var key = keys[i];
                        // A etapa de condição é de suma importância. Caso ele não consiga tratar uma expression deve parar todo o processo
                        if (!string.IsNullOrEmpty(key) && string.IsNullOrEmpty(_expressions[key]?.Trim()) || !exp.Contains(key))
                        {
                            throw new Exception($"Erro ao montar condições da query {key} -> {exp}");
                        }
                        exp = exp.Replace(key, _expressions[key]);
                    }

                    exp = exp.Replace(DBQueryConstants.AND_ALSO, DBKeysConstants.AND)
                            .Replace(DBQueryConstants.OR_ELSE, DBKeysConstants.OR)
                            .Replace(DBQueryConstants.EQUALS, DBKeysConstants.EQUALS);

                    condition += string.Concat(DBKeysConstants.WHERE_WITH_SPACE, exp);
                }
                else
                {
                    condition += string.Concat(DBKeysConstants.WHERE_WITH_SPACE, exp);
                }
            }

            return condition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="strJoin"></param>
        /// <returns></returns>
        protected string AddJoin(Expression expression, string strJoin)
        {
            var query = string.Empty;
            _currentExpression = expression;
            var aux = _useAlias
                ? string.Concat(GetFullName(((dynamic)expression).Parameters[1].Type), DBKeysConstants.AS_WITH_SPACE, ((dynamic)expression).Parameters[1].Name)
                : GetFullName(((dynamic)expression).Parameters[1].Type);

            query += string.Format(strJoin, aux, DememberExpression(expression));

            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="strJoin"></param>
        /// <returns></returns>
        protected string AddJoinEmpresaFilial(Expression expression, string strJoin)
        {
            var query = string.Empty;
            _currentExpression = expression;
            var aux = _useAlias
                ? string.Concat(GetFullName(((dynamic)expression).Parameters[1].Type), DBKeysConstants.AS_WITH_SPACE, ((dynamic)expression).Parameters[1].Name)
                : GetFullName(((dynamic)expression).Parameters[1].Type);

            query += string.Format(strJoin, aux, DememberExpression(expression));
            query += AddClausesEmpresaFilial(GetTableName(((dynamic)expression).Parameters[0].Type), GetTableName(((dynamic)expression).Parameters[1].Type));
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table1"></param>
        /// <param name="table2"></param>
        /// <returns></returns>
        private string AddClausesEmpresaFilial(string table1, string table2)
        {
            return string.Format(DBKeysConstants.CLAUSE_EMPRESA_FILIAL, table1, table2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTableName(Type type, dynamic exp = null)
        {
            if (_useAlias && exp != null && ContainsProperty(exp, "Expression"))
            {
                return exp.Expression.Name;
            }
            if (_useAlias && exp != null && ContainsProperty(exp, "NodeType") && exp.NodeType == ExpressionType.Call && exp.Arguments.Count > 0)
            {
                return exp.Arguments[0].Expression.Name;
            }
            if (_useAlias && exp != null && ContainsProperty(exp, "NodeType") && exp.NodeType == ExpressionType.Call && exp.Arguments.Count == 0)
            {
                return exp.Object.Name;
            }
            if (_useAlias && exp != null && exp.NodeType == ExpressionType.Parameter)
            {
                return exp.Name;
            }
            var displayName = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

            return displayName != null ? displayName.TableName : type.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName(Type type)
        {
            var displayName = type.GetCustomAttributes(typeof(DatabaseAttribute), true).FirstOrDefault() as DatabaseAttribute;
            if (displayName != null)
            {
                return displayName.DatabaseName;
            }
            else
            {
                throw new Exception("Informe o nome da database");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetPropretyFullName(Type type, dynamic exp, dynamic member = null)
        {
            return string.Concat(GetTableName(type, exp), DBKeysConstants.SINGLE_POINT, GetCollumnName(member == null ? exp.Member : member));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected string GetCollumnName(PropertyInfo prop)
        {
            var att1 = prop.GetCustomAttributes(typeof(ColumnAttribute), false);
            var name1 = att1.Length > 0 ? (att1[0] as ColumnAttribute).DisplayName : prop.Name;
            return name1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="useAlias"></param>
        /// <param name="fromSelect"></param>
        /// <returns></returns>
        protected List<string> GetPropertiesExpression(Expression expression, bool useAlias = false, bool fromSelect = false)
        {
            var properties = new List<string>();
            _currentExpression = expression;
            dynamic exp = expression;

            if (expression.Type == typeof(Func<TEntity, dynamic>) && (!ContainsProperty(exp, "Body") || exp.Body.Type != typeof(object[]))
                && (!ContainsProperty(exp, "Body") || exp.Body.NodeType != ExpressionType.New && exp.Body.NodeType != ExpressionType.MemberInit))
            {
                properties.Add(GetPropertyOfSingleExpression(expression, false, useAlias));
            }
            else
            {
                if (exp.Body.NodeType == ExpressionType.New)
                {
                    if (fromSelect)
                    {
                        _levelModels.Where(A => A.StepType == StepType.CUSTOM_SELECT).FirstOrDefault().ReturnType = exp.Body.Type;
                    }

                    for (var i = 0; i < exp.Body.Arguments.Count; i++)
                    {
                        properties.Add(GetPropertyOfSingleExpression(exp.Body.Arguments[i], false, useAlias, fromSelect ? exp.Body.Members[i].Name : null));
                    }
                }
                else if (exp.Body.NodeType == ExpressionType.MemberInit)
                {
                    if (fromSelect)
                    {
                        _levelModels.Where(A => A.StepType == StepType.CUSTOM_SELECT).FirstOrDefault().ReturnType = exp.Body.Type;
                    }

                    foreach (var a in exp.Body.Bindings)
                    {
                        properties.Add(GetPropertyOfSingleExpression(a.Expression, false, useAlias, fromSelect ? a.Member.Name : null));
                    }
                }
                else if (ContainsProperty(exp.Body, "Arguments") && exp.Body.Arguments.Count > 0)
                {
                    foreach (var a in exp.Body.Arguments[0].Expressions)
                    {
                        properties.Add(GetPropertyOfSingleExpression(a, false, useAlias));
                    }
                }
                else
                {
                    properties.Add(GetPropertyOfSingleExpression(exp, false, useAlias));
                }
            }

            return properties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="hasParamter"></param>
        /// <param name="useAlias"></param>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        protected string GetPropertyOfSingleExpression(dynamic expression, bool hasParamter, bool useAlias, string aliasName = "")
        {
            string retorno = "";
            if (expression != null)
            {
                var exp = ContainsProperty(expression, "Body") ? expression?.Body : expression;
                if (exp.NodeType == ExpressionType.MemberAccess)
                {
                    retorno = GetMemberExpression(exp);
                }
                else if (exp.NodeType == ExpressionType.Constant)
                {
                    retorno = TreatValue(exp.Value, true)?.ToString();
                }
                else if (exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked)
                {
                    if (ContainsProperty(exp.Operand, "Expression"))
                    {
                        retorno = GetPropretyFullName(exp.Operand.Expression.Type, exp.Operand);
                    }
                    else if (ContainsProperty(exp.Operand, "Left"))
                    {
                        retorno = DememberExpression(exp.Operand);
                    }
                    else if (ContainsProperty(exp.Operand, "Operand"))
                    {
                        retorno = DememberExpression(exp.Operand.Operand);
                    }
                    else if (ContainsProperty(exp.Operand, "Arguments"))
                    {
                        retorno = GetPropertyOfSingleExpression(exp.Operand.Arguments[0], hasParamter, false);
                    }
                    else
                    {
                        retorno = exp.Operand.Value?.ToString();
                    }
                }
                else if (exp.NodeType == ExpressionType.Call)
                {
                    if (_defaultFunctions.Exists(f => f.Equals(exp.Method.Name)))
                    {
                        if (DBQueryConstants.ALL_COLUMNS.Equals(exp.Method.Name))
                        {
                            List<string> props = GetProperties(exp, exp.Object.Type);
                            retorno = string.Join(", ", props);
                        }
                        else if (DBQueryConstants.ALIAS_FUNCTION.Equals(exp.Method.Name))
                        {
                            var ret = GetPropertyOfSingleExpression(exp.Arguments[0], hasParamter, false);
                            return string.Concat(ret, DBKeysConstants.AS_WITH_SPACE, exp.Arguments[1].Value);
                        }
                        else if (DBQueryConstants.ISNULL_FUNCTION.Equals(exp.Method.Name))
                        {
                            var ret = GetPropertyOfSingleExpression(exp.Arguments[0], hasParamter, false);
                            var parametro2 = GetPropertyOfSingleExpression(exp.Arguments[1], hasParamter, false);

                            retorno = string.Format(DBKeysConstants.ISNULL, ret, parametro2);
                        }
                        else if (DBQueryConstants.COUNT_FUNCTION.Equals(exp.Method.Name) && !hasParamter)
                        {
                            if (exp.Arguments.Count > 0)
                            {
                                var ret = GetPropertyOfSingleExpression(exp.Arguments[0], hasParamter, false);
                                retorno = string.Format(DBKeysConstants.COUNT_ELEMENT, ret);
                            }
                            else
                            {
                                retorno = DBKeysConstants.COUNT;
                            }
                        }
                        else if (DBQueryConstants.CONCAT_FUNCTION.Equals(exp.Method.Name))
                        {
                            retorno = ConcatFunction(exp);
                        }
                        else if (DBQueryConstants.SQL_FUNCTION.Equals(exp.Method.Name))
                        {
                            retorno = Expression.Lambda(exp.Arguments[0]).Compile().DynamicInvoke();
                        }
                        else if (DBQueryConstants.NAME_FUNCTION.Equals(exp.Method.Name))
                        {
                            retorno = Expression.Lambda(exp).Compile().DynamicInvoke();
                        }
                        else
                        {
                            if (ContainsProperty(exp, "Body"))
                            {
                                var d = exp.Body.Arguments[0];
                                var aux = d.Expressions[0].Arguments[0].Operand;
                                retorno = string.Format(string.Concat(exp.Method.Name, "({0})"), GetPropretyFullName(aux.Expression.Type, aux.Expression, aux.Member));
                            }
                            else
                            {
                                var d = exp.Arguments[0];
                                if (ContainsProperty(d, "Expressions"))
                                {
                                    var aux = d.Expressions[0].Arguments[0].Operand;
                                    retorno = string.Format(string.Concat(exp.Method.Name, "({0})"), GetPropretyFullName(aux.Expression.Type, aux.Expression, aux.Member));
                                }
                                else if (ContainsProperty(exp, "Arguments") && ContainsProperty(d, "Expression"))
                                {
                                    retorno = string.Format(string.Concat(exp.Method.Name, "({0})"), GetPropretyFullName(d.Expression.Type, d));
                                }
                                else
                                {
                                    var aux = d.Operand;
                                    retorno = string.Format(string.Concat(exp.Method.Name, "({0})"), GetPropretyFullName(aux.Expression.Type, aux.Expression, aux.Member));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DBQueryConstants.TO_STRING.Equals(exp.Method.Name)
                            || DBQueryConstants.TO_STRING_SAFE.Equals(exp.Method.Name))
                        {
                            if (exp.Object.Type == typeof(string))
                            {
                                retorno = GetPropertyOfSingleExpression(exp.Object, hasParamter, false);
                            }
                            else
                            {
                                retorno = string.Format(DBKeysConstants.CONVERT_VARCHAR, GetPropertyOfSingleExpression(exp.Object, hasParamter, false));
                            }
                        }
                        else if (DBQueryConstants.GET_VALUE_OR_DEFAULT.Equals(exp.Method.Name))
                        {
                            retorno = GetPropertyOfSingleExpression(exp.Object, hasParamter, false);
                        }
                        else if (DBQueryConstants.PARSE.Equals(exp.Method.Name))
                        {
                            if (ContainsProperty(exp, "Arguments"))
                            {
                                retorno = GetPropertyOfSingleExpression(exp.Arguments[0], hasParamter, false);
                            }
                        }
                    }
                }
                else if (ContainsProperty(exp, "Left"))
                {
                    retorno = DememberExpression(exp);
                }
            }

            if (!string.IsNullOrEmpty(aliasName))
            {
                retorno = string.Concat(retorno, DBKeysConstants.AS_WITH_SPACE, aliasName);
            }

            return retorno ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected List<string> GetProperties(dynamic exp = null, Type type = null)
        {
            var list = new List<string>();
            Type currentType = type != null ? type : typeof(TEntity);
            List<PropertyInfo> infs = currentType.GetProperties().ToList();
            var tableName = GetTableName(currentType, exp);

            for (var i = 0; i < infs.Count; i++)
            {
                var prop = infs[i];
                if (prop.GetCustomAttributes(typeof(IgnoreAttribute), false).Count() == 0)
                {
                    var propName = prop.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;
                    if (propName != null)
                    {
                        list.Add(string.IsNullOrEmpty(tableName)
                            ? propName.DisplayName
                            : tableName + DBKeysConstants.SINGLE_POINT + propName.DisplayName);
                    }
                    else
                    {
                        list.Add(string.IsNullOrEmpty(tableName)
                            ? prop.Name
                            : tableName + DBKeysConstants.SINGLE_POINT + prop.Name);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected string DememberExpression(Expression expression)
        {
            if (expression is LambdaExpression && ((dynamic)expression).Parameters.Count > 1)
            {
                expression = ((dynamic)expression).Body;
            }

            bool ignore = false;
            string value = string.Empty, oldExpression = string.Empty, equalty = string.Empty;
            Expression left = null, right = null;

            if (ContainsProperty(expression, "Left"))
            {
                left = GetLeftNode(expression);
            }

            if (ContainsProperty(expression, "Right"))
            {
                right = GetRightNode(expression);
            }

            if (ContainsProperty(expression, "NodeType"))
            {
                equalty = GetComparador(expression);
            }

            if (left != null && !ContainsProperty(left, "Left"))
            {
                // se o separador não é um comparador (AND OU OR). É uma expressão unica verificar se é uma bool antes
                if (expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse)
                {
                    oldExpression = expression.ToString();
                    var booleanExpression = TreatBooleanPropertyExpression(left);
                    if (!string.IsNullOrEmpty(booleanExpression?.Trim()))
                    {
                        value = string.Format("{0} {1} {2}", booleanExpression, "{0}", "{1}");
                    }
                }
                else
                {
                    var result = ReadExpression(expression, left, ref ignore);
                    if (!string.IsNullOrEmpty(result) && !string.IsNullOrWhiteSpace(result))
                    {
                        oldExpression = expression.ToString();
                        value = string.Format("{0} {1} {2}", result, "{0}", "{1}");
                    }
                }
            }

            if (left == null)
            {
                oldExpression = expression.ToString();
                var booleanExpression = TreatBooleanPropertyExpression(expression);
                if (!string.IsNullOrEmpty(booleanExpression?.Trim()))
                {
                    value = booleanExpression;
                }
                else
                {
                    var methodExpression = ExtractMethod(expression);
                    value = string.IsNullOrEmpty(methodExpression?.Trim()) ? ReadExpression(expression, expression, ref ignore) : methodExpression;
                }
            }

            if (value == "" && !ignore)
            {
                var leftVal = DememberExpression(left);
                var rigthVal = DememberExpression(right);
                value = string.Format("{0} {1} {2}", leftVal, equalty, rigthVal);
            }
            else
            {
                dynamic r = right;
                if (right != null && ContainsProperty(right, "NodeType")
                  && right.NodeType == ExpressionType.Not && ContainsProperty(right, "Operand"))
                {
                    right = r.Operand;
                }

                // se o separador não é um comparador (AND OU OR). É uma expressão unica verificar se é uma bool antes
                if (r != null && !ContainsProperty(r, "Left")
                    && expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse)
                {
                    var booleanExpression = TreatBooleanPropertyExpression(r);
                    if (!string.IsNullOrEmpty(booleanExpression?.Trim()))
                    {
                        value = string.Format(value, equalty, booleanExpression);

                        if (!string.IsNullOrEmpty(oldExpression) && !string.IsNullOrEmpty(value) && !_expressions.ContainsKey(oldExpression))
                        {
                            _expressions.Add(oldExpression, value);
                        }

                        return value;
                    }
                }

                if (right is MemberExpression)
                {
                    var rightMen1 = right as MemberExpression;
                    bool isValue = true;
                    isValue = IsValue(r);

                    if (rightMen1.Member.Name == "Value")
                    {
                        rightMen1 = rightMen1.Expression as MemberExpression;
                    }

                    if (!isValue)
                    {
                        value = string.Format(value, equalty, GetPropretyFullName(rightMen1.Member.DeclaringType, rightMen1, rightMen1.Member));
                    }
                    else
                    {
                        value = TreatExpression(right, equalty, value);
                    }
                }
                else if (right is MethodCallExpression && DBQueryConstants.SQL_FUNCTION.Equals(r.Method.Name))
                {
                    value = string.Format(value, equalty, ExtractMethod(r));
                }
                else if (r != null && ContainsProperty(r, "Operand") && ContainsProperty(r, "NodeType")
                    && (r.NodeType == ExpressionType.Convert || r.NodeType == ExpressionType.ConvertChecked) && !IsValue(r.Operand))
                {
                    value = string.Format(value, equalty, GetPropretyFullName(r.Operand.Member.DeclaringType, r.Operand, r.Operand.Member));
                }
                else
                {
                    value = TreatExpression(right, equalty, value);
                }

                if (!string.IsNullOrEmpty(oldExpression) && !string.IsNullOrEmpty(value) && !_expressions.ContainsKey(oldExpression))
                {
                    _expressions.Add(oldExpression, value);
                }
            }
            return value;
        }

        /// <summary>
        /// Carrega o valor da member expression passada
        /// </summary>
        /// <param name="expressionComplete"></param>
        /// <param name="partExpression"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public string ReadExpression(Expression expressionComplete, Expression partExpression, ref bool ignore)
        {
            if (partExpression is MemberExpression)
            {
                var isValue = IsValue(partExpression);
                if (isValue)
                {
                    return TreatValue(GetValue(partExpression), true)?.ToString();
                }
                else
                {
                    dynamic l = partExpression;
                    if (partExpression.NodeType == ExpressionType.Not && ContainsProperty(partExpression, "Operand"))
                    {
                        partExpression = l.Operand;
                    }

                    var leftMem = partExpression as MemberExpression;
                    if (leftMem.Member.Name == "Value")
                    {
                        leftMem = leftMem.Expression as MemberExpression;
                    }
                    var propertyInfo = (PropertyInfo)leftMem.Member;
                    var name = GetCollumnName(propertyInfo);

                    if (propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), false).Count() == 0)
                    {
                        return GetTableName(propertyInfo.DeclaringType, partExpression) + "." + name;
                    }
                    else
                    {
                        ignore = true;
                    }
                }
            }
            else if (partExpression != null && ContainsProperty(expressionComplete, "Method") && DBQueryConstants.CONCAT_FUNCTION.Equals(((dynamic)partExpression).Method?.Name))
            {
                return ExtractMethod(partExpression);
            }
            else
            {
                return GetPropertyOfSingleExpression(partExpression, false, false);
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partExpression"></param>
        /// <returns></returns>
        private string GetMemberExpression(Expression partExpression)
        {
            dynamic l = partExpression;
            if (partExpression.NodeType == ExpressionType.Not && ContainsProperty(partExpression, "Operand"))
            {
                partExpression = l.Operand;
            }

            var leftMem = partExpression as MemberExpression;
            if (leftMem.Member.Name == "Value")
            {
                leftMem = leftMem.Expression as MemberExpression;
            }
            var propertyInfo = (PropertyInfo)leftMem.Member;
            var name = GetCollumnName(propertyInfo);

            return GetTableName(propertyInfo.DeclaringType, partExpression) + "." + name;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="expression"></param>
        public string TreatBooleanPropertyExpression(Expression expression)
        {
            string value = null;
            dynamic exp = expression;
            if (expression.Type == typeof(bool)
                && (expression.NodeType == ExpressionType.Not || expression.NodeType == ExpressionType.MemberAccess
                || expression.NodeType == ExpressionType.Call && exp.Method.Name == DBQueryConstants.GET_VALUE_OR_DEFAULT))
            {
                if (ContainsProperty(exp, "Expression"))
                {
                    if (exp.Expression.Type == typeof(bool?))
                    {
                        exp = exp.Expression;
                    }
                }
                if (expression.NodeType == ExpressionType.Not && ContainsProperty(expression, "Operand"))
                {
                    value = string.Concat(DBKeysConstants.EQUALS_WITH_SPACE, DBKeysConstants.FALSE_VALUE);
                    exp = ((dynamic)expression).Operand;
                    if (ContainsProperty(exp, "Expression"))
                    {
                        if (exp.Expression.Type == typeof(bool?))
                        {
                            exp = exp.Expression;
                        }
                    }
                }
                else
                {
                    value = string.Concat(DBKeysConstants.EQUALS_WITH_SPACE, DBKeysConstants.TRUE_VALUE);
                }

                if (ContainsProperty(exp, "Method") && exp.Method.Name == DBQueryConstants.GET_VALUE_OR_DEFAULT)
                {
                    return GetPropertyOfSingleExpression(exp, false, false) + value;
                }
                else
                    return string.Concat(GetPropretyFullName(exp.Member.DeclaringType, exp, exp.Member), value);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <param name="equality"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string TreatExpression(Expression right, string equality, string value)
        {
            var val = TreatValue(GetValue(right), true, right);
            if (DBKeysConstants.NULL.Equals(val))
            {
                if (DBKeysConstants.EQUALS.Equals(equality))
                {
                    equality = DBKeysConstants.IS;
                }
                else if (DBKeysConstants.NOT_EQUAL.Equals(equality))
                {
                    equality = DBKeysConstants.IS_NOT;
                }
            }
            return string.Format(value, equality, val);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected string ExtractMethod(Expression method)
        {
            string valueA = string.Empty;
            string valueB = string.Empty;
            string comparador = string.Empty;
            if (method is MethodCallExpression)
            {
                var mtd = (MethodCallExpression)method;
                comparador = TreatComparerByMethod(mtd.Method.Name);

                if (VerififyNativesMethods(mtd, ref valueA))
                {
                    return valueA;
                }
                if (DBQueryConstants.CONCAT_FUNCTION.Equals(mtd.Method.Name))
                {
                    return ConcatFunction(method);
                }
                else if (DBQueryConstants.SQL_FUNCTION.Equals(mtd.Method.Name))
                {
                    return (string)Expression.Lambda(mtd.Arguments[0]).Compile().DynamicInvoke();
                }
                else if (mtd.Arguments.Count > 0)
                {
                    List<dynamic> values = new List<dynamic>();
                    for (var i = 0; i < mtd.Arguments.Count; i++)
                    {
                        bool isValue = true;
                        string value = string.Empty;
                        if (IsValue(mtd.Arguments[i]))
                        {
                            dynamic arg = mtd.Arguments[i];
                            if (comparador == DBKeysConstants.IN || comparador == DBKeysConstants.NOT_IN)
                            {
                                value = InterpretIn(mtd, arg, value);
                            }
                            else if (arg != null && ContainsProperty(arg, "Member") && ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.MemberAccess)
                            {
                                value = GetValue(arg);
                            }
                            else if (arg.GetType().Name.Equals("PropertyExpression"))
                            {
                                value = string.Concat(GetTableName(arg.Member.DeclaringType, mtd), DBKeysConstants.SINGLE_POINT, GetCollumnName(arg.Member));
                            }
                            else if (arg.GetType().Name.Equals("ConstantExpression"))
                            {
                                value = arg.Value;
                            }
                            else if (ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.Call)
                            {
                                value = GetValue(arg);
                            }
                        }
                        else
                        {
                            isValue = false;
                            dynamic arg = mtd.Arguments[i];
                            if (ContainsProperty(arg, "Expression") && ContainsProperty(arg, "Type"))
                            {
                                value = string.Concat(GetTableName(arg.Expression.Type, mtd), DBKeysConstants.SINGLE_POINT, GetCollumnName(arg.Member));
                            }
                        }

                        values.Add(new { value, isValue });
                    }

                    if (values.Any())
                    {
                        valueA = values[0].isValue ? TreatValue(values[0].value, true)?.ToString() : values[0].value;
                        if (mtd.Arguments != null && mtd.Arguments.Count > 1)
                        {
                            dynamic arg = mtd.Arguments[1];
                            if (ContainsProperty(arg, "Method") && arg.Method != null
                                && DBQueryConstants.CONCAT_FUNCTION.Equals(arg.Method.Name))
                            {
                                valueB = ValidateValueByMethod(values[1], mtd.Method.Name, true);
                            }
                            else
                            {
                                valueB = ValidateValueByMethod(values[1], mtd.Method.Name, false);
                            }
                        }
                        else
                        {
                            // valueB = ValidateValueByMethod(values[1], mtd.Method.Name, false);
                        }
                    }
                }
            }

            return string.Format("{0} {1} {2}", valueA, comparador, valueB);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mtd"></param>
        /// <param name="arg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string InterpretIn(MethodCallExpression mtd, dynamic arg, string value)
        {
            if (ContainsProperty(arg, "Expressions") && arg.Expressions != null)
            {
                var aux = new List<string>();
                foreach (var item in arg.Expressions)
                {
                    var val = GetValue(item);
                    aux.Add("'" + val + "'");
                }
                value = "(" + string.Join(", ", aux) + ")";
            }
            else if (ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.Call && ContainsProperty(arg, "Type"))
            {
                var list = GetValue(arg);
                if (list.GetType() == typeof(string) || DBQueryConstants.GENERATE_SCRIPT_IN.Equals(arg.Method.Name))
                {
                    value = list;
                }
                else
                {
                    var aux = new List<string>();
                    foreach (var item in list)
                    {
                        aux.Add("'" + item.ToString() + "'");
                    }
                    value = "(" + string.Join(", ", aux) + ")";
                }
            }
            else if (arg != null && ContainsProperty(arg, "Member") && ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.MemberAccess)
            {
                var val = GetValue(arg);
                if (val.GetType() == typeof(string[]) || val.GetType() == typeof(short[]) || val.GetType() == typeof(int[]) || val.GetType() == typeof(long[]))
                {
                    var aux = new List<string>();
                    foreach (var item in val)
                    {
                        aux.Add("'" + item.ToString() + "'");
                    }
                    value = "(" + string.Join(", ", aux) + ")";
                }
                else
                {
                    value = GetValue(arg);
                }
            }
            else if (arg.GetType().Name.Equals("PropertyExpression"))
            {
                value = string.Concat(GetTableName(arg.Member.DeclaringType, mtd), DBKeysConstants.SINGLE_POINT, GetCollumnName(arg.Member));
            }
            else if (arg.GetType().Name.Equals("ConstantExpression"))
            {
                value = arg.Value;
            }

            return value == "()" ? "('')" : value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        private string ConcatFunction(dynamic exp)
        {
            var d = exp.Arguments[0];
            var list = new List<string>();

            for (var i = 0; i < d.Expressions.Count; i++)
            {
                var xp1 = d.Expressions[i];
                var xp = xp1;
                if (xp != null && xp.NodeType == ExpressionType.Convert && ContainsProperty(xp, "Operand"))
                {
                    xp = xp.Operand;
                }

                string value = "";
                if (xp is MemberExpression)
                {
                    bool isValue = true;
                    isValue = IsValue(xp);

                    if (!isValue)
                    {
                        var propertyInfo1 = (PropertyInfo)xp.Member;

                        var name1 = GetCollumnName(propertyInfo1);

                        value = GetTableName(propertyInfo1.DeclaringType, xp) + "." + name1;
                    }
                }

                if (xp.Type == typeof(IsNull))
                {
                    value = GetPropertyOfSingleExpression(xp, false, false);
                }

                if (string.IsNullOrEmpty(value))
                {
                    value = TreatValue(GetValue(xp), true)?.ToString();
                }

                value = string.Format(DBKeysConstants.CONVERT_VARCHAR, value);
                list.Add(value);
            }

            return string.Format(DBKeysConstants.CONCAT, string.Join(",", list));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="isConcatValue"></param>
        /// <returns></returns>
        protected string ValidateValueByMethod(dynamic obj, string methodName, bool isConcatValue)
        {
            string valorTratado = TreatValue(obj.value, true)?.ToString();
            if (DBQueryConstants.LIKE_FUNCTION.Equals(methodName) || DBQueryConstants.NOT_LIKE_FUNCTION.Equals(methodName))
            {
                if (!obj.isValue || isConcatValue)
                {
                    return string.Format(DBKeysConstants.CONCAT, $"'%', {obj.value} ,'%'");
                }
                return obj.value.ToString().Contains("%") ? $"'{obj.value}'" : $"'%{obj.value}%'";
            }
            else if (DBQueryConstants.IN_FUNCTION.Equals(methodName)
                    || DBQueryConstants.NOT_IN_FUNCTION.Equals(methodName))
            {
                return obj.value.ToString();
            }
            else
            {
                return valorTratado;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        protected string TreatComparerByMethod(string methodName)
        {
            if (DBQueryConstants.LIKE_FUNCTION.Equals(methodName))
            {
                return DBKeysConstants.LIKE;
            }
            else
            if (DBQueryConstants.NOT_LIKE_FUNCTION.Equals(methodName))
            {
                return DBKeysConstants.NOT_LIKE;
            }
            else if (DBQueryConstants.IN_FUNCTION.Equals(methodName))
            {
                return DBKeysConstants.IN;
            }
            else if (DBQueryConstants.NOT_IN_FUNCTION.Equals(methodName))
            {
                return DBKeysConstants.NOT_IN;
            }
            else if (DBQueryConstants.EQUALS_FUNCTION.Equals(methodName))
            {
                return DBKeysConstants.EQUALS;
            }
            else if (DBQueryConstants.CONTAINS.Equals(methodName))
            {
                return DBKeysConstants.LIKE_VALUE;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        protected dynamic GetValue(Expression right)
        {
            if (right is ConstantExpression)
            {
                var rightConst = right as ConstantExpression;
                return rightConst.Value;
            }
            if (right is MemberExpression)
            {
                var objectMember = Expression.Convert(right, typeof(object));

                var getterLambda = Expression.Lambda<Func<object>>(objectMember);

                var getter = getterLambda.Compile();

                return getter();
            }
            if (right is MethodCallExpression)
            {
                dynamic r = right;
                if (ContainsProperty(r, "Method") &&
                    (DBQueryConstants.LIKE_FUNCTION.Equals(r.Method.Name) || DBQueryConstants.IN_FUNCTION.Equals(r.Method.Name)
                        || DBQueryConstants.NOT_IN_FUNCTION.Equals(r.Method.Name) || DBQueryConstants.CONCAT_FUNCTION.Equals(r.Method.Name)
                        || DBQueryConstants.SQL_FUNCTION.Equals(r.Method.Name) || DBQueryConstants.NOT_LIKE_FUNCTION.Equals(r.Method.Name)
                    ))
                {
                    return ExtractMethod(right);
                }
                else
                {
                    return Expression.Lambda(right).Compile().DynamicInvoke();
                }
            }
            if (right is UnaryExpression)
            {
                var uExp = right as UnaryExpression;
                if (uExp.Operand is ConstantExpression)
                {
                    var rightConst = uExp.Operand as ConstantExpression;
                    return rightConst.Value;
                }
                if (uExp.Operand is MemberExpression)
                {
                    var objectMember = Expression.Convert(right, typeof(object));

                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);

                    var getter = getterLambda.Compile();

                    return getter();
                }
                if (uExp.Operand is MethodCallExpression)
                {
                    object result = Expression.Lambda(right).Compile().DynamicInvoke();
                    return result;
                }
            }
            if (right != null && ContainsProperty(right, "NodeType") && right.NodeType == ExpressionType.Call)
            {
                return Expression.Lambda(right).Compile().DynamicInvoke();
            }
            else if (right != null && ContainsProperty(right, "NodeType") && (right.NodeType == ExpressionType.ConvertChecked || right.NodeType == ExpressionType.Convert))
            {
                return Expression.Lambda(right).Compile().DynamicInvoke();
            }
            else if (right != null && ContainsProperty(right, "NodeType") && right.NodeType == ExpressionType.Lambda)
            {
                return Expression.Lambda(right).Compile().DynamicInvoke();
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected bool IsValue(dynamic expression)
        {
            if (expression == null) { return true; }
            var right = expression;
            dynamic r = right;
            dynamic exp = _currentExpression;
            bool isValue = true;

            if (_currentExpression != null && ContainsProperty(exp, "Parameters") && exp.Parameters != null)
            {
                foreach (var e in exp.Parameters)
                {
                    if (ContainsProperty(r, "NodeType") && r.NodeType != null && r.NodeType == ExpressionType.Not
                        && ContainsProperty(r, "Operand"))
                    {
                        r = r.Operand;
                    }
                    if (ContainsProperty(r, "Expression") && r.Expression != null && ContainsProperty(r.Expression, "Member"))
                    {
                        if (r.Expression.Member != null && e.Name == r.Expression.Member.Name)
                        {
                            isValue = false;
                            break;
                        }
                        if (ContainsProperty(r, "Expression") && r.Expression != null && ContainsProperty(r.Expression, "Member")
                            && (r.Member.Name == "Value" || r.Member.Name == "HasValue") && ContainsProperty(r.Expression, "Expression") && r.Expression.Expression != null)
                        {
                            if (ContainsProperty(r.Expression.Expression, "Name") && r.Expression.Expression.Name != null && e.Name == r.Expression.Expression.Name)
                            {
                                isValue = false;
                                break;
                            }
                        }
                    }
                    else if (ContainsProperty(r, "Expression") && r.Expression != null && ContainsProperty(r.Expression, "Name"))
                    {
                        if (e.Name == r.Expression.Name)
                        {
                            isValue = false;
                            break;
                        }
                    }
                }
            }

            return isValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool VerififyNativesMethods(MethodCallExpression method, ref string value)
        {
            string valueA = string.Empty;
            string valueB = string.Empty;
            string comparador = string.Empty;
            dynamic mtd = method;
            if (method.Method.Name.Equals(DBQueryConstants.EQUALS_FUNCTION))
            {
                if (IsValue(mtd.Object))
                {
                    valueA = TreatValue(GetValue(mtd.Object), true)?.ToString();
                }
                else
                {
                    valueA = string.Concat(GetTableName(mtd.Object.Member.DeclaringType, mtd), DBKeysConstants.SINGLE_POINT, mtd.Object.Member.Name);
                }

                comparador = TreatComparerByMethod(mtd.Method.Name);

                dynamic arg = mtd.Arguments[0];

                if (IsValue(arg))
                {
                    valueB = TreatValue(GetValue(arg), true)?.ToString();
                }
                else
                {
                    if (ContainsProperty(arg, "Expression") && ContainsProperty(arg, "Type"))
                    {
                        valueB = string.Concat(GetTableName(arg.Expression.Type, mtd), DBKeysConstants.SINGLE_POINT, GetCollumnName(arg.Member));
                    }
                }
                value = string.Format("{0} {1} {2}", valueA, comparador, valueB);
                return true;
            }
            else if (method.Method.Name.Equals(DBQueryConstants.CONTAINS))
            {
                if (IsValue(mtd.Object))
                {
                    valueA = TreatValue(GetValue(mtd.Object), true)?.ToString();
                }
                else
                {
                    valueA = string.Concat(GetTableName(mtd.Object.Member.DeclaringType, mtd), DBKeysConstants.SINGLE_POINT, mtd.Object.Member.Name);
                }

                comparador = TreatComparerByMethod(mtd.Method.Name);

                dynamic arg = mtd.Arguments[0];

                if (IsValue(arg))
                {
                    valueB = TreatValue(GetValue(arg), false)?.ToString();
                }
                else
                {
                    if (ContainsProperty(arg, "Expression") && ContainsProperty(arg, "Type"))
                    {
                        valueB = string.Concat(GetTableName(arg.Expression.Type, mtd), DBKeysConstants.SINGLE_POINT, GetCollumnName(arg.Member));
                    }
                }
                value = string.Format("{0} {1}", valueA, string.Format(comparador, valueB));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetComparador(Expression expression)
        {
            string equalty = string.Empty;
            if (expression.NodeType == ExpressionType.Equal)
            {
                equalty = DBKeysConstants.EQUALS;
            }
            else if (expression.NodeType == ExpressionType.AndAlso)
            {
                equalty = DBKeysConstants.AND;
            }
            else if (expression.NodeType == ExpressionType.OrElse)
            {
                equalty = DBKeysConstants.OR;
            }
            else if (expression.NodeType == ExpressionType.NotEqual)
            {
                equalty = DBKeysConstants.NOT_EQUAL;
            }
            else if (expression.NodeType == ExpressionType.GreaterThan)
            {
                equalty = DBKeysConstants.GREATER_THAN;
            }
            else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                equalty = DBKeysConstants.GREATER_THAN_OR_EQUAL;
            }
            else if (expression.NodeType == ExpressionType.LessThan)
            {
                equalty = DBKeysConstants.LESS_THAN; ;
            }
            else if (expression.NodeType == ExpressionType.LessThanOrEqual)
            {
                equalty = DBKeysConstants.LESS_THAN_OR_EQUAL;
            }
            else if (expression.NodeType == ExpressionType.Multiply)
            {
                equalty = DBKeysConstants.MULTIPLY;
            }
            else if (expression.NodeType == ExpressionType.Divide)
            {
                equalty = DBKeysConstants.DIVIDE;
            }
            else if (expression.NodeType == ExpressionType.Subtract)
            {
                equalty = DBKeysConstants.SUBSTRACT;
            }
            else if (expression.NodeType == ExpressionType.Add)
            {
                equalty = DBKeysConstants.ADD;
            }
            return equalty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected Expression GetLeftNode(Expression expression)
        {
            dynamic exp = expression;
            var left = exp.Left;
            if (left != null && (left.NodeType == ExpressionType.Convert || left.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(left, "Operand"))
            {
                left = left.Operand;
                if ((left.NodeType == ExpressionType.Convert || left.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(left, "Operand"))
                {
                    return left.Operand;
                }
                return left;
            }
            return (Expression)exp.Left;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected Expression GetRightNode(Expression expression)
        {
            dynamic exp = expression;
            var right = exp.Right;
            if (right != null && (right.NodeType == ExpressionType.Convert || right.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(right, "Operand"))
            {
                right = right.Operand;
                if ((right.NodeType == ExpressionType.Convert || right.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(right, "Operand"))
                {
                    return right.Operand;
                }
                return right;
            }
            return (Expression)exp.Right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="useQuotes"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object TreatValue(dynamic val, bool useQuotes = false, dynamic expression = null)
        {
            if (expression != null)
            {
                if (expression is MethodCallExpression)
                {
                    if (DBQueryConstants.CONCAT_FUNCTION.Equals(expression.Method.Name))
                    {
                        return val;
                    }
                }
            }

            if (val == null || val.GetType() != typeof(string) && string.IsNullOrEmpty(val?.ToString()) && useQuotes)
            {
                return DBKeysConstants.NULL;
            }

            if (val.GetType() == typeof(DateTime))
            {
                return useQuotes ? $"'{val.ToString(DBKeysConstants.DATE_FORMAT)}'" : val.ToString(DBKeysConstants.DATE_FORMAT);
            }
            if (val.GetType() == typeof(DateTimeOffset))
            {
                return useQuotes ? $"'{val.ToString(DBKeysConstants.DATE_OFF_SET_FORMAT)}'" : val.ToString(DBKeysConstants.DATE_OFF_SET_FORMAT);
            }
            else if (val.GetType() == typeof(bool))
            {
                return val ? 1 : 0;
            }
            else if (val.GetType() == typeof(decimal))
            {
                return val?.ToString(DBKeysConstants.DECIMAL_FORMAT)?.Replace(",", ".");
            }
            else if (val.GetType() == typeof(int) || val.GetType() == typeof(decimal))
            {
                return val;
            }
            else
            {
                val = val.ToString().Replace("'", "''");
                return useQuotes ? $"'{val}'" : val;
            }
        }
        #endregion

        #region Validate Domain 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        protected bool IsValid(EntityBase domain)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ValidationContext _context = new ValidationContext(domain);

            IList<ValidationResult> validationResults = new List<ValidationResult>();
            if (domain != null)
            {
                if (!Validator.TryValidateObject(domain, _context, validationResults, true))
                {
                    if (validationResults.Count == 1)
                    {
                        stringBuilder.AppendLine("Por favor revise o formulário. Foi encontrado o seguinte erro:");
                    }
                    else
                    {
                        stringBuilder.AppendLine("Por favor revise o formulário. Foram encontrados os seguintes erros:");
                    }
                    stringBuilder.AppendLine("");

                    for (var i = 1; i <= validationResults.Count(); i++)
                    {
                        ValidationResult result = validationResults[i - 1];
                        if (result == validationResults.Last())
                        {
                            stringBuilder.Append("- " + result.ErrorMessage);
                        }
                        else
                        {
                            stringBuilder.AppendLine("- " + result.ErrorMessage);
                        }
                    }

                    if (validationResults.Any())
                    {
                        throw new Exception(stringBuilder.ToString());
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
