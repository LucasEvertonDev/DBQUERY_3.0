using System;
using System.Linq.Expressions;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.CustomSelect;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.Select
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SelectBaseStep<TEntity> : SelectOrderByStep<TEntity>, IPersistenceStep where TEntity : EntityBase
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
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public SelectAfterWhereStep<TEntity> Where(Expression<Func<TEntity, bool>> expression = null)
        {
            return InstanceNextLevel<SelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de join da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddJoin(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <param name="expression">Parametro usado para indicar as condições de contrato do join("ON").</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterJoinStep.
        /// </returns>
        public CustomSelectAfterJoinStep<TEntity> Join<Entity1, Entity2>(Expression<Func<Entity1, Entity2, bool>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterJoinStep<TEntity>>(_levelFactory.PrepareJoinStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de join da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddJoin(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <param name="expression">Parametro usado para indicar as condições de contrato do join("ON").</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterJoinStep.
        /// </returns>
        public CustomSelectAfterJoinStep<TEntity> JoinEmpresaFilial<Entity1, Entity2>(Expression<Func<Entity1, Entity2, bool>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterJoinStep<TEntity>>(_levelFactory.PrepareJoinEmpresaFilialStep(expression));
        }


        /// <summary>
        ///     Responsável pela etapa de join da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddJoin(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <param name="expression">Parametro usado para indicar as condições de contrato do join("ON").</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterJoinStep.
        /// </returns>
        public CustomSelectAfterJoinStep<TEntity> LeftJoin<Entity1, Entity2>(Expression<Func<Entity1, Entity2, bool>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterJoinStep<TEntity>>(_levelFactory.PrepareLeftJoinStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de left join da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddJoin(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <param name="expression">Parametro usado para indicar as condições de contrato do join("ON").</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterJoinStep.
        /// </returns>
        public CustomSelectAfterJoinStep<TEntity> LeftJoinEmpresaFilial<Entity1, Entity2>(Expression<Func<Entity1, Entity2, bool>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterJoinStep<TEntity>>(_levelFactory.PrepareLeftJoinEmpresaFilialStep(expression));
        }
    }
}
