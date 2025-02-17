using DB.Query.Cli.CodeForge.Stored;
using Microsoft.CSharp;
using Microsoft.Data.SqlClient;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Text;

namespace DB.Query.Cli.CodeForge
{
    public abstract class CodeForge
    {
        protected string _conexao;
        protected string _tableName;
        protected string _database;
        protected string _schema;
        protected string GetAndSet = "{ get; set; }";

        public abstract CodeCompileUnit Init();

        protected DataTable ExecuteQuery(string query)
        {
            var dataTable = new DataTable();
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                SqlConnection Sql_Conexao = OpenConnection();

                SqlCommand Sql_Comando = new SqlCommand(query, Sql_Conexao);
                SqlDataAdapter sql_Ada = new SqlDataAdapter(Sql_Comando);
                sql_Ada.Fill(dataTable);
            }
            catch
            {
                return null;
            }
            return dataTable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static CodeTypeDeclaration CreateClass(string name)
        {
            /// https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-create-a-class-using-codedom
            CodeTypeDeclaration result = new CodeTypeDeclaration(name);
            result.Attributes = MemberAttributes.Public;
            return result;
        }
        
        protected CodeAttributeDeclaration CreateAttribute(string attributeName, params CodeAttributeArgument[] arguments)
        {
            var attr = new CodeAttributeDeclaration();
            attr.Name = attributeName;
            if(arguments != null && arguments[0] != null) 
                attr.Arguments.AddRange(arguments);
            return attr;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetCustomImplementation(string className)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var folder = Path.Combine(AppContext.BaseDirectory, "Entities");

            if (File.Exists(string.Concat(folder, className, ".cs")))
            {
                string[] lines = System.IO.File.ReadAllLines(string.Concat(folder, className, ".cs"));

                bool getLines = false;

                foreach (string line in lines)
                {
                    // Use a tab to indent each line of the file.
                    if (line.Contains("#region Custom Implementation"))
                    {
                        getLines = true;
                    }
                    else if (line.Contains("#endregion"))
                    {
                        getLines = false;
                    }

                    if (!line.Contains("#region Custom Implementation") && getLines && !line.Contains("Declare your implementation here"))
                    {
                        stringBuilder.AppendLine(line);
                    }
                }
            }
            else
            {
                stringBuilder.AppendLine("");
            }

            return stringBuilder.ToString();
        }

       protected void GenerateClass(CodeCompileUnit compileUnit, string folder, string className)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            StringWriter writer = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter(writer, " ");

            provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions()
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = false
            });

            tw.Close();
            WriteFileClass(writer, folder, className);
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteFileClass(StringWriter writer, string folder, string className)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(folder, $"{className}.cs"), false))
            {
                var code = writer.ToString().Replace("};", "}").Replace("public class", "public partial class");
                sw.Write(code);
                sw.Close();
            }
        }

        protected Type ValidateType(Type type, bool isnullable)
        {
            Type typeColumn = null;

            if (type == typeof(byte) && !isnullable)
            {
                typeColumn = typeof(int);
            }
            else if (type == typeof(byte) && isnullable)
            {
                typeColumn = typeof(int?);
            }
            else if (type == typeof(int) && isnullable)
            {
                typeColumn = typeof(int?);
            }
            else if (type == typeof(decimal) && isnullable)
            {
                typeColumn = typeof(decimal?);
            }
            else if (type == typeof(long) && isnullable)
            {
                typeColumn = typeof(long?);
            }
            else if (type == typeof(DateTime) && isnullable)
            {
                typeColumn = typeof(DateTime?);
            }
            else if (type == typeof(double) && isnullable)
            {
                typeColumn = typeof(double?);
            }
            else if (type == typeof(bool) && isnullable)
            {
                typeColumn = typeof(bool?);
            }

            return typeColumn ?? type;
        }

        protected SqlConnection OpenConnection()
        {
            SqlConnection Sql_Conexao = new SqlConnection(_conexao);
            Sql_Conexao.Open();

            if (!string.IsNullOrEmpty(_database))
            {
                Sql_Conexao.ChangeDatabase(_database);
            }

            return Sql_Conexao;
        }

        protected Dictionary<string, ParameterTypeMap> typeMapper = new Dictionary<string, ParameterTypeMap>
        {
            {
                "bit", new ParameterTypeMap
                {
                    ClrType = typeof(bool),
                    DbType = SqlDbType.Bit,
                    LengthDivisor = null
                }
            },
            {
                "tinyint", new ParameterTypeMap
                {
                    ClrType = typeof(int),
                    DbType = SqlDbType.TinyInt,
                    LengthDivisor = null
                }
            },
            {
                "smallint", new ParameterTypeMap
                {
                    ClrType = typeof(short),
                    DbType = SqlDbType.SmallInt,
                    LengthDivisor = null
                }
            },
            {
                "int", new ParameterTypeMap
                {
                    ClrType = typeof(int),
                    DbType = SqlDbType.Int,
                    LengthDivisor = null
                }
            },
            {
                "bigint", new ParameterTypeMap
                {
                    ClrType = typeof(long?),
                    DbType = SqlDbType.BigInt,
                    LengthDivisor = null
                }
            },
            {
                "varchar", new ParameterTypeMap
                {
                    ClrType = typeof(string),
                    DbType = SqlDbType.VarChar,
                    LengthDivisor = 1
                }
            },
            {
                "nvarchar", new ParameterTypeMap
                {
                    ClrType = typeof(string),
                    DbType = SqlDbType.NVarChar,
                    LengthDivisor = 2
                }
            },
            {
                "text", new ParameterTypeMap
                {
                    ClrType = typeof(string),
                    DbType = SqlDbType.Text,
                    LengthDivisor = 2
                }
            },
            {
                "char", new ParameterTypeMap
                {
                    ClrType = typeof(string),
                    DbType = SqlDbType.Char,
                    LengthDivisor = 1
                }
            },
            {
                "nchar", new ParameterTypeMap
                {
                    ClrType = typeof(string),
                    DbType = SqlDbType.NChar,
                    LengthDivisor = 2
                }
            },
            {
                "date", new ParameterTypeMap
                {
                    ClrType = typeof(DateTime),
                    DbType = SqlDbType.Date,
                    LengthDivisor = null
                }
            },
            {
                "datetime", new ParameterTypeMap
                {
                    ClrType = typeof(DateTime),
                    DbType = SqlDbType.DateTime,
                    LengthDivisor = null
                }
            },
            {
                "smalldatetime", new ParameterTypeMap
                {
                    ClrType = typeof(DateTime),
                    DbType = SqlDbType.SmallDateTime,
                    LengthDivisor = null
                }
            },
            {
                "time", new ParameterTypeMap
                {
                    ClrType = typeof(TimeSpan),
                    DbType = SqlDbType.Time,
                    LengthDivisor = null
                }
            },
            {
                "varbinary", new ParameterTypeMap
                {
                    ClrType = typeof(byte[]),
                    DbType = SqlDbType.VarBinary,
                    LengthDivisor = null
                }
            },
            {
                "money", new ParameterTypeMap
                {
                    ClrType = typeof(decimal),
                    DbType = SqlDbType.Money,
                    LengthDivisor = null
                }
            },
            {
                "numeric", new ParameterTypeMap
                {
                    ClrType = typeof(decimal),
                    DbType = SqlDbType.Decimal,
                    LengthDivisor = null
                }
            },
            {
                "decimal", new ParameterTypeMap
                {
                    ClrType = typeof(decimal),
                    DbType = SqlDbType.Decimal,
                    LengthDivisor = null
                }
            },
            {
                "real", new ParameterTypeMap
                {
                    ClrType = typeof(float),
                    DbType = SqlDbType.Real,
                    LengthDivisor = null
                }
            },
            {
                "float", new ParameterTypeMap
                {
                    ClrType = typeof(double),
                    DbType = SqlDbType.Float,
                    LengthDivisor = null
                }
            },
            {
                "uniqueidentifier", new ParameterTypeMap
                {
                    ClrType = typeof(Guid),
                    DbType = SqlDbType.UniqueIdentifier,
                    LengthDivisor = null
                }
            },
            {
                "datetimeoffset", new ParameterTypeMap
                {
                    ClrType = typeof(DateTimeOffset),
                    DbType = SqlDbType.DateTimeOffset,
                    LengthDivisor = null
                }
            },
            {
                "xml", new ParameterTypeMap
                {
                    ClrType = typeof(SqlXml),
                    DbType = SqlDbType.Xml,
                    LengthDivisor = null
                }
            }
        };
    }
}
