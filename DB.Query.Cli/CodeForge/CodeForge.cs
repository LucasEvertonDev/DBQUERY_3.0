using Microsoft.Data.SqlClient;
using System.Data;

namespace DB.Query.Cli.CodeForge
{
    public abstract class CodeForge
    {
        protected string _conexao;
        protected string _tableName;
        protected string _database;
        protected string _schema;

        public abstract void Init();

        public DataTable ExecuteQuery(string query)
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

        private SqlConnection OpenConnection()
        {
            SqlConnection Sql_Conexao = new SqlConnection(_conexao);
            Sql_Conexao.Open();

            if (!string.IsNullOrEmpty(_database))
            {
                Sql_Conexao.ChangeDatabase(_database);
            }

            return Sql_Conexao;
        }
    }
}
