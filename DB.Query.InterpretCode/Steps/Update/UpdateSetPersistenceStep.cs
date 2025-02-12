using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using System;
using System.Linq.Expressions;

namespace DB.Query.InterpretCode.Steps.Update
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class UpdateSetPersistenceStep<TEntity> : UpdatePersistenceStep<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <returns>
        ///     Retorno do tipo PersistenceStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public UpdatePersistenceStep<TEntity> Where(Expression<Func<TEntity, bool>> expression = null)
        {
            return InstanceNextLevel<UpdatePersistenceStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }
    }
}
