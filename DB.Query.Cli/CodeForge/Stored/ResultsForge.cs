using Microsoft.Data.SqlClient;
using System;
using System.CodeDom;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DB.Query.Cli.CodeForge.Stored
{
    public class ResultsForge : CodeForge
    {
        private string _className;
        private bool _normalizeColumns;

        public ResultsForge(string conexao, string database, string tableName, string className, bool normalizeColumns)
        {
            _conexao = conexao;
            _tableName = tableName;
            _database = database;
            _className = className;
            _normalizeColumns = normalizeColumns;
        }

        public override CodeCompileUnit Init()
        {
            try
            {
                _tableName += "Result";

                CodeCompileUnit compileUnit = new CodeCompileUnit();

                var columns = ExecuteQuery($"SELECT name AS [ParameterName], type_name(user_type_id) AS [ParameterDataType], max_length AS [ParameterMaxBytes] FROM sys.parameters WHERE object_id = object_id('{_tableName}')");

                var result = ExecProcedureValuesDefault(columns);

                if (string.IsNullOrWhiteSpace(_className))
                {   // Default name
                    _className = "Unnamed";
                }

                CodeTypeDeclaration codeClass = CreateClass(_className);
                codeClass.CustomAttributes.Add(CreateAttribute("Procedure", new CodeAttributeArgument(new CodePrimitiveExpression(_tableName))));

                // Add public properties
                foreach (DataColumn column in result.Columns)
                {
                    codeClass.Members.Add(CreateProperty(column.ColumnName, column.DataType));
                }

                var member = new CodeSnippetTypeMember();
                member.Comments.Add(new CodeCommentStatement("Declare your implementation here"));
                member.Text = GetCustomImplementation(_className) + "        ";
                member.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Custom Implementation"));
                member.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Custom Implementation"));

                codeClass.Members.Add(member);

                var entryAssembly = Assembly.GetEntryAssembly();
                var mainNamespace = entryAssembly.GetTypes()
                    .Select(t => t.Namespace)
                    .Distinct()
                    .OrderBy(n => n)
                    .FirstOrDefault();

                Console.WriteLine("O namespace principal do projeto é: " + mainNamespace);

                CodeNamespace codeNamespace = new CodeNamespace(mainNamespace);
                CodeNamespace importsNamespace = new CodeNamespace
                {
                    Imports =
                {
                    new CodeNamespaceImport($"DB.Query.Core.Annotations.Entity"),
                    new CodeNamespaceImport($"DB.Query.Core.Annotations"),
                    new CodeNamespaceImport($"DB.Query.Core.Entities")
                }
                };
                codeNamespace.Types.Add(codeClass);

                compileUnit.Namespaces.Add(importsNamespace);
                compileUnit.Namespaces.Add(codeNamespace);

                return compileUnit;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private CodeTypeMember CreateProperty(string columnName, Type dataType)
        {
            dataType = dataType = ValidateType(dataType, true);

            string memberName = $"{columnName} {GetAndSet}";

            CodeMemberField result = new CodeMemberField(dataType, memberName);
            result.Comments.Add(new CodeCommentStatement("<summary>", true));
            result.Comments.Add(new CodeCommentStatement("Propiedade mapeada para a definição de " + columnName, true));
            result.Comments.Add(new CodeCommentStatement("</summary>", true));

            result.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private DataTable ExecProcedureValuesDefault(DataTable columns)
        {
            DataTable dataTable = new DataTable();
            SqlConnection Sql_Conexao = OpenConnection();

            SqlCommand Sql_Comando = new SqlCommand(_tableName, Sql_Conexao) { CommandType = CommandType.StoredProcedure };

            columns.AsEnumerable().ToList().ForEach(member =>
            {
                typeMapper.TryGetValue(member["ParameterDataType"].ToString(), out ParameterTypeMap parameterTypeMap);
                Sql_Comando.AddParameter(member["ParameterName"].ToString(), parameterTypeMap.DbType, null);
            });

            SqlDataAdapter sql_Ada = new SqlDataAdapter(Sql_Comando);
            sql_Ada.Fill(dataTable);

            return dataTable;
        }
    }
}
