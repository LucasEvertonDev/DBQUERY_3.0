using DB.Query.Cli.CodeForge.Stored;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace DB.Query.Cli.CodeForge
{
    public class StoredForge : CodeForge
    {
        private string _className;
        private bool _normalizeColumns;

        public StoredForge(string conexao, string database, string tableName, string className, bool normalizeColumns)
        {
            _conexao = conexao;
            _tableName = tableName;
            _database = database;
            _className = className;
            _normalizeColumns = normalizeColumns;
        }

        public override CodeCompileUnit Init()
        {
            var classe = "";

            var parameters = new ParametersForge(_conexao, _database, _tableName, _className, _normalizeColumns);
            var result = new ResultsForge(_conexao, _database, _tableName, _className, _normalizeColumns);

            classe = GetCode(parameters.Init());

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Storeds");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(path, $"{_className}.cs"), false))
            {
                sw.Write(classe);
                sw.Close();
            }
            return null;
        }

        private string GetCode(CodeCompileUnit codeCompileUnit)
        {
            if (codeCompileUnit is null)
            {
                return "";
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();
            StringWriter writer = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter(writer, " ");

            provider.GenerateCodeFromCompileUnit(codeCompileUnit, tw, new CodeGeneratorOptions()
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = false
            });

            tw.Close();
            return writer.ToString().Replace("};", "}").Replace("public class", "public partial class");
        }
    }
}
