using DB.Query.InterpretCode.Steps.Repositories.Interfaces;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Repositories
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Query<TEntity> : QueryBase<TEntity>, IQuery, IQuery<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// Indica se deve ser considerado os apelidos dos parâmetros das expressions montadas como chave 'AS' para acesso as tabelas.
        /// <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>
        /// </summary>
        /// <param name="alias">O parâmetro deve conter o nome o qual será associado a classe instanciada na query</param>
        /// <example>query.UseAlias("ci").Select().Execute(). Tal expressão dará origem a seguinte query: SELECT * FROM table as ci</example>
        /// <returns>Retorno do tipo QueryAfterAlias, responsável por garantir o controle da próxiam etapa.
        /// Impedindo que esse método seja novamente chamado na mesma operação</returns>
        public QueryAfterAlias<TEntity> UseAlias(string alias)
        {
            return InstanceNextLevel<QueryAfterAlias<TEntity>>(_levelFactory.PrepareAliasStep(alias));
        }
    }
}
