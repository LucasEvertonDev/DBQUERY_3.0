using System;
using System.CodeDom;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DB.Query.Cli.CodeForge.Stored
{
    public class ParametersForge : CodeForge
    {
        private string _className;
        private bool _normalizeColumns;

        public ParametersForge(string conexao, string database, string tableName, string className, bool normalizeColumns)
        {
            _conexao = conexao;
            _tableName = tableName;
            _database = database;
            _className = className;
            _normalizeColumns = normalizeColumns;
        }

        public override CodeCompileUnit Init()
        {
            _className += "Parameters";

            CodeCompileUnit compileUnit = new CodeCompileUnit();

            var result = ExecuteQuery($"SELECT name AS [ParameterName], type_name(user_type_id) AS [ParameterDataType], max_length AS [ParameterMaxBytes] FROM sys.parameters WHERE object_id = object_id('{_tableName}')");

            if (string.IsNullOrWhiteSpace(_className))
            {   // Default name
                _className = "Unnamed";
            }

            CodeTypeDeclaration codeClass = CreateClass(_className);
            codeClass.CustomAttributes.Add(CreateAttribute("Database", new CodeAttributeArgument(new CodePrimitiveExpression(_database))));
            codeClass.CustomAttributes.Add(CreateAttribute("Procedure", new CodeAttributeArgument(new CodePrimitiveExpression(_tableName))));
            codeClass.CustomAttributes.Add(CreateAttribute("Timeout", new CodeAttributeArgument(new CodePrimitiveExpression(60))));

            codeClass.BaseTypes.Add("StoredProcedureBase");

            result.AsEnumerable().ToList().ForEach(member =>
            {
                codeClass.Members.Add(CreateProperty(member));
            });

            var member = new CodeSnippetTypeMember();
            member.Comments.Add(new CodeCommentStatement("Declare your implementation here"));
            member.Text = GetCustomImplementation(_className) + "        ";
            member.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Custom Implementation"));
            member.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Custom Implementation"));

            codeClass.Members.Add(member);

            CodeNamespace codeNamespace = new CodeNamespace($"DB.Query.{_database}.Storeds");
            CodeNamespace importsNamespace = new CodeNamespace
            {
                Imports =
                {
                    new CodeNamespaceImport($"DB.Query.Core.Annotations.Entity"),
                    new CodeNamespaceImport($"DB.Query.Core.Annotations"),
                    new CodeNamespaceImport($"DB.Query.Core.Entities"),
                    new CodeNamespaceImport("DB.Query.Core.Annotations.StoredProcedure"),
                    new CodeNamespaceImport("DB.Query.Core.Models")
                }
            };
            codeNamespace.Types.Add(codeClass);

            var resultforge = new ResultsForge(_conexao, _database, _tableName, _className.Replace("Parameters", ""), _normalizeColumns);

            var codClassResult = resultforge.GetResultClass();

            if (codClassResult is not null)
            {
                codeNamespace.Types.Add(codClassResult);
            }

            compileUnit.Namespaces.Add(importsNamespace);
            compileUnit.Namespaces.Add(codeNamespace);

            return compileUnit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private CodeTypeMember CreateProperty(DataRow member)
        {
            typeMapper.TryGetValue(member["ParameterDataType"].ToString(), out ParameterTypeMap parameterTypeMap);

            string memberName = $"{member["ParameterName"].ToString().Replace("@", "")}";

            CodeMemberField result = new CodeMemberField(parameterTypeMap.ClrType, !_normalizeColumns ? $"{memberName} {GetAndSet}" : $"{memberName.Replace("_", "")} {GetAndSet}");

            result.Comments.Add(new CodeCommentStatement("<summary>", true));
            result.Comments.Add(new CodeCommentStatement("Propiedade mapeada para a definição de " + memberName, true));
            result.Comments.Add(new CodeCommentStatement("</summary>", true));

            CodeAttributeDeclaration attr = new CodeAttributeDeclaration()
            {
                Name = "Paremeter",
            };
            attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(memberName)));
            attr.Arguments.Add(new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlDbType)), parameterTypeMap.DbType.ToString())));

            result.CustomAttributes.Add(CreateAttribute("Paremeter", new CodeAttributeArgument(new CodePrimitiveExpression(memberName)), new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlDbType)), parameterTypeMap.DbType.ToString()))));

            result.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            return result;
        }
    }
}
