using System;
using System.Linq.Expressions;
using DB.Query.InterpretCode.Services.InterpretServices;
using DB.Query.InterpretCode.Steps.Core.Interfaces;
using DB.Query.Core.Entities;

namespace DB.Query.InterpretCode.Steps.CustomSelect
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CustomSelectAfterJoinStep<TEntity> : CustomSelectOrderByStep<TEntity>, IPersistenceStep where TEntity : EntityBase
    {
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

        #region Where
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
        public CustomSelectAfterWhereStep<TEntity> Where(Expression<Func<TEntity, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="TEntity1"></typeparam>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<TEntity1>(Expression<Func<TEntity1, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2>(Expression<Func<Entity1, Entity2, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <typeparam name="Entity9"></typeparam>
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
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
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
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
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
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
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
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
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
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
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
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
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa de filtros da query
        ///     <para>
        ///       A expressão deve ter um resultado booleano, porém é de suma importância na comparação de propriedade evitar: associações, parses e funções que não foram tratadas. Tendo como exceção os paramêtros passados para a consulta.
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddWhere(Expression)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicar as condições da query.</param>
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
        /// <returns>
        ///     Retorno do tipo SelectAfterWhereStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterWhereStep<TEntity> Where<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15, Entity16>
            (Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, Entity9, Entity10, Entity11, Entity12, Entity13, Entity14, Entity15, Entity16, bool>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterWhereStep<TEntity>>(_levelFactory.PrepareWhereStep(expression));
        }


        #endregion

        #region Group by
        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy(Expression<Func<TEntity, dynamic>> expression = null)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public virtual CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1>(Expression<Func<Entity1, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy(Expression<Func<TEntity, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1>(Expression<Func<Entity1, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2>(Expression<Func<Entity1, Entity2, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3>(Expression<Func<Entity1, Entity2, Entity3, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4>(Expression<Func<Entity1, Entity2, Entity3, Entity4, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, dynamic[]>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        /// <summary>
        ///     Responsável pela etapa Group By da query
        ///     <para>
        ///       A expressão deve listar as colunas que agruparão a query
        ///     </para>
        ///     <para><see href="https://dev.azure.com/DevTeamFivenBR/IT%20Fiven%20BR/_git/SIGN%20QUERY?version=GBmain">Consulte a documentação.</see></para>    
        ///     <para><see cref="InterpretService{TEntity}.AddGroupBy(Expression, string)">Navegue para o método de geração script.</see></para>
        /// </summary>
        /// <param name="expression">Parametro usado para indicaras propriedades que serão agrupadas.</param>
        /// <typeparam name="Entity1"></typeparam>
        /// <typeparam name="Entity2"></typeparam>
        /// <typeparam name="Entity3"></typeparam>
        /// <typeparam name="Entity4"></typeparam>
        /// <typeparam name="Entity5"></typeparam>
        /// <typeparam name="Entity6"></typeparam>
        /// <typeparam name="Entity7"></typeparam>
        /// <typeparam name="Entity8"></typeparam>
        /// <returns>
        ///     Retorno do tipo CustomSelectAfterGroupByStep, responsável por garantir o controle da próxima etapa. Impedindo que esse método seja novamente chamado na mesma operação.
        /// </returns>
        public CustomSelectAfterGroupByStep<TEntity> GroupBy<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8>(Expression<Func<Entity1, Entity2, Entity3, Entity4, Entity5, Entity6, Entity7, Entity8, dynamic>> expression)
        {
            return InstanceNextLevel<CustomSelectAfterGroupByStep<TEntity>>(_levelFactory.PrepareGroupByStep(expression));
        }

        #endregion
    }
}
