using System;
using System.Linq.Expressions;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.InterpretCode.Steps.Select;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.CustomSelect
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CustomSelectOrderByStep<TEntity> : CustomSelectPersistenceStep<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <returns>
        ///     Retorno do tipo SelectAfterOrderByStep.
        /// </returns>
        public SelectAfterOrderByStep<TEntity> OrderBy(Expression<Func<TEntity, dynamic>> expression)
        {
            return InstanceNextLevel<SelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <returns>
        ///     Retorno do tipo SelectAfterOrderByStep.
        /// </returns>
        public SelectAfterOrderByStep<TEntity> OrderByDesc(Expression<Func<TEntity, dynamic>> expression)
        {
            return InstanceNextLevel<SelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <returns>
        ///     Retorno do tipo SelectAfterOrderByStep.
        /// </returns>
        public SelectAfterOrderByStep<TEntity> OrderBy(Expression<Func<TEntity, dynamic[]>> expression)
        {
            return InstanceNextLevel<SelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <returns>
        ///     Retorno do tipo SelectAfterOrderByStep.
        /// </returns>
        public SelectAfterOrderByStep<TEntity> OrderByDesc(Expression<Func<TEntity, dynamic[]>> expression)
        {
            return InstanceNextLevel<SelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1>(Expression<Func<Entity1, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1>(Expression<Func<Entity1, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1>(Expression<Func<Entity1, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1>(Expression<Func<Entity1, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam> 
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }


        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam> 
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }
        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }


        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }


        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderBy<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByAscStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Order by By da query
        ///     <para>
        ///       A expressão deve listar as colunas que ordenarão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddOrderBy(string, Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as colunas a serem ordenadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterOrderByStep.
        /// </returns>
        public CustomSelectAfterOrderByStep<TEntity> OrderByDesc<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterOrderByStep<TEntity>>(_levelFactory.PrepareOrderByDescStep(expression));
        }
    }
}
