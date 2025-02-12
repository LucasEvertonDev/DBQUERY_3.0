using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.CustomSelect
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CustomSelectAfterOrderByStep<TEntity> : CustomSelectOrderByStep<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
        /// <summary>
        ///    Indica que a ação a ser realizada será uma paginação! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        public CustomSelectPersistenceStep<TEntity> Pagination(int pageSize, int pageNumber)
        {
            return InstanceNextLevel<CustomSelectPersistenceStep<TEntity>>(_levelFactory.PreparePaginationStep(pageSize, pageNumber));
        }
    }
}
