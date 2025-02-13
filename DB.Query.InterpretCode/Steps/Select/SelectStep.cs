using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Select
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SelectStep<TEntity> : SelectBaseStep<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <returns>
        ///     Retorno do tipo SelectAfterDistinctStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public SelectAfterDistinctStep<TEntity> Distinct()
        {
            return InstanceNextLevel<SelectAfterDistinctStep<TEntity>>(_levelFactory.PrepareDistinctStep());
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <returns>
        ///     Retorno do tipo SelectAfterTopStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public new SelectAfterTopStep<TEntity> Top(int top)
        {
            return InstanceNextLevel<SelectAfterTopStep<TEntity>>(_levelFactory.PrepareTopStep(top));
        }
    }
}
