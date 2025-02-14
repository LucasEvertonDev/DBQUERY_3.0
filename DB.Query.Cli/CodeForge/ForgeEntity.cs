namespace DB.Query.Cli.CodeForge
{
    public class ForgeEntity : CodeForge
    {
        public ForgeEntity(string conexao, string database, string schema, string tableName)
        {
            _conexao = conexao;
            _tableName = tableName;
            _database = database;
            _schema = schema;
        }

        public override void Init()
        {
            
        }
    }
}
