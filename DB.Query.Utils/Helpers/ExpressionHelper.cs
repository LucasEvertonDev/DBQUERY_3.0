using System.Linq.Expressions;
using System;
using System.Linq;
using DB.Query.Core.Annotations.Entity;

namespace DB.Query.Utils.Helpers
{
    public static class ExpressionHelper
    {
        public static string TranslateLambda(dynamic propertyExpression, bool useAlias = false, bool includeSourceName = false)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            string propertyName = InterpretExpression(propertyExpression.Body, useAlias, includeSourceName);

            return propertyName;
        }

        private static string InterpretExpression(Expression expression, bool useAlias = false, bool includeSourceName = false)
        {
            switch (expression)
            {
                case MemberExpression memberExpression:
                    return GetNestedPropertyNameSegment(memberExpression, useAlias, includeSourceName);

                case MethodCallExpression methodCallExpression when methodCallExpression.Method.Name == "get_Item" && methodCallExpression.Arguments.Count == 1:
                    var indexExpression = (ConstantExpression)methodCallExpression.Arguments[0];
                    return $"{InterpretExpression(methodCallExpression.Object, useAlias, includeSourceName)}[{indexExpression.Value}]";

                case UnaryExpression unaryExpression:
                    return InterpretExpression(unaryExpression.Operand, useAlias, includeSourceName);

                case var exp when exp.NodeType == ExpressionType.Parameter:
                    return GetTableNameOrAlias(exp.Type, exp, useAlias);

                default:
                    return string.Empty;
            }
        }

        public static string GetTableNameOrAlias(Type type, dynamic exp, bool useAlias)
        {
            // Obtém o atributo TableAttribute do tipo
            var displayName = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;

            // Se houver um alias e o nome não for vazio, retorna o alias
            if (useAlias && !string.IsNullOrEmpty(exp.Name))
            {
                return exp.Name;
            }

            // Caso contrário, retorna o nome da tabela se o atributo existir; caso contrário, retorna o nome do tipo
            return displayName != null ? displayName.TableName : type.Name;
        }

        private static string GetNestedPropertyNameSegment(MemberExpression memberExpression, bool useAlias = false, bool includeSourceName = false)
        {
            if (memberExpression.Expression is null)
            {
                return memberExpression.Member.Name;
            }

            string parentSegment = InterpretExpression(memberExpression.Expression, useAlias, includeSourceName);
            return parentSegment == string.Empty ? $"{memberExpression.Member.Name}" : $"{parentSegment}.{memberExpression.Member.Name}";
        }

        static void start()
        {
            Expression<Func<int, bool>> expr1 = x => x > 1 || x < 10;
            Expression<Func<int, int>> expr2 = x => x + 1;
            Expression<Func<int, bool>> expr3 = x => x + 1 > 5;
            Expression<Func<int, bool>> expr4 = x => x + 1 == x + 2;
            Expression<Func<int, bool>> expr5 = x => x > 1 && (x < 10 || x == 5);
            Expression<Func<int, bool>> expr6 = x => x == 0; // Exemplo simples

            Console.WriteLine(RemoveUnnecessaryParentheses(expr1.Body)); // x > 1 || x < 10
            Console.WriteLine(RemoveUnnecessaryParentheses(expr2.Body)); // x + 1
            Console.WriteLine(RemoveUnnecessaryParentheses(expr3.Body)); // (x + 1) > 5
            Console.WriteLine(RemoveUnnecessaryParentheses(expr4.Body)); // (x + 1) == (x + 2)
            Console.WriteLine(RemoveUnnecessaryParentheses(expr5.Body)); // (x > 1 && (x < 10 || x == 5))
            Console.WriteLine(RemoveUnnecessaryParentheses(expr6.Body)); // x == 0
        }

        static string RemoveUnnecessaryParentheses(Expression expression)
        {
            return FormatExpression(expression, 0);
        }

        static string FormatExpression(Expression expression, int parentPrecedence)
        {
            if (expression is BinaryExpression binary)
            {
                int currentPrecedence = GetOperatorPrecedence(binary.NodeType);
                var left = FormatExpression(binary.Left, currentPrecedence);
                var right = FormatExpression(binary.Right, currentPrecedence);

                // Se a expressão original está envolta em parênteses, mantenha-os
                bool hasExplicitParentheses = IsWrappedInParentheses(binary);
                if (parentPrecedence > currentPrecedence || hasExplicitParentheses)
                    return $"({left} {GetOperator(binary.NodeType)} {right})";
                else
                    return $"{left} {GetOperator(binary.NodeType)} {right}";
            }
            else if (expression is UnaryExpression unary)
            {
                return $"{GetOperator(unary.NodeType)}{FormatExpression(unary.Operand, 0)}";
            }
            else if (expression is ParameterExpression parameter)
            {
                return parameter.Name;
            }
            else if (expression is ConstantExpression constant)
            {
                return constant.Value.ToString();
            }
            else if (expression is MemberExpression member)
            {
                return member.Member.Name;
            }

            return expression.ToString();
        }

        static string GetOperator(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.Add => "+",
                ExpressionType.Subtract => "-",
                ExpressionType.Multiply => "*",
                ExpressionType.Divide => "/",
                ExpressionType.Negate => "-",
                ExpressionType.AndAlso => "&&",
                ExpressionType.OrElse => "||",
                ExpressionType.Equal => "==",
                ExpressionType.NotEqual => "!=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Operador {nodeType} não suportado")
            };
        }

        static int GetOperatorPrecedence(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.OrElse => 1,
                ExpressionType.AndAlso => 2,
                ExpressionType.Equal => 3,
                ExpressionType.NotEqual => 3,
                ExpressionType.LessThan => 3,
                ExpressionType.LessThanOrEqual => 3,
                ExpressionType.GreaterThan => 3,
                ExpressionType.GreaterThanOrEqual => 3,
                ExpressionType.Add => 4,
                ExpressionType.Subtract => 4,
                ExpressionType.Multiply => 5,
                ExpressionType.Divide => 5,
                _ => 0
            };
        }

        static bool IsWrappedInParentheses(Expression expression)
        {
            if (expression is BinaryExpression)
            {
                return true;
            }

            if (expression is UnaryExpression unary)
            {
                return unary.NodeType == ExpressionType.Negate;
            }

            return false;
        }
    }

}

