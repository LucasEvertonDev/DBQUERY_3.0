using DB.Query.Core.Annotations;
using DB.Query.Core.Annotations.Entity;
using DB.Query.Core.Contants;
using DB.Query.Core.Entities;
using DB.Query.Core.Enuns;
using DB.Query.Core.Models;
using DB.Query.InterpretCode.Services.Others;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DB.Query.InterpretCode.Services.InterpretServices
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InterpretService<TEntity> where TEntity : EntityBase
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
        protected bool ContainsProperty(object obj, string name) => MemoryCacheService.ContainsProperty(obj.GetType(), name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetFullName(Type type) => string.Concat(GetDatabaseName(type), DBKeysConstants.T_A, GetTableName(type));
        /// <summary>
        /// 
        /// </summary>
        protected static readonly List<string> _defaultFunctions = new List<string>
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
            DBQueryConstants.NAME_FUNCTION,
            DBQueryConstants.CASE_SQL_FUNCTION,
            DBQueryConstants.IS_NULL_SQL_FUNCTION,
            DBQueryConstants.IIF_SQL_FUNCTION,
            DBQueryConstants.CONVERT_SQL_FUNCTION,
            DBQueryConstants.STRING_AGG_SQL_FUNCTION,
            DBQueryConstants.CONCAT_SQL_FUNCTION,
            DBQueryConstants.SUM_SQL_FUNCTION,
        };


        protected static readonly Dictionary<string, Func<dynamic, string>> SQL_FUNCTIONS = new Dictionary<string, Func<dynamic, string>>
        {
            {
                DBQueryConstants.SQL_FUNCTION,
                (arguments) =>
                {
                    return $"({Expression.Lambda(arguments[0]).Compile().DynamicInvoke()})";
                }
            },
            {
                DBQueryConstants.CASE_SQL_FUNCTION,
                (arguments) =>
                {
                    return $"(CASE {string.Join(" ", Expression.Lambda(arguments[0]).Compile().DynamicInvoke())} END)";
                }
            },
            {
                DBQueryConstants.IS_NULL_SQL_FUNCTION,
                (arguments) =>
                {
                    return  $"ISNULL({Expression.Lambda(arguments[0]).Compile().DynamicInvoke()}, {Expression.Lambda(arguments[1]).Compile().DynamicInvoke()})";
                }
            },
            {
                DBQueryConstants.IIF_SQL_FUNCTION,
                (arguments) =>
                {
                    return  $"IIF({Expression.Lambda(arguments[0]).Compile().DynamicInvoke()}, {Expression.Lambda(arguments[1]).Compile().DynamicInvoke()}, {Expression.Lambda(arguments[2]).Compile().DynamicInvoke()})";
                }
            },
            {
                DBQueryConstants.CONVERT_SQL_FUNCTION,
                (arguments) =>
                {
                    return  $"CONVERT({Expression.Lambda(arguments[0]).Compile().DynamicInvoke()}, {Expression.Lambda(arguments[1]).Compile().DynamicInvoke()})";
                }
            },
            {
                DBQueryConstants.STRING_AGG_SQL_FUNCTION,
                (arguments) =>
                {
                    return  $"STRING_AGG({Expression.Lambda(arguments[0]).Compile().DynamicInvoke()}, {Expression.Lambda(arguments[1]).Compile().DynamicInvoke()})";
                }
            },
            {
                DBQueryConstants.CONCAT_SQL_FUNCTION,
                (arguments) =>
                {
                    return  $"CONCAT({Expression.Lambda(arguments[0]).Compile().DynamicInvoke()}, {Expression.Lambda(arguments[1]).Compile().DynamicInvoke()})";
                }
            },
            {
                DBQueryConstants.SUM_SQL_FUNCTION,
                (arguments) =>
                {
                    return  $"SUM({Expression.Lambda(arguments[0]).Compile().DynamicInvoke()})";
                }
            }
        };

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
        protected void AddGroupBy(Expression expressions, StringBuilder queryBuilder)
        {
            // Verifica se a consulta já contém a cláusula GROUP BY
            bool containsGroupBy = queryBuilder.ToString().Contains(DBKeysConstants.GROUP_BY);

            // Se a cláusula GROUP BY não estiver presente, adiciona uma nova cláusula
            if (!containsGroupBy)
            {
                queryBuilder.Append(DBKeysConstants.GROUP_BY_WITH_SPACE);
            }

            // Se houver expressões fornecidas para agrupar
            if (expressions != null)
            {
                // Obtém as propriedades a partir das expressões
                var properties = GetPropertiesExpression(expressions);

                // Se a consulta já contém GROUP BY e há propriedades a serem adicionadas
                if (containsGroupBy && properties.Count > 0)
                {
                    // Adiciona as propriedades separadas por vírgula
                    queryBuilder.Append(", " + string.Join(", ", properties));
                }
                else
                {
                    // Se GROUP BY não estava presente, adiciona as propriedades diretamente
                    queryBuilder.Append(string.Join(", ", properties));
                }
            }
        }


        /// <summary>
        /// Adiciona cláusulas ORDER BY a uma string de consulta com base nas expressões fornecidas.
        /// </summary>
        /// <param name="tipo">O tipo de ordenação (ascendente ou descendente).</param>
        /// <param name="expressions">As expressões para ordenar.</param>
        /// <param name="queryBuilder">O StringBuilder da consulta atual.</param>
        /// <param name="adicionadaOrdenacao">Indica se a ordenação já foi adicionada.</param>
        /// <returns>A string da consulta atualizada com as cláusulas ORDER BY.</returns>
        protected void AddOrderBy(string tipo, Expression expressions, StringBuilder queryBuilder, bool adicionadaOrdenacao)
        {
            // Se a ordenação ainda não foi adicionada, adiciona a cláusula ORDER BY
            if (!adicionadaOrdenacao)
            {
                queryBuilder.Append(DBKeysConstants.ORDER_BY_WITH_SPACE);
            }

            // Se houver expressões fornecidas para ordenar
            if (expressions != null)
            {
                // Obtém as propriedades a partir das expressões
                var properties = GetPropertiesExpression(expressions);

                // Se a ordenação já foi adicionada e há propriedades a serem incluídas
                if (adicionadaOrdenacao && properties.Count > 0)
                {
                    // Adiciona as propriedades separadas por vírgula, mantendo o tipo de ordenação
                    queryBuilder.Append(", " + string.Join($" {tipo}, ", properties) + $" {tipo}");
                }
                else
                {
                    // Se a ordenação não foi adicionada, adiciona as propriedades diretamente
                    queryBuilder.Append(string.Join($" {tipo}, ", properties) + $" {tipo}");
                }
            }
        }

        /// <summary>
        /// Adiciona cláusulas WHERE a uma consulta com base na expressão fornecida.
        /// </summary>
        /// <param name="expression">A expressão que contém as condições WHERE.</param>
        /// <returns>A string da condição WHERE formada a partir da expressão.</returns>
        protected string AddWhere(Expression expression)
        {
            var conditionBuilder = new StringBuilder(); // Usando StringBuilder para melhor performance

            // Se a expressão fornecida não for nula
            if (expression != null)
            {
                string query = TranslateLambda(rawExpression: expression, expression: expression);
                // Adiciona a cláusula WHERE à condição
                conditionBuilder.Append(DBKeysConstants.WHERE_WITH_SPACE).Append(query);
            }

            // Retorna a condição WHERE formada
            return conditionBuilder.ToString(); // Retorna a string formada
        }


        /// <summary>
        /// Adiciona cláusulas WHERE a uma consulta com base na expressão fornecida.
        /// </summary>
        /// <param name="expression">A expressão que contém as condições WHERE.</param>
        /// <param name="conditionBuilder"></param>
        /// <returns>A string da condição WHERE formada a partir da expressão.</returns>
        protected void AddWhere(Expression expression, StringBuilder conditionBuilder)
        {
            // Se a expressão fornecida não for nula
            if (expression != null)
            {
                string query = TranslateLambda(rawExpression: expression, expression: expression);
                // Adiciona a cláusula WHERE à condição
                conditionBuilder.Append(DBKeysConstants.WHERE_WITH_SPACE).Append(query);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawExpression">Arvore completa utilizada validar valores</param>
        /// <param name="expression">a ser traduzida</param>
        /// <param name="dontUseLambdaParentheses"></param>
        /// <returns></returns>
        private string TranslateLambda(Expression rawExpression, Expression expression, bool dontUseLambdaParentheses = false)
        {
            // Inicializa a parte da expressão
            dynamic expressionPart = ContainsProperty(expression, "Body") ? ((dynamic)expression).Body : expression;

            // Converte a parte da expressão em string
            StringBuilder lambdaBuilder = new StringBuilder(expressionPart.ToString());

            // Obtém os parâmetros da expressão lambda
            ReadOnlyCollection<ParameterExpression> parameters = rawExpression != null && ContainsProperty(rawExpression, "Parameters")
                ? ((dynamic)rawExpression).Parameters
                : new ReadOnlyCollection<ParameterExpression>(new List<ParameterExpression>());

            // Processa a expressão para extrair partes relevantes
            if (dontUseLambdaParentheses)
            {
                return DememberExpression(parameters: parameters, expression: expressionPart, replaceLambda: false, lambdaText: ref lambdaBuilder);
            }

            DememberExpression(parameters: parameters, expression: expressionPart, replaceLambda: true, lambdaText: ref lambdaBuilder);

            // Substitui palavras-chave da consulta
            lambdaBuilder.Replace(DBQueryConstants.AND_ALSO, DBKeysConstants.AND);
            lambdaBuilder.Replace(DBQueryConstants.OR_ELSE, DBKeysConstants.OR);
            lambdaBuilder.Replace(DBQueryConstants.EQUALS, DBKeysConstants.EQUALS);

            return lambdaBuilder.ToString(); // Retorna a expressão traduzida
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">Arvore completa utilizada validar valores</param>
        /// <param name="expression">a ser traduzida</param>
        /// <param name="dontUseLambdaParentheses"></param>
        /// <returns></returns>
        private string TranslateLambda(ReadOnlyCollection<ParameterExpression> parameters, Expression expression, bool dontUseLambdaParentheses = false)
        {
            dynamic expressionPart = ContainsProperty(expression, "Body") ? ((dynamic)expression).Body : expression;

            var lambdaText = new StringBuilder(expressionPart.ToString());

            if (dontUseLambdaParentheses)
            {
                return DememberExpression(parameters: parameters, expression: expressionPart, lambdaText: ref lambdaText, replaceLambda: false);
            }

            DememberExpression(parameters: parameters, expression: expressionPart, lambdaText: ref lambdaText, replaceLambda: true);

            // Usando um método separado para aplicar as substituições
            lambdaText.Replace(DBQueryConstants.AND_ALSO, DBKeysConstants.AND);
            lambdaText.Replace(DBQueryConstants.OR_ELSE, DBKeysConstants.OR);
            lambdaText.Replace(DBQueryConstants.EQUALS, DBKeysConstants.EQUALS);

            return lambdaText.ToString();
        }

        /// <summary>
        /// Adiciona cláusulas JOIN a uma consulta com base na expressão fornecida.
        /// </summary>
        /// <param name="expression">A expressão que contém as informações do JOIN.</param>
        /// <param name="queryBuilder"></param>
        /// <param name="strJoin">A string de formato para o tipo de JOIN.</param>
        /// <returns>A string da cláusula JOIN formada a partir da expressão.</returns>
        protected void AddJoin(Expression expression, StringBuilder queryBuilder, string strJoin)
        {
            // Obtém o nome completo do tipo da segunda parâmetro da expressão, com ou sem alias
            var aux = _useAlias
                ? string.Concat(GetFullName(((dynamic)expression).Parameters[1].Type), DBKeysConstants.AS_WITH_SPACE, ((dynamic)expression).Parameters[1].Name)
                : GetFullName(((dynamic)expression).Parameters[1].Type);

            // Formata a string de JOIN com o nome completo e a parte extra da expressão
            queryBuilder.AppendFormat(strJoin, aux, TranslateLambda(rawExpression: expression, expression, true));
        }

        /// <summary>
        /// Adiciona cláusulas JOIN específicas para a relação entre Empresa e Filial.
        /// </summary>
        /// <param name="expression">A expressão que contém as informações do JOIN.</param>
        /// <param name="queryBuilder"></param>
        /// <param name="strJoin">A string de formato para o tipo de JOIN.</param>
        /// <returns>A string da cláusula JOIN formada a partir da expressão.</returns>
        protected void AddJoinEmpresaFilial(Expression expression, StringBuilder queryBuilder, string strJoin)
        {
            // Obtém o nome completo do tipo do segundo parâmetro da expressão, com ou sem alias
            var aux = _useAlias
                ? string.Concat(GetFullName(((dynamic)expression).Parameters[1].Type), DBKeysConstants.AS_WITH_SPACE, ((dynamic)expression).Parameters[1].Name)
                : GetFullName(((dynamic)expression).Parameters[1].Type);

            // Formata a string de JOIN com o nome completo e a parte extra da expressão
            queryBuilder.AppendFormat(strJoin, aux, TranslateLambda(rawExpression: expression, expression: expression, true));

            // Adiciona cláusulas específicas para a relação Empresa e Filial
            queryBuilder.Append(AddClausesEmpresaFilial(GetTableName(((dynamic)expression).Parameters[0].Type), GetTableName(((dynamic)expression).Parameters[1].Type)));
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
        /// Obtém o nome da tabela associado a um tipo, com suporte para aliases.
        /// </summary>
        /// <param name="type">O tipo do qual se deseja obter o nome da tabela.</param>
        /// <param name="exp">Uma expressão opcional que pode fornecer informações adicionais.</param>
        /// <returns>O nome da tabela correspondente ao tipo.</returns>
        protected virtual string GetTableName(Type type, dynamic exp = null)
        {
            // Verifica se deve usar alias e se a expressão não é nula
            if (_useAlias && exp != null)
            {
                // Se a expressão possui uma propriedade "Expression", retorna seu nome
                if (ContainsProperty(exp, "Expression"))
                {
                    return exp.Expression.Name;
                }
                // Se a expressão é uma chamada e possui argumentos
                if (exp.NodeType == ExpressionType.Call && ContainsProperty(exp, "NodeType") && exp.Arguments.Count > 0)
                {
                    return exp.Arguments[0].Expression.Name; // Retorna o nome do primeiro argumento
                }
                // Se a expressão é uma chamada sem argumentos
                if (exp.NodeType == ExpressionType.Call && ContainsProperty(exp, "NodeType") && exp.Arguments.Count == 0)
                {
                    return exp.Object.Name; // Retorna o nome do objeto
                }
                // Se a expressão é um parâmetro, retorna seu nome
                if (exp.NodeType == ExpressionType.Parameter)
                {
                    return exp.Name;
                }
            }

            // Obtém o atributo de tabela associado ao tipo
            var displayName = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

            // Retorna o nome da tabela se o atributo existir; caso contrário, retorna o nome do tipo
            return displayName != null ? displayName.TableName : type.Name;
        }

        /// <summary>
        /// Obtém o nome do banco de dados associado a um tipo, se especificado através de atributos.
        /// </summary>
        /// <param name="type">O tipo do qual se deseja obter o nome do banco de dados.</param>
        /// <returns>O nome do banco de dados associado ao tipo.</returns>
        public string GetDatabaseName(Type type)
        {
            // Tenta obter o atributo DatabaseAttribute associado ao tipo
            var displayName = type.GetCustomAttributes(typeof(DatabaseAttribute), true).FirstOrDefault() as DatabaseAttribute;

            // Retorna o nome do banco de dados especificado pelo atributo
            return displayName.DatabaseName;
        }


        //// <summary>
        /// Obtém o nome completo de uma propriedade, concatenando o nome da tabela e o nome da coluna.
        /// </summary>
        /// <param name="type">O tipo do qual se deseja obter o nome da tabela.</param>
        /// <param name="exp">A expressão que contém informações sobre a propriedade.</param>
        /// <param name="member">O membro opcional cujos detalhes devem ser usados.</param>
        /// <returns>O nome completo da propriedade no formato "Tabela.Coluna".</returns>
        protected string GetPropertyFullName(Type type, dynamic exp, dynamic member = null)
        {
            // Obtém o nome da tabela e o nome da coluna, concatenando-os
            return string.Concat(
                GetTableName(type, exp),
                DBKeysConstants.SINGLE_POINT,
                GetColumnName(member ?? exp.Member)
            );
        }

        /// <summary>
        /// Obtém o nome da coluna associado a uma propriedade.
        /// </summary>
        /// <param name="prop">A propriedade da qual se deseja obter o nome da coluna.</param>
        /// <returns>O nome da coluna correspondente à propriedade.</returns>
        protected string GetColumnName(PropertyInfo prop)
        {
            // Obtém o nome da coluna utilizando o serviço de cache em memória
            return MemoryCacheService.GetDisplayName(prop);
        }

        /// <summary>
        /// Obtém uma lista de propriedades a partir de uma expressão.
        /// </summary>
        /// <param name="expression">A expressão da qual se deseja extrair as propriedades.</param>
        /// <param name="useAlias">Indica se deve usar alias ao obter as propriedades.</param>
        /// <param name="fromSelect">Indica se a chamada vem de uma seleção personalizada.</param>
        /// <returns>Uma lista de nomes de propriedades extraídas da expressão.</returns>
        protected List<string> GetPropertiesExpression(Expression expression, bool useAlias = false, bool fromSelect = false)
        {
            var properties = new List<string>();
            dynamic exp = expression;

            ReadOnlyCollection<ParameterExpression> parameters = exp != null && ContainsProperty(exp, "Parameters")
                 ? exp.Parameters
                 : new ReadOnlyCollection<ParameterExpression>(new List<ParameterExpression>());

            // Verifica se a expressão é uma função que retorna um tipo dinâmico
            if (expression.Type == typeof(Func<TEntity, dynamic>) &&
                (!ContainsProperty(exp, "Body") || exp.Body.Type != typeof(object[])) &&
                (!ContainsProperty(exp, "Body") || exp.Body.NodeType != ExpressionType.New && exp.Body.NodeType != ExpressionType.MemberInit))
            {
                properties.Add(GetPropertyOfSingleExpression(parameters: parameters, expression, false, useAlias));
            }
            else
            {
                // Se a expressão é do tipo "New"
                if (exp.Body.NodeType == ExpressionType.New)
                {
                    if (fromSelect)
                    {
                        _levelModels.FirstOrDefault(A => A.StepType == StepType.CUSTOM_SELECT).ReturnType = exp.Body.Type;
                    }

                    for (var i = 0; i < exp.Body.Arguments.Count; i++)
                    {
                        properties.Add(GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Body.Arguments[i], hasParameter: false, useAlias: useAlias,
                            aliasName: fromSelect ? exp.Body.Members[i].Name : null));
                    }
                }
                // Se a expressão é do tipo "MemberInit"
                else if (exp.Body.NodeType == ExpressionType.MemberInit)
                {
                    if (fromSelect)
                    {
                        _levelModels.FirstOrDefault(A => A.StepType == StepType.CUSTOM_SELECT).ReturnType = exp.Body.Type;
                    }

                    foreach (var binding in exp.Body.Bindings)
                    {
                        properties.Add(GetPropertyOfSingleExpression(parameters: parameters, expression: binding.Expression, hasParameter: false, useAlias: useAlias,
                            aliasName: fromSelect ? binding.Member.Name : null));
                    }
                }
                // Se a expressão tem argumentos
                else if (ContainsProperty(exp.Body, "Arguments") && exp.Body.Arguments.Count > 0 && ContainsProperty(exp.Body.Arguments[0], "Expressions"))
                {
                    foreach (var arg in exp.Body.Arguments[0].Expressions)
                    {
                        properties.Add(GetPropertyOfSingleExpression(parameters: parameters, expression: arg, hasParameter: false, useAlias: useAlias));
                    }
                }
                else
                {
                    properties.Add(GetPropertyOfSingleExpression(parameters: parameters, expression: exp, hasParameter: false, useAlias: useAlias));
                }
            }

            return properties;
        }

        /// <summary>
        /// Obtém a representação de uma única expressão de propriedade.
        /// </summary>
        /// <param name="parameters">Expressao bruta.</param>
        /// <param name="expression">A expressão da qual se deseja obter a propriedade.</param>
        /// <param name="hasParameter">Indica se a expressão possui parâmetros.</param>
        /// <param name="useAlias">Indica se deve usar um alias na representação.</param>
        /// <param name="aliasName">O nome do alias a ser utilizado, se aplicável.</param>
        /// <returns>A representação da propriedade como string.</returns>
        protected string GetPropertyOfSingleExpression(ReadOnlyCollection<ParameterExpression> parameters, dynamic expression, bool hasParameter, bool useAlias, string aliasName = "")
        {
            // Retorna string vazia se a expressão for nula.
            if (expression == null) return string.Empty;

            string result = string.Empty;
            var exp = ContainsProperty(expression, "Body") ? expression.Body : expression;

            // Processa a expressão com base no tipo do nó.
            switch (exp.NodeType)
            {
                case ExpressionType.MemberAccess:
                    result = GetMemberExpression(exp);
                    break;

                case ExpressionType.Constant:
                    result = TreatValue(exp.Value, true)?.ToString();
                    break;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    result = HandleConvertExpression(parameters: parameters, exp: exp);
                    break;

                case ExpressionType.Call:
                    result = HandleMethodCallExpression(parameters: parameters, exp: exp, hasParameter: hasParameter);
                    break;

                default:
                    if (ContainsProperty(exp, "Left"))
                    {
                        result = TranslateLambda(parameters: parameters, expression: exp);
                    }
                    break;
            }

            // Adiciona o alias, se fornecido
            if (!string.IsNullOrEmpty(aliasName) && !(result?.Contains(DBKeysConstants.DOT_ALL_COLUMNS)).GetValueOrDefault())
            {
                result = string.Concat(result, DBKeysConstants.AS_WITH_SPACE, aliasName);
            }

            return result ?? string.Empty;
        }

        /// <summary>
        /// Trata diferentes formas de conversão de expressões.
        /// </summary>
        ///  <param name="parameters">bruta.</param>
        /// <param name="exp">A expressão de conversão a ser tratada.</param>
        /// <returns>A representação da propriedade convertida como string.</returns>
        private string HandleConvertExpression(ReadOnlyCollection<ParameterExpression> parameters, dynamic exp)
        {
            // Trata diferentes formas de conversão
            if (ContainsProperty(exp.Operand, "Expression"))
            {
                return GetPropertyFullName(exp.Operand.Expression.Type, exp.Operand);
            }
            if (ContainsProperty(exp.Operand, "Left"))
            {
                return TranslateLambda(parameters: parameters, expression: exp.Operand);
            }
            if (ContainsProperty(exp.Operand, "Operand"))
            {
                return TranslateLambda(parameters: parameters, expression: exp.Operand.Operand);
            }
            if (ContainsProperty(exp.Operand, "Arguments"))
            {
                return GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Operand.Arguments[0], hasParameter: false, useAlias: false);
            }
            return exp.Operand.Value?.ToString();
        }

        /// <summary>
        /// Lida com chamadas de método e retorna a representação adequada.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="exp">A expressão da chamada de método.</param>
        /// <param name="hasParameter">Indica se a chamada possui parâmetros.</param>
        /// <returns>A representação da chamada de método como string.</returns>
        private string HandleMethodCallExpression(ReadOnlyCollection<ParameterExpression> parameters, dynamic exp, bool hasParameter)
        {
            // Verifica se é uma função padrão
            if (_defaultFunctions.Exists(f => f.Equals(exp.Method.Name)))
            {
                return HandleDefaultFunctions(parameters: parameters, exp: exp, hasParameter: hasParameter);
            }

            // Trata métodos específicos
            if (DBQueryConstants.TO_STRING.Equals(exp.Method.Name) || DBQueryConstants.TO_STRING_SAFE.Equals(exp.Method.Name))
            {
                return HandleToStringFunction(parameters: parameters, exp: exp);
            }

            if (DBQueryConstants.GET_VALUE_OR_DEFAULT.Equals(exp.Method.Name))
            {
                return GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Object, hasParameter: hasParameter, useAlias: false);
            }

            if (DBQueryConstants.PARSE.Equals(exp.Method.Name) && ContainsProperty(exp, "Arguments"))
            {
                return GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Arguments[0], hasParameter: hasParameter, useAlias: false);
            }

            return string.Empty;
        }

        /// <summary>
        /// Lida com funções padrão e retorna a representação apropriada.
        /// </summary>
        /// <param name="parameters">Arvore de expressão na sua forma mais bruta</param>
        /// <param name="exp">A expressão da função padrão.</param>
        /// <param name="hasParameter">Indica se a função possui parâmetros.</param>
        /// <returns>A representação da função padrão como string.</returns>
        private string HandleDefaultFunctions(ReadOnlyCollection<ParameterExpression> parameters, dynamic exp, bool hasParameter)
        {
            switch (exp.Method.Name)
            {
                case DBQueryConstants.ALL_COLUMNS:
                    return GetTableName(exp.Object.Type ?? typeof(TEntity), exp) + DBKeysConstants.DOT_ALL_COLUMNS;

                case DBQueryConstants.ALIAS_FUNCTION:
                    var aliasResult = GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Arguments[0], hasParameter: hasParameter, useAlias: false);
                    return string.Concat(aliasResult, DBKeysConstants.AS_WITH_SPACE, exp.Arguments[1].Value);

                case DBQueryConstants.ISNULL_FUNCTION:
                    var isNullResult = GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Arguments[0], hasParameter: hasParameter, useAlias: false);
                    var param2 = GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Arguments[1], hasParameter: hasParameter, useAlias: false);
                    return string.Format(DBKeysConstants.ISNULL, isNullResult, param2);

                case DBQueryConstants.COUNT_FUNCTION:
                    return HandleCountFunction(parameters: parameters, exp: exp, hasParameter: hasParameter);

                case DBQueryConstants.CONCAT_FUNCTION:
                    return ConcatFunction(parameters: parameters, exp: exp);

                case var key when SQL_FUNCTIONS.ContainsKey(key):
                    return SQL_FUNCTIONS[key](exp.Arguments);

                case DBQueryConstants.NAME_FUNCTION:
                    return Expression.Lambda(exp).Compile().DynamicInvoke();

                default:
                    return HandleOtherDefaultFunctions(exp);
            }
        }

        /// <summary>
        /// Trata a função COUNT e retorna a representação apropriada.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="exp">A expressão da função COUNT.</param>
        /// <param name="hasParameter">Indica se a função possui parâmetros.</param>
        /// <returns>A representação da função COUNT como string.</returns>
        private string HandleCountFunction(ReadOnlyCollection<ParameterExpression> parameters, dynamic exp, bool hasParameter)
        {
            // Trata a função COUNT
            if (!hasParameter)
            {
                if (exp.Arguments.Count > 0)
                {
                    var countResult = GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Arguments[0], hasParameter: hasParameter, useAlias: false);
                    return string.Format(DBKeysConstants.COUNT_ELEMENT, countResult);
                }
                return DBKeysConstants.COUNT;
            }
            return string.Empty;
        }

        /// <summary>
        /// Trata outras funções padrão e retorna a representação apropriada.
        /// </summary>
        /// <param name="exp">A expressão da função a ser tratada.</param>
        /// <returns>A representação da função como string.</returns>
        private string HandleOtherDefaultFunctions(dynamic exp)
        {
            // Trata outras funções padrão
            dynamic d = ContainsProperty(exp, "Body") ? exp.Body : exp.Arguments[0];

            if (ContainsProperty(d, "Expressions"))
            {
                var aux = d.Expressions[0].Arguments[0].Operand;
                return string.Format(string.Concat(exp.Method.Name, "({0})"), GetPropertyFullName(aux.Expression.Type, aux.Expression, aux.Member));
            }
            else if (ContainsProperty(exp, "Arguments") && ContainsProperty(d, "Expression"))
            {
                return string.Format(string.Concat(exp.Method.Name, "({0})"), GetPropertyFullName(d.Expression.Type, d));
            }

            var operand = d.Operand;
            return string.Format(string.Concat(exp.Method.Name, "({0})"), GetPropertyFullName(operand.Expression.Type, operand.Expression, operand.Member));
        }

        /// <summary>
        /// Lida com a conversão de uma expressão para string.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="exp">A expressão a ser convertida.</param>
        /// <returns>A representação da conversão para string como string.</returns>
        private string HandleToStringFunction(ReadOnlyCollection<ParameterExpression> parameters, dynamic exp)
        {
            // Trata a conversão para string
            if (exp.Object.Type == typeof(string))
            {
                return GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Object, hasParameter: false, useAlias: false);
            }
            return string.Format(DBKeysConstants.CONVERT_VARCHAR, GetPropertyOfSingleExpression(parameters: parameters, expression: exp.Object, hasParameter: false, useAlias: false));
        }

        /// <summary>
        /// Obtém as propriedades de uma expressão ou tipo específico.
        /// </summary>
        /// <param name="exp">A expressão de onde obter as propriedades.</param>
        /// <param name="type">O tipo cujas propriedades devem ser obtidas.</param>
        /// <returns>Uma lista de strings representando as propriedades.</returns>
        protected List<string> GetProperties(dynamic exp = null, Type type = null)
        {
            var list = new List<string>();
            Type currentType = type != null ? type : typeof(TEntity);
            List<PropertyInfo> infs = currentType.GetProperties().ToList();
            var tableName = GetTableName(currentType, exp);

            for (var i = 0; i < infs.Count; i++)
            {
                var prop = infs[i];
                // Ignora propriedades decoradas com IgnoreAttribute.
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
        /// Desmembra uma expressão em seus componentes individuais, convertendo-a em uma representação em string.
        /// </summary>
        /// <param name="parameters">A expressão bruta a ser desmembrada.</param>
        /// <param name="expression">A expressão a ser desmembrada.</param>
        /// <param name="lambdaText"></param>
        /// <param name="replaceLambda"></param>
        /// <returns>Uma string representando a expressão desmembrada.</returns>
        protected string DememberExpression(ReadOnlyCollection<ParameterExpression> parameters, Expression expression, ref StringBuilder lambdaText, bool replaceLambda)
        {
            // Lida com LambdaExpression que contém múltiplos parâmetros, substituindo a expressão pela sua parte do corpo.
            if (expression is LambdaExpression lambda && lambda.Parameters.Count > 1)
            {
                expression = lambda.Body;
            }

            string value = string.Empty;
            string oldExpression = expression.ToString();
            string equalityOperator = ContainsProperty(expression, "NodeType") ? GetComparador(expression) : string.Empty;

            // Obtém o nó esquerdo e direito da expressão, se disponíveis.
            var left = ContainsProperty(expression, "Left") ? GetLeftNode(expression) : null;
            var right = ContainsProperty(expression, "Right") ? GetRightNode(expression) : null;

            // Se houver um nó esquerdo e ele não tiver filhos, trata o nó esquerdo.
            if (left != null && !ContainsProperty(left, "Left") && !ContainsProperty(right, "Left"))
            {
                value = HandleLeftExpression(parameters: parameters, expression: expression, left: left);
            }

            // Se não houver nó esquerdo, trata o caso em que o nó esquerdo é nulo.
            if (left == null)
            {
                value = HandleNullLeftExpression(parameters: parameters, expression: expression);

                if (replaceLambda)
                    ReplaceLastOccurrence(lambdaText, oldExpression, value);

                return value;
            }

            // Se não foi possível obter um valor, tenta desmembrar recursivamente a expressão.
            if (string.IsNullOrEmpty(value))
            {
                value = HandleRecursiveDemember(parameters: parameters, left: left, right: right, equalityOperator: equalityOperator, ref lambdaText, replaceLambda: replaceLambda);
            }
            else
            {
                // Se já há um valor, processa o nó direito da expressão.
                value = HandleRightExpression(parameters: parameters, nodeExpression: expression, rightExpression: right, equalityOperator: equalityOperator, value: value);

                if (replaceLambda)
                    ReplaceLastOccurrence(lambdaText, oldExpression, value);
            }

            return value;
        }

        public static void ReplaceLastOccurrence(StringBuilder text, string search, string replace)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentException("O valor de busca não pode ser nulo ou vazio.", nameof(search));
            }

            // Busca manualmente no StringBuilder
            int length = text.Length;
            int searchLength = search.Length;
            int index = -1;

            for (int i = length - searchLength; i >= 0; i--)
            {
                if (text.ToString(i, searchLength) == search)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                throw new Exception("A ocorrência de busca não foi encontrada.");
            }

            // Cria um novo StringBuilder para a substituição
            StringBuilder sb = new StringBuilder(length - searchLength + replace.Length);
            sb.Append(text, 0, index);  // Parte antes da ocorrência
            sb.Append(replace);          // Adiciona a nova string
            sb.Append(text, index + searchLength, length - (index + searchLength)); // Parte depois da ocorrência

            // Copia o resultado de volta para o texto original
            text.Clear();
            text.Append(sb);
        }


        /// <summary>
        /// Lida com a parte esquerda de uma expressão, formatando-a adequadamente.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="expression">A expressão original.</param>
        /// <param name="left">O nó esquerdo da expressão.</param>
        /// <returns>A string formatada da expressão esquerda.</returns>
        private string HandleLeftExpression(ReadOnlyCollection<ParameterExpression> parameters, Expression expression, Expression left)
        {
            string value = string.Empty;

            // Verifica se a expressão é booleana (And/Or).
            if (expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse)
            {
                var booleanExpression = TreatBooleanPropertyExpression(parameters, left);
                if (!string.IsNullOrEmpty(booleanExpression?.Trim()))
                {
                    // Formata a expressão booleana.
                    value = string.Format("{0} {1} {2}", booleanExpression, "{0}", "{1}");
                }
            }
            else
            {
                // Lê a expressão do nó esquerdo.
                var result = ReadExpression(parameters, left);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    value = string.Format("{0} {1} {2}", result, "{0}", "{1}");
                }
            }

            return value;
        }

        /// <summary>
        /// Lida com o caso em que o nó esquerdo da expressão é nulo.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="expression">A expressão original.</param>
        /// <returns>A string resultante ao tratar o nó esquerdo nulo.</returns>
        private string HandleNullLeftExpression(ReadOnlyCollection<ParameterExpression> parameters, Expression expression)
        {
            string value = string.Empty;

            // Trata a expressão booleana se existir.
            var booleanExpression = TreatBooleanPropertyExpression(parameters, expression);
            if (!string.IsNullOrEmpty(booleanExpression?.Trim()))
            {
                value = booleanExpression;
            }
            else
            {
                // Extrai um método da expressão, se possível.
                var methodExpression = ExtractMethod(parameters: parameters, method: expression);
                value = string.IsNullOrEmpty(methodExpression?.Trim()) ? ReadExpression(parameters: parameters, partExpression: expression) : methodExpression;
            }

            return value;
        }

        /// <summary>
        /// Lida com a desmembragem recursiva de uma expressão, combinando os valores esquerdo e direito.
        /// </summary>
        /// <param name="parameters">Arvore de expressão na sua forma bruta.</param>
        /// <param name="left">O nó esquerdo da expressão.</param>
        /// <param name="right">O nó direito da expressão.</param>
        /// <param name="equalityOperator">O operador de igualdade a ser utilizado.</param>
        /// <param name="lambdaText"></param>
        /// <param name="replaceLambda"></param>
        /// <returns>A string resultante da desmembragem recursiva.</returns>
        private string HandleRecursiveDemember(ReadOnlyCollection<ParameterExpression> parameters, Expression left, Expression right, string equalityOperator, ref StringBuilder lambdaText, bool replaceLambda)
        {
            // Desmembra recursivamente os nós esquerdo e direito.
            string leftVal = DememberExpression(parameters: parameters, expression: left, ref lambdaText, replaceLambda: replaceLambda);
            string rightVal = DememberExpression(parameters: parameters, expression: right, ref lambdaText, replaceLambda: replaceLambda);
            return string.Format("{0} {1} {2}", leftVal, equalityOperator, rightVal);
        }

        /// <summary>
        /// Lida com a parte direita de uma expressão, formatando-a adequadamente.
        /// </summary>
        /// <param name="parameters">A expressão original.</param>
        /// <param name="nodeExpression">A expressão nó (left - right).</param>
        /// <param name="rightExpression">O nó direito da expressão.</param>
        /// <param name="equalityOperator">O operador de igualdade a ser utilizado.</param>
        /// <param name="value">O valor já formatado da expressão.</param>
        /// <returns>A string formatada da expressão direita.</returns>
        private string HandleRightExpression(ReadOnlyCollection<ParameterExpression> parameters, Expression nodeExpression, dynamic rightExpression, string equalityOperator, string value)
        {
            dynamic rightDynamicExpression = rightExpression;

            //// Lida com o caso de negação.
            //if (rightDynamicExpression != null && ContainsProperty(rightDynamicExpression, "NodeType") && rightDynamicExpression.NodeType == ExpressionType.Not && ContainsProperty(rightDynamicExpression, "Operand"))
            //{
            //    rightExpression = rightDynamicExpression.Operand; // TODO
            //}

            // Verifica se a expressão direita é única. (bool) sem comparado == true
            if (IsUniqueExpression(nodeExpression, rightDynamicExpression))
            {
                var booleanExpression = TreatBooleanPropertyExpression(parameters, rightDynamicExpression);
                if (!string.IsNullOrEmpty(booleanExpression?.Trim()))
                {
                    // Formata o valor final, incluindo a expressão booleana.
                    value = string.Format(value, equalityOperator, booleanExpression);

                    ////AddToExpressions(oldExpression, value);
                    return value;
                }
            }

            // Processa a expressão direita.
            value = ProcessRightExpression(parameters, rightDynamicExpression, equalityOperator, value);

            ////AddToExpressions(oldExpression, value);

            return value;
        }

        /// <summary>
        /// Verifica se a expressão direita é única e não possui filhos.
        /// </summary>
        /// <param name="expression">A expressão original.</param>
        /// <param name="partExpression">O nó direito da expressão.</param>
        /// <returns>True se a expressão é única, caso contrário false.</returns>
        private bool IsUniqueExpression(Expression expression, dynamic partExpression)
        {
            // Verifica se a expressão direita não possui filhos e é do tipo And/Or.
            return partExpression != null && !ContainsProperty(partExpression, "Left") &&
                   (expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse);
        }

        /// <summary>
        /// Processa a parte direita de uma expressão, tratando diferentes tipos de nós.
        /// </summary>
        /// <param name="parameters">Expressao bruta.</param>
        /// <param name="rightExpression">O nó direito da expressão.</param>
        /// <param name="equalityOperator">O operador de igualdade a ser utilizado.</param>
        /// <param name="value">O valor já formatado da expressão.</param>
        /// <returns>A string formatada da expressão direita.</returns>
        private string ProcessRightExpression(ReadOnlyCollection<ParameterExpression> parameters, dynamic rightExpression, string equalityOperator, string value)
        {
            // Lida com expressões que são membros.
            if (rightExpression is MemberExpression rightMember)
            {
                bool isValue = IsValue(parameters, rightExpression);
                if (!isValue && rightMember.Member.Name == "Value")
                {
                    rightMember = rightMember.Expression as MemberExpression;
                }

                value = !isValue
                    ? string.Format(value, equalityOperator, GetPropertyFullName(rightMember.Member.DeclaringType, rightMember, rightMember.Member))
                    : TreatExpression(parameters: parameters, right: rightMember, equality: equalityOperator, value: value);
            }
            // Trata chamadas de método que são funções SQL.
            else if (rightExpression is MethodCallExpression && SQL_FUNCTIONS.ContainsKey(rightExpression.Method.Name))
            {
                value = string.Format(value, equalityOperator, ExtractMethod(parameters: parameters, method: rightExpression));
            }
            // Lida com expressões convertidas.
            else if (rightExpression != null && ContainsProperty(rightExpression, "Operand") && ContainsProperty(rightExpression, "NodeType") &&
                     (rightExpression.NodeType == ExpressionType.Convert || rightExpression.NodeType == ExpressionType.ConvertChecked)
                     && !IsValue(parameters: parameters, expression: rightExpression.Operand))
            {
                value = string.Format(value, equalityOperator, GetPropertyFullName(rightExpression.Operand.Member.DeclaringType, rightExpression.Operand, rightExpression.Operand.Member));
            }
            else
            {
                // Trata a expressão de forma geral.
                value = TreatExpression(parameters: parameters, right: rightExpression, equality: equalityOperator, value: value);
            }

            return value;
        }

        ///// <summary>
        ///// Adiciona uma expressão ao dicionário, se não existir.
        ///// </summary>
        ///// <param name="oldExpression">A expressão antiga a ser adicionada.</param>
        ///// <param name="value">O valor formatado da expressão.</param>
        //private void AddToExpressions(string oldExpression, string value)
        //{
        //    // Adiciona a expressão antiga e seu valor ao dicionário, se não existir.
        //    if (!string.IsNullOrEmpty(oldExpression) && !string.IsNullOrEmpty(value) && !_expressions.ContainsKey(oldExpression))
        //    {
        //        _expressions.Add(oldExpression, value);
        //    }
        //}

        /// <summary>
        /// Carrega o valor da expressão de membro passada.
        /// </summary>
        /// <param name="parameters">Arvore de expressão na sua forma bruta</param>
        /// <param name="partExpression">A parte da expressão que será avaliada.</param>
        /// <returns>Uma string representando o valor ou a representação da parte da expressão.</returns>
        public string ReadExpression(ReadOnlyCollection<ParameterExpression> parameters, Expression partExpression)
        {
            // Verifica se a parte da expressão é um MemberExpression.
            if (partExpression is MemberExpression)
            {
                var isValue = IsValue(parameters: parameters, expression: partExpression);
                // Se for um valor, trata e retorna como string.
                if (isValue)
                {
                    return TreatValue(GetValue(parameters: parameters, partExpression: partExpression), true)?.ToString();
                }
                else
                {
                    return GetMemberExpression(partExpression); // TODO MEMORY CACHE
                }
            }
            // Verifica se é uma chamada de método de concatenação e trata.
            else if (partExpression != null && ContainsProperty(partExpression, "Method") && DBQueryConstants.CONCAT_FUNCTION.Equals(((dynamic)partExpression).Method?.Name))
            {
                return ExtractMethod(parameters: parameters, method: partExpression);
            }
            else
            {
                // Para outras expressões, obtém a representação da propriedade.
                return GetPropertyOfSingleExpression(parameters: parameters, expression: partExpression, hasParameter: false, useAlias: false);
            }

            return string.Empty; // Retorna string vazia se não for possível determinar o valor.
        }

        /// <summary>
        /// Obtém a representação da expressão de membro.
        /// </summary>
        /// <param name="partExpression">A expressão de membro a ser processada.</param>
        /// <returns>Uma string representando a tabela e o nome da coluna da expressão de membro.</returns>
        private string GetMemberExpression(Expression partExpression)
        {
            dynamic l = partExpression;
            // Verifica se é uma negação e ajusta a expressão.
            if (partExpression.NodeType == ExpressionType.Not && ContainsProperty(partExpression, "Operand"))
            {
                partExpression = l.Operand;
            }

            var leftMem = partExpression as MemberExpression;
            // Verifica se o membro é "Value" e ajusta a expressão.
            if (leftMem.Member.Name == "Value")
            {
                leftMem = leftMem.Expression as MemberExpression;
            }

            var propertyInfo = (PropertyInfo)leftMem.Member;
            var name = GetColumnName(propertyInfo);

            // Retorna a tabela e o nome da coluna correspondente.
            return GetTableName(propertyInfo.DeclaringType, partExpression) + "." + name;
        }

        /// <summary>
        /// Verifica se a expressão é uma expressão booleana.
        /// </summary>
        /// <param name="expression">A expressão a ser verificada.</param>
        /// <param name="exp">Uma expressão dinâmica adicional para verificação.</param>
        /// <returns>True se a expressão for booleana; caso contrário, false.</returns>
        private bool IsBooleanExpression(Expression expression, dynamic exp)
        {
            return expression.Type == typeof(bool) &&
                (expression.NodeType == ExpressionType.Not ||
                expression.NodeType == ExpressionType.MemberAccess ||
                expression.NodeType == ExpressionType.Call && exp.Method.Name == DBQueryConstants.GET_VALUE_OR_DEFAULT);
        }
        /// <summary>
        /// Trata uma expressão booleana e retorna sua representação como string.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="expression">A expressão booleana a ser tratada.</param>
        /// <returns>Uma string representando a expressão booleana ou null se não for válida.</returns>
        public string TreatBooleanPropertyExpression(ReadOnlyCollection<ParameterExpression> parameters, dynamic expression)
        {
            string value = null;

            // Verifica se a expressão é booleana.
            if (IsBooleanExpression(expression, expression))
            {
                // Ajusta a expressão se for um Nullable<bool>.
                if (ContainsProperty(expression, "Expression") && expression.Expression.Type == typeof(bool?))
                {
                    expression = expression.Expression;
                }

                // Trata a negação na expressão.
                if (expression.NodeType == ExpressionType.Not && ContainsProperty(expression, "Operand"))
                {
                    value = $"{DBKeysConstants.EQUALS_WITH_SPACE}{DBKeysConstants.FALSE_VALUE}";
                    expression = expression.Operand;

                    if (ContainsProperty(expression, "Expression") && expression.Expression.Type == typeof(bool?))
                    {
                        expression = expression.Expression;
                    }
                }
                else
                {
                    value = $"{DBKeysConstants.EQUALS_WITH_SPACE}{DBKeysConstants.TRUE_VALUE}";
                }

                // Se for um método GET_VALUE_OR_DEFAULT, trata a expressão.
                if (ContainsProperty(expression, "Method") && expression.Method.Name == DBQueryConstants.GET_VALUE_OR_DEFAULT)
                {
                    return GetPropertyOfSingleExpression(parameters: parameters, expression: expression, hasParameter: false, useAlias: false) + value;
                }
                else if (ContainsProperty(expression, "Member"))
                {
                    return $"{GetPropertyFullName(expression.Member.DeclaringType, expression, expression.Member)}{value}";
                }
                else if (ContainsProperty(expression, "NodeType") && expression.NodeType == ExpressionType.Call)
                {
                    return null;
                }
            }

            return value; // Retorna null se a expressão não for booleana.
        }

        /// <summary>
        /// Trata uma expressão e retorna uma string formatada com o valor e o comparador.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="right">A expressão da direita a ser tratada.</param>
        /// <param name="equality">O comparador a ser utilizado na expressão.</param>
        /// <param name="value">A string formatada que representa o valor.</param>
        /// <returns>Uma string formatada representando a expressão.</returns>
        protected string TreatExpression(ReadOnlyCollection<ParameterExpression> parameters, Expression right, string equality, string value)
        {
            var val = TreatValue(GetValue(parameters: parameters, partExpression: right), true, right);

            // Ajusta o comparador se o valor for nulo.
            if (DBKeysConstants.NULL.Equals(val))
            {
                equality = DBKeysConstants.EQUALS.Equals(equality) ? DBKeysConstants.IS : DBKeysConstants.IS_NOT;
            }

            return string.Format(value, equality, val); // Retorna a string formatada.
        }

        /// <summary>
        /// Extrai o método de uma expressão e retorna uma string formatada representando a chamada do método.
        /// </summary>
        /// <param name="parameters">Arvore de expressoes bruta</param>
        /// <param name="method">A expressão do tipo MethodCallExpression a ser processada.</param>
        /// <returns>Uma string representando a chamada do método, ou string vazia se não for uma MethodCallExpression.</returns>
        protected string ExtractMethod(ReadOnlyCollection<ParameterExpression> parameters, Expression method)
        {
            if (method is MethodCallExpression mtd)
            {
                // Processa o método e retorna o resultado
                return ProcessMethodCall(parameters: parameters, mtd: mtd);
            }
            else if (method.NodeType == ExpressionType.Not)
            {
                return ProcessMethodCall(parameters: parameters, mtd: ((dynamic)method).Operand, isNotting: true);
            }

            return string.Empty; // Retorna vazio se não for uma MethodCallExpression
        }

        /// <summary>
        /// Processa uma chamada de método e retorna o resultado formatado.
        /// </summary>
        /// <param name="parameters">Representa a arvore bruta</param>
        /// <param name="mtd">A expressão do tipo MethodCallExpression a ser processada.</param>
        /// <param name="isNotting">Indica se trata de uma expressão not</param>
        /// <returns>Uma string representando a chamada do método.</returns>
        private string ProcessMethodCall(ReadOnlyCollection<ParameterExpression> parameters, MethodCallExpression mtd, bool isNotting = false)
        {
            string comparador = TreatComparerByMethod(mtd.Method.Name);
            string valueA = string.Empty;

            // Verifica se é um método nativo e retorna imediatamente se for.
            if (VerififyNativesMethods(parameters, mtd, isNotting, ref valueA))
            {
                return valueA;
            }

            // Trata funções específicas
            if (DBQueryConstants.CONCAT_FUNCTION.Equals(mtd.Method.Name))
            {
                return ConcatFunction(parameters, mtd);
            }
            else if (SQL_FUNCTIONS.ContainsKey(mtd.Method.Name))
            {
                return SQL_FUNCTIONS[mtd.Method.Name](mtd.Arguments);
            }

            // Processa os argumentos do método.
            return ProcessMethodArguments(parameters: parameters, mtd: mtd, comparador: comparador);
        }

        /// <summary>
        /// Processa os argumentos de uma chamada de método e retorna a string formatada.
        /// </summary>
        /// <param name="parameters">Arvore de expressão completa em sua forma bruta</param>
        /// <param name="mtd">A expressão do tipo MethodCallExpression a ser processada.</param>
        /// <param name="comparador">O comparador obtido da chamada do método.</param>
        /// <returns>Uma string formatada representando a chamada do método.</returns>
        private string ProcessMethodArguments(ReadOnlyCollection<ParameterExpression> parameters, MethodCallExpression mtd, string comparador)
        {
            List<dynamic> values = new List<dynamic>();
            string valueA = string.Empty;
            string valueB = string.Empty;

            // Processa cada argumento da chamada do método.
            for (var i = 0; i < mtd.Arguments.Count; i++)
            {
                dynamic arg = mtd.Arguments[i];
                string value = ProcessArgument(parameters: parameters, arg: arg, comparador: comparador, mtd: mtd);

                // Adiciona o valor processado à lista.
                bool isValue = IsValue(parameters: parameters, expression: arg);
                values.Add(new { value, isValue });
            }

            // Formata e retorna a string final.
            if (values.Any())
            {
                valueA = values[0].isValue ? TreatValue(values[0].value, true)?.ToString() : values[0].value;
                valueB = values.Count > 1 ? ProcessValueB(mtd, values) : string.Empty;
            }

            return string.Format("{0} {1} {2}", valueA, comparador, valueB);
        }

        /// <summary>
        /// Processa um único argumento e retorna seu valor formatado.
        /// </summary>
        /// <param name="parameters">Arvore de expressão completa em sua forma bruta</param>
        /// <param name="arg">O argumento a ser processado.</param>
        /// <param name="comparador">O comparador associado à chamada do método.</param>
        /// <param name="mtd">A expressão do tipo MethodCallExpression.</param>
        /// <returns>Uma string representando o valor do argumento.</returns>
        private string ProcessArgument(ReadOnlyCollection<ParameterExpression> parameters, dynamic arg, string comparador, MethodCallExpression mtd)
        {
            string value = string.Empty;

            if (IsValue(parameters: parameters, expression: arg))
            {
                // Processa o argumento como um valor.
                value = ProcessValueArgument(parameters: parameters, arg: arg, comparador: comparador, mtd: mtd);
            }
            else
            {
                // Processa o argumento não sendo um valor.
                value = ProcessNonValueArgument(arg, mtd);
            }

            return value;
        }

        /// <summary>
        /// Processa um argumento que é um valor e retorna seu valor formatado.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="arg">O argumento a ser processado.</param>
        /// <param name="comparador">O comparador associado à chamada do método.</param>
        /// <param name="mtd">A expressão do tipo MethodCallExpression.</param>
        /// <returns>Uma string representando o valor do argumento.</returns>
        private string ProcessValueArgument(ReadOnlyCollection<ParameterExpression> parameters, dynamic arg, string comparador, MethodCallExpression mtd)
        {
            if (comparador == DBKeysConstants.IN || comparador == DBKeysConstants.NOT_IN)
            {
                return InterpretIn(parameters: parameters, mtd: mtd, arg: arg, value: string.Empty);
            }
            else if (ContainsProperty(arg, "Member") && ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.MemberAccess)
            {
                return GetValue(parameters: parameters, partExpression: arg);
            }
            else if (arg.GetType().Name.Equals("PropertyExpression"))
            {
                return string.Concat(GetTableName(arg.Member.DeclaringType, mtd), DBKeysConstants.SINGLE_POINT, GetColumnName(arg.Member));
            }
            else if (arg.GetType().Name.Equals("ConstantExpression"))
            {
                return arg.Value.ToString();
            }
            else if (ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.Call)
            {
                return GetValue(parameters: parameters, partExpression: arg);
            }

            return string.Empty; // Retorna vazio se não puder processar.
        }

        /// <summary>
        /// Processa um argumento que não é um valor e retorna seu valor formatado.
        /// </summary>
        /// <param name="arg">O argumento a ser processado.</param>
        /// <param name="mtd">A expressão do tipo MethodCallExpression.</param>
        /// <returns>Uma string representando o valor do argumento.</returns>
        private string ProcessNonValueArgument(dynamic arg, MethodCallExpression mtd)
        {
            if (ContainsProperty(arg, "Expression") && ContainsProperty(arg, "Type"))
            {
                return string.Concat(GetTableName(arg.Expression.Type, mtd), DBKeysConstants.SINGLE_POINT, GetColumnName(arg.Member));
            }

            return string.Empty; // Retorna vazio se não puder processar.
        }

        /// <summary>
        /// Processa o segundo valor (valueB) de acordo com os argumentos do método.
        /// </summary>
        /// <param name="mtd">A expressão do tipo MethodCallExpression.</param>
        /// <param name="values">Lista de valores processados dos argumentos.</param>
        /// <returns>Uma string representando o segundo valor.</returns>
        private string ProcessValueB(MethodCallExpression mtd, List<dynamic> values)
        {
            dynamic arg = mtd.Arguments[1];

            // Verifica se o segundo argumento é uma chamada de concatenação e processa.
            if (ContainsProperty(arg, "Method") && arg.Method != null &&
                DBQueryConstants.CONCAT_FUNCTION.Equals(arg.Method.Name))
            {
                return ValidateValueByMethod(values[1], mtd.Method.Name, true);
            }

            return ValidateValueByMethod(values[1], mtd.Method.Name, false); // Retorna o valor validado.
        }

        /// <summary>
        /// Interpreta uma chamada de método para construir uma cláusula IN em SQL.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="mtd">A expressão do tipo MethodCallExpression a ser processada.</param>
        /// <param name="arg">O argumento a ser interpretado para a cláusula IN.</param>
        /// <param name="value">O valor inicial a ser ajustado.</param>
        /// <returns>Uma string representando a cláusula IN ajustada.</returns>
        public string InterpretIn(ReadOnlyCollection<ParameterExpression> parameters, MethodCallExpression mtd, dynamic arg, string value)
        {
            // Verifica se o argumento contém uma lista de expressões.
            if (ContainsProperty(arg, "Expressions") && arg.Expressions != null)
            {
                value = HandleExpressions(parameters, arg);
            }
            // Verifica se o argumento é uma chamada de método.
            else if (IsMethodCall(arg))
            {
                value = HandleMethodCall(parameters, arg, value);
            }
            // Verifica se o argumento é uma referência a um membro.
            else if (IsMemberAccess(arg))
            {
                value = HandleMemberAccess(parameters, arg);
            }
            // Verifica se o argumento é uma expressão de propriedade.
            else if (IsPropertyExpression(arg))
            {
                value = HandlePropertyExpression(arg, mtd);
            }
            // Trata caso de uma expressão constante.
            else if (IsConstantExpression(arg))
            {
                value = arg.Value;
            }

            return AdjustEmptyValue(value); // Ajusta o valor vazio, se necessário.
        }

        /// <summary>
        /// Trata uma lista de expressões e retorna uma string formatada para a cláusula IN.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="arg">O argumento que contém as expressões.</param>
        /// <returns>Uma string formatada representando as expressões.</returns>
        private string HandleExpressions(ReadOnlyCollection<ParameterExpression> parameters, dynamic arg)
        {
            var aux = new List<string>();
            foreach (var item in arg.Expressions)
            {
                var val = GetValue(parameters, item);
                aux.Add("'" + val + "'"); // Adiciona cada valor entre aspas simples.
            }
            return "(" + string.Join(", ", aux) + ")"; // Formata e retorna a lista de valores.
        }

        /// <summary>
        /// Trata uma chamada de método e retorna os valores formatados.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="arg">O argumento que contém a chamada do método.</param>
        /// <param name="value">O valor a ser ajustado.</param>
        /// <returns>Uma string formatada representando os valores retornados pela chamada do método.</returns>
        private string HandleMethodCall(ReadOnlyCollection<ParameterExpression> parameters, dynamic arg, string value)
        {
            var list = GetValue(parameters, arg);
            // Retorna imediatamente se o resultado for uma string ou se for um método específico.
            if (list is string || DBQueryConstants.GENERATE_SCRIPT_IN.Equals(arg.Method.Name))
            {
                return list;
            }

            var aux = new List<string>();
            foreach (var item in list)
            {
                aux.Add("'" + item.ToString() + "'"); // Adiciona cada item entre aspas simples.
            }
            return "(" + string.Join(", ", aux) + ")"; // Formata e retorna a lista de valores.
        }

        /// <summary>
        /// Trata acessos a membros e retorna os valores formatados.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="arg">O argumento que representa o acesso ao membro.</param>
        /// <returns>Uma string formatada representando os valores do membro acessado.</returns>
        private string HandleMemberAccess(ReadOnlyCollection<ParameterExpression> parameters, dynamic arg)
        {
            var val = GetValue(parameters, arg);
            // Se o valor for um array de tipos específicos, formata os valores.
            if (val is string[] || val is short[] || val is int[] || val is long[])
            {
                var aux = new List<string>();
                foreach (var item in val)
                {
                    aux.Add("'" + item.ToString() + "'"); // Adiciona cada item entre aspas simples.
                }
                return "(" + string.Join(", ", aux) + ")"; // Formata e retorna a lista de valores.
            }
            return GetValue(parameters, arg); // Retorna o valor se não for um array.
        }

        /// <summary>
        /// Trata expressões de propriedade e retorna seu nome formatado.
        /// </summary>
        /// <param name="arg">O argumento que representa a expressão de propriedade.</param>
        /// <param name="mtd">A expressão do tipo MethodCallExpression associada.</param>
        /// <returns>Uma string representando a expressão de propriedade formatada.</returns>
        private string HandlePropertyExpression(dynamic arg, MethodCallExpression mtd)
        {
            return string.Concat(GetTableName(arg.Member.DeclaringType, mtd), DBKeysConstants.SINGLE_POINT, GetColumnName(arg.Member));
        }

        /// <summary>
        /// Ajusta o valor retornado para garantir que não seja uma representação vazia.
        /// </summary>
        /// <param name="value">O valor a ser ajustado.</param>
        /// <returns>Uma string ajustada que nunca será vazia.</returns>
        private string AdjustEmptyValue(string value)
        {
            return value == "()" ? "('')" : value; // Retorna uma string com um valor vazio se necessário.
        }

        /// <summary>
        /// Verifica se o argumento é uma chamada de método.
        /// </summary>
        /// <param name="arg">O argumento a ser verificado.</param>
        /// <returns>True se for uma chamada de método, false caso contrário.</returns>
        private bool IsMethodCall(dynamic arg)
        {
            return ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.Call && ContainsProperty(arg, "Type");
        }

        /// <summary>
        /// Verifica se o argumento é um acesso a um membro.
        /// </summary>
        /// <param name="arg">O argumento a ser verificado.</param>
        /// <returns>True se for um acesso a um membro, false caso contrário.</returns>
        private bool IsMemberAccess(dynamic arg)
        {
            return arg != null && ContainsProperty(arg, "Member") && ContainsProperty(arg, "NodeType") && arg.NodeType == ExpressionType.MemberAccess;
        }

        /// <summary>
        /// Verifica se o argumento é uma expressão de propriedade.
        /// </summary>
        /// <param name="arg">O argumento a ser verificado.</param>
        /// <returns>True se for uma expressão de propriedade, false caso contrário.</returns>
        private bool IsPropertyExpression(dynamic arg)
        {
            return arg.GetType().Name.Equals("PropertyExpression");
        }

        /// <summary>
        /// Verifica se o argumento é uma expressão constante.
        /// </summary>
        /// <param name="arg">O argumento a ser verificado.</param>
        /// <returns>True se for uma expressão constante, false caso contrário.</returns>
        private bool IsConstantExpression(dynamic arg)
        {
            return arg.GetType().Name.Equals("ConstantExpression");
        }

        /// <summary>
        /// Concatena os valores das expressões passadas em um formato SQL adequado.
        /// </summary>
        /// <param name="parameters">Arvore de expressão na sua forma bruta</param>
        /// <param name="exp">A expressão que contém os argumentos a serem concatenados.</param>
        /// <returns>Uma string representando a concatenação dos valores formatados para SQL.</returns>
        private string ConcatFunction(ReadOnlyCollection<ParameterExpression> parameters, dynamic exp)
        {
            var d = exp.Arguments[0]; // Obtém os argumentos da chamada.
            var list = new List<string>(); // Lista para armazenar os valores concatenados.

            for (var i = 0; i < d.Expressions.Count; i++)
            {
                var xp1 = d.Expressions[i]; // Pega cada expressão da lista.
                var xp = xp1;

                // Se a expressão é uma conversão, obtém o operando.
                if (xp != null && xp.NodeType == ExpressionType.Convert && ContainsProperty(xp, "Operand"))
                {
                    xp = xp.Operand;
                }

                string value = ""; // Inicializa o valor da expressão.

                // Se a expressão é um MemberExpression, obtém seu valor correspondente.
                if (xp is MemberExpression)
                {
                    bool isValue = true;
                    isValue = IsValue(parameters: parameters, expression: xp);

                    // Se não é um valor, obtemos o nome da coluna.
                    if (!isValue)
                    {
                        var propertyInfo1 = (PropertyInfo)xp.Member;
                        var name1 = GetColumnName(propertyInfo1);
                        value = GetTableName(propertyInfo1.DeclaringType, xp) + "." + name1; // Formata o nome completo da coluna.
                    }
                }

                // Se a expressão é uma verificação de nulo, trata-a de forma especial.
                if (xp.Type == typeof(IsNull))
                {
                    value = GetPropertyOfSingleExpression(parameters: parameters, expression: xp, hasParameter: false, useAlias: false);
                }

                // Se não foi atribuído um valor, tenta obter o valor da expressão.
                if (string.IsNullOrEmpty(value))
                {
                    value = TreatValue(GetValue(parameters, xp), true)?.ToString();
                }

                // Formata o valor para SQL.
                value = string.Format(DBKeysConstants.CONVERT_VARCHAR, value);
                list.Add(value); // Adiciona o valor à lista.
            }

            // Retorna a string final com a concatenação dos valores.
            return string.Format(DBKeysConstants.CONCAT, string.Join(",", list));
        }

        /// <summary>
        /// Valida o valor de um objeto com base no método e ajusta o formato para SQL.
        /// </summary>
        /// <param name="obj">O objeto que contém o valor a ser validado.</param>
        /// <param name="methodName">O nome do método que está sendo avaliado.</param>
        /// <param name="isConcatValue">Indica se o valor é parte de uma concatenação.</param>
        /// <returns>Uma string formatada representando o valor validado para SQL.</returns>
        protected string ValidateValueByMethod(dynamic obj, string methodName, bool isConcatValue)
        {
            string valorTratado = TreatValue(obj.value, true)?.ToString(); // Trata o valor.

            // Trata os métodos LIKE e NOT LIKE, ajustando a formatação.
            if (DBQueryConstants.LIKE_FUNCTION.Equals(methodName) || DBQueryConstants.NOT_LIKE_FUNCTION.Equals(methodName))
            {
                if (!obj.isValue || isConcatValue)
                {
                    return string.Format(DBKeysConstants.CONCAT, $"'%', {obj.value} ,'%'"); // Adiciona '%' ao redor do valor.
                }

                if (obj.value is null)
                {
                    return "('')";
                }

                return obj.value.ToString().Contains("%") ? $"'{obj.value}'" : $"'%{obj.value}%'"; // Formatação baseada na presença de '%'.
            }
            // Trata os métodos IN e NOT IN.
            else if (DBQueryConstants.IN_FUNCTION.Equals(methodName) || DBQueryConstants.NOT_IN_FUNCTION.Equals(methodName))
            {
                return obj.value?.ToString() ?? "('')"; // Retorna o valor diretamente.
            }
            // Retorna o valor tratado para outros métodos.
            else
            {
                return valorTratado;
            }
        }

        /// <summary>
        /// Mapeia o nome de um método para seu comparador SQL correspondente.
        /// </summary>
        /// <param name="methodName">O nome do método a ser tratado.</param>
        /// <param name="isNotting">Para alguns cenarios vai tratar a negação</param>
        /// <returns>Uma string representando o comparador SQL correspondente.</returns>
        protected string TreatComparerByMethod(string methodName, bool isNotting = false)
        {
            // Mapeia o nome do método para o comparador SQL correspondente.
            if (DBQueryConstants.LIKE_FUNCTION.Equals(methodName))
            {
                return DBKeysConstants.LIKE;
            }
            else if (DBQueryConstants.NOT_LIKE_FUNCTION.Equals(methodName))
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
                return isNotting ? DBKeysConstants.NOT_EQUAL : DBKeysConstants.EQUALS;
            }
            else if (DBQueryConstants.CONTAINS.Equals(methodName))
            {
                return isNotting ? DBKeysConstants.NOT_LIKE : DBKeysConstants.LIKE;
            }
            else
            {
                return ""; // Retorna vazio para métodos não mapeados.
            }
        }
        /// <summary>
        /// Obtém o valor de uma expressão fornecida, tratando diferentes tipos de expressões.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="partExpression">A expressão da qual o valor deve ser obtido.</param>
        /// <returns>O valor da expressão, que pode ser de qualquer tipo dinâmico.</returns>
        protected dynamic GetValue(ReadOnlyCollection<ParameterExpression> parameters, Expression partExpression)
        {
            switch (partExpression)
            {
                case ConstantExpression rightConst:
                    return rightConst.Value; // Retorna o valor de uma constante.

                case MemberExpression memberExp:
                    return CompileAndInvoke(Expression.Convert(memberExp, typeof(object))); // Compila e invoca o membro.

                case MethodCallExpression methodCallExp:
                    // Verifica se o método é um dos métodos especiais que requerem extração.
                    if (ContainsProperty(methodCallExp, "Method") &&
                        (DBQueryConstants.LIKE_FUNCTION.Equals(methodCallExp.Method.Name) ||
                         DBQueryConstants.IN_FUNCTION.Equals(methodCallExp.Method.Name) ||
                         DBQueryConstants.NOT_IN_FUNCTION.Equals(methodCallExp.Method.Name) ||
                         DBQueryConstants.CONCAT_FUNCTION.Equals(methodCallExp.Method.Name) ||
                         SQL_FUNCTIONS.ContainsKey(methodCallExp.Method.Name) ||
                         DBQueryConstants.NOT_LIKE_FUNCTION.Equals(methodCallExp.Method.Name)))
                    {
                        return ExtractMethod(parameters: parameters, method: partExpression); // Extrai o método se for um dos definidos.
                    }
                    return CompileAndInvoke(methodCallExp); // Compila e invoca a chamada do método.

                case UnaryExpression unaryExp:
                    // Lida com expressões unárias e retorna o valor apropriado.
                    return unaryExp.Operand switch
                    {
                        ConstantExpression uConst => uConst.Value, // Valor de uma constante.
                        MemberExpression uMember => CompileAndInvoke(Expression.Convert(uMember, typeof(object))), // Membro convertido.
                        MethodCallExpression uMethodCall => CompileAndInvoke(uMethodCall), // Chamada de método.
                        _ => string.Empty // Retorna vazio para outros tipos.
                    };

                case Expression exp when ContainsProperty(exp, "NodeType"):
                    // Trata expressões com base em seu tipo de nó.
                    return exp.NodeType switch
                    {
                        ExpressionType.Call => CompileAndInvoke(exp), // Chamada de método.
                        ExpressionType.ConvertChecked => CompileAndInvoke(exp), // Conversão verificada.
                        ExpressionType.Convert => CompileAndInvoke(exp), // Conversão simples.
                        ExpressionType.Lambda => CompileAndInvoke(exp), // Expressão lambda.
                        _ => string.Empty // Retorna vazio para outros tipos.
                    };

                default:
                    return string.Empty; // Retorna vazio se a expressão não for reconhecida.
            }
        }

        /// <summary>
        /// Compila e invoca uma expressão, retornando o resultado dinâmico.
        /// </summary>
        /// <param name="expression">A expressão a ser compilada e invocada.</param>
        /// <returns>O resultado da invocação da expressão.</returns>
        private dynamic CompileAndInvoke(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke(); // Compila e invoca a expressão.
        }

        /// <summary>
        /// Verifica se a expressão representa um valor. Feito atravéz das comparacoes dos nomes dos parametros da lambda
        /// </summary>
        /// <param name="parameters">Represanta e expressao bruta da lambda seja (select, join, where)</param>
        /// <param name="expression">A expressão a ser verificada.</param>
        /// <returns>Retorna verdadeiro se a expressão é um valor; caso contrário, retorna falso.</returns>
        protected bool IsValue(ReadOnlyCollection<ParameterExpression> parameters, dynamic expression)
        {
            if (expression == null)
            {
                return true; // Null é considerado um valor.
            }

            bool isValue = true; // Inicializa o indicador de valor como verdadeiro.

            // Verifica se a expressão atual contém parâmetros.

            foreach (var parameter in parameters)
            {
                // Se a expressão é uma negação, obtém o operando.
                if (ContainsProperty(expression, "NodeType") && expression.NodeType == ExpressionType.Not && ContainsProperty(expression, "Operand"))
                {
                    expression = expression.Operand; // Atualiza a expressão para o operando.
                }

                // Verifica se a expressão tem um membro e compara com os parâmetros.
                if (ContainsProperty(expression, "Expression") && expression.Expression != null)
                {
                    if (ContainsProperty(expression.Expression, "Member"))
                    {
                        // Verifica se o membro atual é igual a um parâmetro.
                        if (expression.Expression.Member != null && parameter.Name == expression.Expression.Member.Name)
                        {
                            isValue = false; // Não é um valor.
                            break;
                        }

                        // Verifica se o membro é "Value" ou "HasValue" para tipos anuláveis.
                        if ((expression.Member.Name == "Value" || expression.Member.Name == "HasValue") &&
                            ContainsProperty(expression.Expression, "Expression") &&
                            expression.Expression.Expression != null &&
                            ContainsProperty(expression.Expression.Expression, "Name") &&
                            expression.Expression.Expression.Name != null &&
                            parameter.Name == expression.Expression.Expression.Name)
                        {
                            isValue = false; // Não é um valor.
                            break;
                        }
                    }
                    // Verifica se o nome da expressão é igual a um parâmetro.
                    else if (ContainsProperty(expression.Expression, "Name") && parameter.Name == expression.Expression.Name)
                    {
                        isValue = false; // Não é um valor.
                        break;
                    }
                }
            }

            return isValue; // Retorna o resultado da verificação.
        }


        /// <summary>
        /// Verifica se o método chamado é um dos métodos nativos conhecidos e formata o valor de saída.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="method">A expressão do tipo MethodCallExpression a ser verificada.</param>
        /// <param name="value">Uma referência para armazenar o resultado formatado.</param>
        /// <param name="isNotting">Indica se trata de uma expressão not</param>
        /// <returns>Retorna verdadeiro se o método é um nativo reconhecido, caso contrário, retorna falso.</returns>
        public bool VerififyNativesMethods(ReadOnlyCollection<ParameterExpression> parameters, MethodCallExpression method, bool isNotting, ref string value)
        {
            string valueA = string.Empty; // Armazena o valor à esquerda da comparação.
            string valueB = string.Empty; // Armazena o valor à direita da comparação.
            string comparador = string.Empty; // Armazena o comparador.

            dynamic mtd = method; // A chamada do método em questão.

            // Verifica se o método é uma função de igualdade (equals).
            if (mtd.Method.Name.Equals(DBQueryConstants.EQUALS_FUNCTION))
            {
                valueA = GetFormattedValue(parameters: parameters, expression: mtd.Object); // Obtém o valor da primeira parte da comparação.
                comparador = TreatComparerByMethod(mtd.Method.Name, isNotting); // Obtém o comparador correspondente ao método.
                valueB = GetFormattedValue(parameters: parameters, expression: mtd.Arguments[0]); // Obtém o valor do argumento da comparação.

                // Formata a string final no formato: valorA comparador valorB.
                value = string.Format("{0} {1} {2}", valueA, comparador, valueB);
                return true; // Retorna verdadeiro indicando que um método nativo foi encontrado.
            }
            // Verifica se o método é uma função de contagem (contains).
            else if (mtd.Method.Name.Equals(DBQueryConstants.CONTAINS))
            {
                valueA = GetFormattedValue(parameters: parameters, expression: mtd.Object); // Obtém o valor da primeira parte da comparação.
                comparador = TreatComparerByMethod(mtd.Method.Name, isNotting); // Obtém o comparador correspondente ao método.
                valueB = GetFormattedValue(parameters: parameters, expression: mtd.Arguments[0], useQuotes: false); // Obtém o valor do argumento da comparação.

                // Formata a string final no formato: valorA comparador valorB.
                value = string.Format("{0} {1} {2}", valueA, comparador, valueB.Contains("%") ? $"'{valueB}'" : $"'%{valueB}%'");
                return true; // Retorna verdadeiro indicando que um método nativo foi encontrado.
            }

            return false; // Retorna falso se não for um método nativo reconhecido.
        }

        /// <summary>
        /// Obtém o valor formatado a partir de uma expressão.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="expression">A expressão a ser formatada.</param>
        /// <param name="useQuotes">Indica se as aspas devem ser usadas na formatação.</param>
        /// <returns>O valor formatado.</returns>
        private string GetFormattedValue(ReadOnlyCollection<ParameterExpression> parameters, dynamic expression, bool useQuotes = true)
        {
            if (IsValue(parameters, expression))
            {
                return TreatValue(GetValue(parameters, expression), useQuotes)?.ToString(); // Trata e formata o valor.
            }
            else
            {
                // Se não for um valor, concatena o nome da tabela e da coluna.
                return string.Concat(GetTableName(expression.Member.DeclaringType, expression),
                                     DBKeysConstants.SINGLE_POINT,
                                     expression.Member.Name);
            }
        }

        /// <summary>
        /// Obtém o comparador correspondente ao tipo de expressão.
        /// </summary>
        /// <param name="expression">A expressão a ser analisada.</param>
        /// <returns>Retorna o comparador como uma string.</returns>
        protected string GetComparador(Expression expression)
        {
            // Inicializa a string do comparador.
            string comparador = string.Empty;

            // Atribui o comparador correspondente com base no tipo de expressão.
            comparador = expression.NodeType switch
            {
                ExpressionType.Equal => DBKeysConstants.EQUALS,
                ExpressionType.AndAlso => DBKeysConstants.AND,
                ExpressionType.OrElse => DBKeysConstants.OR,
                ExpressionType.NotEqual => DBKeysConstants.NOT_EQUAL,
                ExpressionType.GreaterThan => DBKeysConstants.GREATER_THAN,
                ExpressionType.GreaterThanOrEqual => DBKeysConstants.GREATER_THAN_OR_EQUAL,
                ExpressionType.LessThan => DBKeysConstants.LESS_THAN,
                ExpressionType.LessThanOrEqual => DBKeysConstants.LESS_THAN_OR_EQUAL,
                ExpressionType.Multiply => DBKeysConstants.MULTIPLY,
                ExpressionType.Divide => DBKeysConstants.DIVIDE,
                ExpressionType.Subtract => DBKeysConstants.SUBSTRACT,
                ExpressionType.Add => DBKeysConstants.ADD,
                _ => comparador // Retorna a string inicial se não houver correspondência.
            };

            return comparador; // Retorna o comparador obtido.
        }


        /// <summary>
        /// Obtém o nó esquerdo de uma expressão.
        /// </summary>
        /// <param name="expression">A expressão a ser analisada.</param>
        /// <returns>Retorna o nó esquerdo da expressão.</returns>
        protected Expression GetLeftNode(Expression expression)
        {
            dynamic exp = expression; // Converte a expressão para dinâmica.
            var left = exp.Left; // Obtém o nó esquerdo da expressão.

            // Verifica se o nó é um tipo convertido e extrai o operando.
            if (left != null && (left.NodeType == ExpressionType.Convert || left.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(left, "Operand"))
            {
                left = left.Operand;
                // Verifica se ainda é um tipo convertido e extrai novamente o operando.
                if ((left.NodeType == ExpressionType.Convert || left.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(left, "Operand"))
                {
                    return left.Operand;
                }
                return left; // Retorna o nó esquerdo.
            }
            return (Expression)exp.Left; // Retorna o nó esquerdo original.
        }

        /// <summary>
        /// Obtém o nó direito de uma expressão.
        /// </summary>
        /// <param name="expression">A expressão a ser analisada.</param>
        /// <returns>Retorna o nó direito da expressão.</returns>
        protected Expression GetRightNode(Expression expression)
        {
            dynamic exp = expression; // Converte a expressão para dinâmica.
            var right = exp.Right; // Obtém o nó direito da expressão.

            // Verifica se o nó é um tipo convertido e extrai o operando.
            if (right != null && (right.NodeType == ExpressionType.Convert || right.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(right, "Operand"))
            {
                right = right.Operand;
                // Verifica se ainda é um tipo convertido e extrai novamente o operando.
                if ((right.NodeType == ExpressionType.Convert || right.NodeType == ExpressionType.ConvertChecked) && ContainsProperty(right, "Operand"))
                {
                    return right.Operand;
                }
                return right; // Retorna o nó direito.
            }
            return (Expression)exp.Right; // Retorna o nó direito original.
        }

        /// <summary>
        /// Trata o valor de uma expressão, aplicando formatação conforme necessário.
        /// </summary>
        /// <param name="val">O valor a ser tratado.</param>
        /// <param name="useQuotes">Indica se as aspas devem ser usadas.</param>
        /// <param name="expression">A expressão associada, se houver.</param>
        /// <returns>Retorna o valor tratado.</returns>
        public object TreatValue(dynamic val, bool useQuotes = false, dynamic expression = null)
        {
            // Verifica se a expressão é uma chamada de método CONCAT e retorna o valor sem alteração.
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

            // Verifica se o valor é nulo ou uma string vazia, retornando um valor nulo constante se necessário.
            if (val == null || val.GetType() != typeof(string) && string.IsNullOrEmpty(val?.ToString()) && useQuotes)
            {
                return DBKeysConstants.NULL;
            }

            // Trata valores de DateTime, formatando conforme necessário.
            if (val.GetType() == typeof(DateTime))
            {
                return useQuotes ? $"'{val.ToString(DBKeysConstants.DATE_FORMAT)}'" : val.ToString(DBKeysConstants.DATE_FORMAT);
            }
            if (val.GetType() == typeof(DateTimeOffset))
            {
                return useQuotes ? $"'{val.ToString(DBKeysConstants.DATE_OFF_SET_FORMAT)}'" : val.ToString(DBKeysConstants.DATE_OFF_SET_FORMAT);
            }
            // Trata valores booleanos, convertendo para 1 ou 0.
            else if (val.GetType() == typeof(bool))
            {
                return val ? 1 : 0;
            }
            // Trata valores decimais, formatando e substituindo vírgulas por pontos.
            else if (val.GetType() == typeof(decimal))
            {
                return val?.ToString(DBKeysConstants.DECIMAL_FORMAT)?.Replace(",", ".");
            }
            // Trata valores inteiros ou decimais.
            else if (val.GetType() == typeof(int) || val.GetType() == typeof(decimal))
            {
                return val; // Retorna o valor sem alteração.
            }
            // Trata valores de outros tipos, escapando aspas e formatando conforme necessário.
            else
            {
                val = val.ToString().Replace("'", "''"); // Escapa aspas simples.
                return useQuotes ? $"'{val}'" : val; // Retorna com ou sem aspas, conforme necessário.
            }
        }

        #endregion

        #region Validate Domain 
        /// <summary>
        /// Verifica se o objeto de domínio é válido.
        /// </summary>
        /// <param name="domain">O objeto de domínio a ser validado.</param>
        /// <returns>Retorna true se o objeto for válido; caso contrário, lança uma exceção.</returns>
        protected bool IsValid(EntityBase domain)
        {
            StringBuilder stringBuilder = new StringBuilder(); // Construtor para mensagens de erro.
            ValidationContext _context = new ValidationContext(domain); // Contexto de validação para o objeto de domínio.

            IList<ValidationResult> validationResults = new List<ValidationResult>(); // Lista para armazenar resultados de validação.

            if (domain != null) // Verifica se o objeto de domínio não é nulo.
            {
                // Tenta validar o objeto de domínio.
                if (!Validator.TryValidateObject(domain, _context, validationResults, true))
                {
                    // Mensagem padrão para um ou mais erros.
                    if (validationResults.Count == 1)
                    {
                        stringBuilder.AppendLine("Por favor revise o formulário. Foi encontrado o seguinte erro:");
                    }
                    else
                    {
                        stringBuilder.AppendLine("Por favor revise o formulário. Foram encontrados os seguintes erros:");
                    }
                    stringBuilder.AppendLine(""); // Adiciona uma linha em branco.

                    // Adiciona cada mensagem de erro à string.
                    for (var i = 1; i <= validationResults.Count(); i++)
                    {
                        ValidationResult result = validationResults[i - 1]; // Obtém o resultado da validação.
                        if (result == validationResults.Last()) // Se for o último erro, não adiciona uma nova linha.
                        {
                            stringBuilder.Append("- " + result.ErrorMessage);
                        }
                        else // Para os demais erros, adiciona uma nova linha.
                        {
                            stringBuilder.AppendLine("- " + result.ErrorMessage);
                        }
                    }

                    // Lança uma exceção com a lista de erros, se houver.
                    if (validationResults.Any())
                    {
                        throw new Exception(stringBuilder.ToString());
                    }
                }
            }
            return true; // Retorna true se não houver erros de validação.
        }
        #endregion
    }
}
