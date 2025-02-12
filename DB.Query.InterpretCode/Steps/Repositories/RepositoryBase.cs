using System;
using System.Linq.Expressions;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Repositories.Interfaces;
using DB.Query.InterpretCode.Transaction;
using DB.Query.InterpretCode.Steps.Core;
using DB.Query.InterpretCode.Steps.Delete;
using DB.Query.InterpretCode.Steps.Insert;
using DB.Query.InterpretCode.Steps.Update;
using DB.Query.Core.Entities;
using DB.Query.InterpretCode.Steps.Select;
using DB.Query.InterpretCode.Steps.CustomSelect;

namespace DB.Query.InterpretCode.Steps.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RepositoryBase<TEntity> : DBQuery<TEntity>, IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        ///     Indica que a ação a ser realizada será um INSERT simples, sem verificações.
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>
        ///     <para><see cref="InterpretInsertService{TEntity}.GenerateInsertScript()">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="domain">Parametro do tipo TEntity, passado como tipo na instância da classe Repository.</param>
        /// <returns>
        ///     Retorno do tipo InsertStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação
        /// </returns>
        public InsertStep<TEntity> Insert(TEntity domain)
        {
            return InstanceNextLevel<InsertStep<TEntity>>(_levelFactory.PrepareInsertStep(domain));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um DELETE simples.
        ///     <para>ATENÇÃO!! O controle de quais entidades serão apagadas deve ser realizado na condição WHERE!</para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretDeleteService{TEntity}.GenerateDeleteScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <returns>
        ///     Retorno do tipo DeleteStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public DeleteStep<TEntity> Delete()
        {
            return InstanceNextLevel<DeleteStep<TEntity>>(_levelFactory.PrepareDeleteStep());
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT simples!
        ///     <para>Um SELECT SIMPLES, nada mais é que um espelho da tabela representada pela entidade (SELECT * FROM TEntity)</para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <returns>
        ///     Retorno do tipo SelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public SelectStep<TEntity> Select()
        {
            return InstanceNextLevel<SelectStep<TEntity>>(_levelFactory.PrepareSimpleSelectStep());
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um UPDATE simples, sem verificações.
        ///     <para>ATENÇÃO!! O controle de quais entidades serão atualizadas deve ser realizado na condição WHERE!</para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretUpdateService{TEntity}.GenerateUpdateScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="domain">Parametro do tipo TEntity, passado como tipo na instância da classe Repository.</param>
        /// <returns>
        ///     Retorno do tipo UpdateStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public UpdateStep<TEntity> Update(TEntity domain)
        {
            return InstanceNextLevel<UpdateStep<TEntity>>(_levelFactory.PrepareUpdateStep(domain));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um INSERT, com uma validação de existência para efetuação do mesmo. 
        ///     <para>ATENÇÃO!! A verificação da existência é feita atravez de todas as propriedades mapeadas, QUE NÃO ESTÂO MAPEADAS COMO IDENTITY(AUTO-INCREMENT)</para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretInsertService{TEntity}.GenerateInsertIfNotExistsScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="domain">Parametro do tipo TEntity, passado como tipo na instância da classe Repository.</param>
        /// <returns>
        ///     Retorno do tipo InsertStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public InsertStep<TEntity> InsertIfNotExists(TEntity domain)
        {
            return InstanceNextLevel<InsertStep<TEntity>>(_levelFactory.PrepareInsertIfNotExistsStep(domain));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um INSERT, com uma validação de existência para efetuação do mesmo.
        ///     Caso já exista é realizado o UPDATE.
        ///     <para>ATENÇÃO!! A etapa WHERE é de suma importância para essa ação.  Pois a mesma irá verificar a existência e também controlar a atualização se necessário.</para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretUpdateService{TEntity}.GenerateInsertOrUpdateScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="domain">Parametro do tipo TEntity, passado como tipo na instância da classe Repository.</param>
        /// <returns>
        ///     Retorno do tipo UpdateStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public UpdateStep<TEntity> InsertOrUpdate(TEntity domain)
        {
            return InstanceNextLevel<UpdateStep<TEntity>>(_levelFactory.PrepareInsertOrUpdateStep(domain));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um INSERT, após uma etapa de DELETE
        ///     <para>ATENÇÃO!! O controle de quais entidades serão apagadas deve ser realizado na condição WHERE!</para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretDeleteService{TEntity}.GenerateDeleteAndInsertScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="domain">Parametro do tipo TEntity, passado como tipo na instância da classe Repository.</param>
        /// <returns>
        ///     Retorno do tipo DeleteStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public DeleteStep<TEntity> DeleteAndInsert(TEntity domain)
        {
            return InstanceNextLevel<DeleteStep<TEntity>>(_levelFactory.PrepareDeleteAndInsertStep(domain));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select(Expression<Func<TEntity, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select(Expression<Func<TEntity, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1>(Expression<Func<Entity1, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1>(Expression<Func<Entity1, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }


        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }
        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        ///  <typeparam name="Entity10"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>

        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <typeparam name="Entity10"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        ///<typeparam name="Entity10"></typeparam>
        ///<typeparam name="Entity11"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>

        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <typeparam name="Entity10"></typeparam>
        /// <typeparam name="Entity11"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        ///<typeparam name="Entity10"></typeparam>
        ///<typeparam name="Entity11"></typeparam>
        ///<typeparam name="Entity12"></typeparam>
        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>

        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <typeparam name="Entity10"></typeparam>
        /// <typeparam name="Entity11"></typeparam>
        /// <typeparam name="Entity12"></typeparam>

        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <typeparam name="Entity10"></typeparam>
        /// <typeparam name="Entity11"></typeparam>
        /// <typeparam name="Entity12"></typeparam>
        /// <typeparam name="Entity13"></typeparam>

        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <typeparam name="Entity10"></typeparam>
        /// <typeparam name="Entity11"></typeparam>
        /// <typeparam name="Entity12"></typeparam>
        /// <typeparam name="Entity13"></typeparam>
        /// <typeparam name="Entity14"></typeparam>

        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <typeparam name="Entity10"></typeparam>
        /// <typeparam name="Entity11"></typeparam>
        /// <typeparam name="Entity12"></typeparam>
        /// <typeparam name="Entity13"></typeparam>
        /// <typeparam name="Entity14"></typeparam>
        /// <typeparam name="Entity15"></typeparam>

        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        ///     Indica que a ação a ser realizada será um SELECT! 
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para> 
        ///     <para><see cref="InterpretSelectService{TEntity}.GenerateSelectScript">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <typeparam name="Entity10"></typeparam>
        /// <typeparam name="Entity11"></typeparam>
        /// <typeparam name="Entity12"></typeparam>
        /// <typeparam name="Entity13"></typeparam>
        /// <typeparam name="Entity14"></typeparam>
        /// <typeparam name="Entity15"></typeparam>
        /// <typeparam name="Entity16"></typeparam>

        /// <param name="expression">Parametro usado para indicar as colunas de retorno.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectStep<TEntity> Select<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15, Entity16>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15, Entity16, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectStep<TEntity>>(_levelFactory.PrepareSelectStep(expression));
        }

        /// <summary>
        /// Responsavável por colocar o repository em questão na transação corrente.
        /// </summary>
        /// <param name="dataBaseService"></param>
        /// <returns></returns>
        public void BindTransaction(DBTransaction dataBaseService)
        {
            _transaction = dataBaseService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DBTransaction GetTransaction()
        {
            return _transaction;
        }
    }
}
