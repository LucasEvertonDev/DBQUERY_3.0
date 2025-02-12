using DB.Query.Core.Annotations;
using DB.Query.Core.Annotations.Entity;
using DB.Query.Core.Entities;
using DB.Query.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColumnAttribute = DB.Query.Core.Annotations.Entity.ColumnAttribute;
using TableAttribute = DB.Query.Core.Annotations.Entity.TableAttribute;

namespace DB.Query.InterpretCode.Factorys
{
    public class EntityAttributesModelFactory<TEntity> where TEntity : EntityBase
    {
        public EntityAttributesModelFactory() { }

        /// <summary>
        /// Interpreta uma entidade e cria um modelo de atributos da entidade.
        /// </summary>
        /// <param name="entity">A entidade a ser interpretada.</param>
        /// <param name="getValues">Indica se os valores das propriedades devem ser obtidos.</param>
        /// <param name="entityAttributes">Modelo de atributos da entidade existente (opcional).</param>
        /// <returns>Modelo de atributos da entidade.</returns>
        public EntityAttributesModel<TEntity> InterpretEntity(TEntity entity, bool getValues, EntityAttributesModel<TEntity> entityAttributes = null)
        {
            // Retorna os atributos já existentes, se fornecidos
            if (entityAttributes != null)
            {
                return entityAttributes;
            }

            var retorno = new EntityAttributesModel<TEntity>();
            var currentType = typeof(TEntity);

            // Obtém e armazena em cache atributos de banco de dados e tabela
            var databaseAttr = currentType.GetCustomAttributes<DatabaseAttribute>().FirstOrDefault();
            var tableAttr = currentType.GetCustomAttributes<TableAttribute>().FirstOrDefault();

            retorno.Database = databaseAttr?.DatabaseName; // Nome do banco de dados
            retorno.Name = tableAttr?.TableName ?? currentType.Name; // Nome da tabela

            // Obtém e armazena em cache as propriedades da entidade
            var props = currentType.GetProperties();
            var propsList = new List<PropsAttributesModel<TEntity>>(props.Length);

            // Itera sobre as propriedades da entidade
            foreach (var prop in props)
            {
                // Ignora propriedades marcadas com o atributo Ignore
                var ignoreAttr = prop.GetCustomAttributes<IgnoreAttribute>().FirstOrDefault();
                if (ignoreAttr != null)
                {
                    continue;
                }

                var propInfo = new PropsAttributesModel<TEntity>
                {
                    Name = prop.GetCustomAttributes<ColumnAttribute>().FirstOrDefault()?.DisplayName ?? prop.Name,
                    Type = prop.PropertyType
                };

                // Define se a propriedade é chave primária
                var primaryKey = prop.GetCustomAttributes<PrimaryKeyAttribute>().FirstOrDefault();
                if (primaryKey != null)
                {
                    propInfo.PrimaryKey = true;
                    propInfo.Identity = primaryKey.Identity;
                }

                // Obtém o valor da propriedade, se necessário
                if (getValues)
                {
                    propInfo.Valor = prop.GetValue(entity);
                }

                propsList.Add(propInfo); // Adiciona à lista de propriedades
            }

            retorno.Props.AddRange(propsList); // Adiciona a lista de propriedades ao retorno
            return retorno; // Retorna o modelo de atributos da entidade
        }
    }
}
