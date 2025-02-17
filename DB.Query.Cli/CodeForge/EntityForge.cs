using System;
using System.CodeDom;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DB.Query.Cli.CodeForge
{
    public class EntityForge : CodeForge
    {
        private string _className;
        private bool _normalizeColumns;

        public EntityForge(string conexao, string database,string tableName, string className, bool normalizeColumns)
        {
            _conexao = conexao;
            _tableName = tableName;
            _database = database;
            _className = className;
            _normalizeColumns = normalizeColumns;
        }

        public override CodeCompileUnit Init()
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            var result = ExecuteQuery($"SELECT TOP(10) * FROM {_database}..{_tableName}");

            var tableDetails = ExecuteQuery(GetTableDetaisQuery());

            if (string.IsNullOrWhiteSpace(_className))
            {   // Default name
                _className = "Unnamed";
            }

            CodeTypeDeclaration codeClass = CreateClass(_className);

            codeClass.CustomAttributes.Add(CreateAttribute("Database", new CodeAttributeArgument(new CodePrimitiveExpression(_database))));
            codeClass.CustomAttributes.Add(CreateAttribute("Table", new CodeAttributeArgument(new CodePrimitiveExpression(_tableName))));

            codeClass.BaseTypes.Add("EntityBase");

            foreach (DataColumn column in result.Columns)
            {
                codeClass.Members.Add(CreateProperty(column.ColumnName, column.DataType, tableDetails));
            }

            IncludeSnippet(codeClass, tableDetails);

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
                    new CodeNamespaceImport($"System"),
                    new CodeNamespaceImport($"System.Collections.Generic"),
                    new CodeNamespaceImport($"System.Linq.Expressions"),
                    new CodeNamespaceImport($"DB.Query.Core.Annotations.Entity"),
                    new CodeNamespaceImport($"DB.Query.Core.Annotations"),
                    new CodeNamespaceImport("DB.Query.Core.Entities")
                }
            };
            codeNamespace.Types.Add(codeClass);

            compileUnit.Namespaces.Add(importsNamespace);
            compileUnit.Namespaces.Add(codeNamespace);

            GenerateClass(compileUnit, Path.Combine(Directory.GetCurrentDirectory(), "Entities"), _className);

            return compileUnit;
        }
    
        private CodeTypeMember CreateProperty(string columnName, Type dataType, DataTable tableDetails)
        {
            var maxLength = 0;
            var column = tableDetails
                .AsEnumerable()
                .ToList()
                .Where(a => a["name"].ToString() == columnName)
                .FirstOrDefault();

            var isnullable = column["is_nullable"].ToString() == "True";

            var isPrimaryKey = column["isPrimaryKey"].ToString() == "True";

            var isIdentity = column["is_identity"].ToString() == "True";

            var isVarcharOrText = column["typeName"].ToString() == "varchar" || column["typeName"].ToString() == "nvarchar" || column["typeName"].ToString() == "text" || column["typeName"].ToString() == "ntext";

            if (!string.IsNullOrEmpty(column["max_length"]?.ToString()))
            {
                maxLength = int.Parse(column["max_length"].ToString());
            }

            dataType = ValidateType(dataType, isnullable);

            string memberName = !_normalizeColumns ? $"{columnName} {GetAndSet}" : $"{columnName.Replace("_", "")} {GetAndSet}";

            CodeMemberField result = new CodeMemberField(dataType, memberName);
            result.Comments.Add(new CodeCommentStatement("<summary>", true));
            result.Comments.Add(new CodeCommentStatement("Propiedade mapeada para a definição de " + columnName, true));
            result.Comments.Add(new CodeCommentStatement("</summary>", true));

            if (isPrimaryKey)
            {
                result.CustomAttributes.Add(CreateAttribute("PrimaryKey", isIdentity ? new CodeAttributeArgument("Identity", new CodePrimitiveExpression(true)) : null));
            }

            if (!isnullable)
            {
                result.CustomAttributes.Add(CreateAttribute("RequiredColumn", new CodeAttributeArgument(new CodePrimitiveExpression(columnName))));
            }

            if (isVarcharOrText && maxLength > 0)
            {
                result.CustomAttributes.Add(CreateAttribute("StringColumnLength", new CodeAttributeArgument(new CodePrimitiveExpression(columnName)), new CodeAttributeArgument(new CodePrimitiveExpression(maxLength))));
            }

            if (columnName.Contains("_") && _normalizeColumns)
            {
                result.CustomAttributes.Add(CreateAttribute("ColumnAttribute", new CodeAttributeArgument(new CodePrimitiveExpression(columnName))));
            }

            result.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            return result;
        }

        private void IncludeSnippet(CodeTypeDeclaration codeClass, DataTable tableDetails)
        {
            var foreignKeys = tableDetails
                .AsEnumerable()
                .ToList()
                .Where(a => !string.IsNullOrEmpty(a["ReferencedTableName"]?.ToString()))
                .GroupBy(g => g["ReferencedTableName"]?.ToString())
                .Select(g => new
                {
                    key = g.Key,
                    columns = g.ToList().Select(g2 => new
                    {
                        column = g2["name"],
                        ReferencedColumnName = g2["ReferencedColumnName"]
                    })
                })
                .Distinct();

            foreach (var foreignKey in foreignKeys)
            {
                string textInit = foreignKey.key.Replace("_", "");
                string textChange = (textInit.Length > 1) ? $"{textInit.Substring(0, 1).ToUpper()}{textInit.Substring(1)}" : (textInit.ToUpper() ?? "");

                string snippetCode = $"{Environment.NewLine}        [Ignore]{Environment.NewLine}        [ForeignKey(typeof({textChange}))]{Environment.NewLine}        public {textChange} {textChange}" + " { get; set; }" + Environment.NewLine; // Snippet de código que define o membro completo
                CodeSnippetTypeMember snippetMember = new CodeSnippetTypeMember(snippetCode);

                codeClass.Members.Add(snippetMember);

                string snippetCode2 = $"{Environment.NewLine}        [Ignore]{Environment.NewLine}        [ForeignKey(typeof({textChange}))]{Environment.NewLine}        private Expression<Func<{textChange}, bool>> Where{textChange} =>" +
                    $" ({textChange} {textChange}) => "; // Snippet de código que define o membro completo

                snippetCode2 += string.Join(" && ", foreignKey.columns.Select(column => $" {textChange}.{column.ReferencedColumnName}  ==  {column.column}"));
                snippetCode2 += ";" + Environment.NewLine;

                codeClass.Members.Add(new CodeSnippetTypeMember(snippetCode2));
            }

            var referencedTables = ExecuteQuery(GetQueryReferencedByFk());

            if (referencedTables is not null)
            {
                var referenced = referencedTables
                    .AsEnumerable()
                    .ToList()
                    .Where(a => !string.IsNullOrEmpty(a["ReferencingTableName"]?.ToString()))
                    .GroupBy(g => g["ReferencingTableName"]?.ToString())
                    .Select(g => new
                    {
                        key = g.Key,
                        columns = g.ToList().Select(g2 => new
                        {
                            column = g2["ReferencingColumnName"],
                            ReferencedColumnName = g2["ReferencedColumnName"]
                        })
                    })
                    .Distinct();

                foreach (var refe in referenced)
                {
                    string textInit = refe.key.Replace("_", "");
                    string textChange = (textInit.Length > 1) ? $"{textInit.Substring(0, 1).ToUpper()}{textInit.Substring(1)}" : (textInit.ToUpper() ?? "");

                    string snippetCode = $"{Environment.NewLine}        [Ignore]{Environment.NewLine}        [ReferencedByForeignKey(typeof({textChange}))]{Environment.NewLine}        public List<{textChange}> {textChange}" + " { get; set; }" + Environment.NewLine; // Snippet de código que define o membro completo
                    CodeSnippetTypeMember snippetMember = new CodeSnippetTypeMember(snippetCode);

                    codeClass.Members.Add(snippetMember);

                    string snippetCode2 = $"{Environment.NewLine}        [Ignore]{Environment.NewLine}        [ReferencedByForeignKey(typeof({textChange}))]{Environment.NewLine}        private Expression<Func<{textChange}, bool>> Where{textChange} =>" +
                        $" ({textChange} {textChange}) => "; // Snippet de código que define o membro completo

                    snippetCode2 += string.Join(" && ", refe.columns.Select(column => $" {textChange}.{column.column} == {column.ReferencedColumnName}"));
                    snippetCode2 += ";" + Environment.NewLine;

                    codeClass.Members.Add(new CodeSnippetTypeMember(snippetCode2));
                }
            }

            codeClass.Members.Add(new CodeSnippetTypeMember() { Text = "\n        " });

            var member = new CodeSnippetTypeMember();
            member.Comments.Add(new CodeCommentStatement("Declare your implementation here"));
            member.Text = GetCustomImplementation(_className) + "        ";
            member.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Custom Implementation"));
            member.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "Custom Implementation"));

            codeClass.Members.Add(member);
        }

        private string GetTableDetaisQuery()
        {
            return @$"
                USE [{_database}]
                SELECT
                    c.name,
                    c.is_nullable,
                    ISNULL(
                        (
                            SELECT
                                i.is_primary_key
                            FROM
                                sys.index_columns ic
                                JOIN sys.indexes i ON ic.object_id = i.object_id
                                AND ic.index_id = i.index_id
                            WHERE
                                i.object_id = c.object_id
                                AND column_id = c.column_id
                                AND i.is_primary_key = 1
                        ),
                        0
                    ) 'isPrimaryKey',
                    c.is_identity,
                    (
                        SELECT
                            TOP(1) CHARACTER_MAXIMUM_LENGTH
                        FROM
                            INFORMATION_SCHEMA.COLUMNS
                        WHERE
                            TABLE_CATALOG = '{_database}'
                            AND TABLE_NAME = '{_tableName}'
                            AND COLUMN_NAME = c.name
                    ) AS max_length,
                    ISNULL(
                        (
                            SELECT
                                COUNT(*)
                            FROM
                                sys.foreign_key_columns AS fkc
                            WHERE
                                fkc.parent_object_id = c.object_id
                                AND fkc.parent_column_id = c.column_id
                        ),
                        0
                    ) AS IsForeignKey,
                    t.name AS typeName,
                    CASE
                        WHEN fk.name IS NOT NULL THEN fk.name
                        ELSE ''
                    END AS ForeignKeyName,
                    CASE
                        WHEN fk.name IS NOT NULL THEN OBJECT_NAME(fk.referenced_object_id)
                        ELSE ''
                    END AS ReferencedTableName,
                    CASE
                        WHEN fk.name IS NOT NULL THEN COL_NAME(
                            fkc.referenced_object_id,
                            fkc.referenced_column_id
                        )
                        ELSE ''
                    END AS ReferencedColumnName,
                    CASE
                        WHEN fk.name IS NOT NULL THEN CASE
                            WHEN (
                                SELECT
                                    COUNT(*)
                                FROM
                                    sys.foreign_key_columns
                                WHERE
                                    referenced_object_id = fk.referenced_object_id
                            ) = 1 THEN '1 para 1 Relationship'
                            ELSE '1 para N Relationship'
                        END
                        ELSE CASE
                            WHEN (
                                SELECT
                                    COUNT(*)
                                FROM
                                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE
                                    CONSTRAINT_TYPE = 'PRIMARY KEY'
                                    AND TABLE_NAME = OBJECT_NAME(c.object_id)
                            ) > 1 THEN 'N para N Relationship'
                            ELSE ''
                        END
                    END AS RelationshipType
                FROM
                    sys.all_columns AS c
                    JOIN sys.types AS t ON c.system_type_id = t.system_type_id
                    LEFT JOIN sys.foreign_key_columns AS fkc ON c.object_id = fkc.parent_object_id
                    AND c.column_id = fkc.parent_column_id
                    LEFT JOIN sys.foreign_keys AS fk ON fkc.constraint_object_id = fk.object_id
                WHERE
                    c.object_id = OBJECT_ID('{_database}..{_tableName}')";
        }

        private string GetQueryReferencedByFk()
        {
            return $@"USE [{_database}]
                SELECT
                    DISTINCT OBJECT_NAME(fk.parent_object_id) AS ReferencingTableName,
                    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ReferencingColumnName,
                    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
                    c.name AS ReferencedColumnName
                FROM
                    sys.foreign_key_columns AS fkc
                    JOIN sys.foreign_keys AS fk ON fkc.constraint_object_id = fk.object_id
                    JOIN sys.columns AS c ON fkc.referenced_object_id = c.object_id
                    AND fkc.referenced_column_id = c.column_id
                WHERE
                    fkc.referenced_object_id = OBJECT_ID('{_database}..{_tableName}')";
        }
    }
}
