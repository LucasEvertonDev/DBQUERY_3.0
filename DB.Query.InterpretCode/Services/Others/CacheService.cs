using DB.Query.Core.Annotations.Entity;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Caching;

namespace DB.Query.InterpretCode.Services.Others
{
    /// <summary>
    /// Serviço para gerenciamento de cache em memória, visando otimizar o acesso a propriedades e seus atributos.
    /// </summary>
    public class MemoryCacheService
    {
        private static readonly MemoryCache Cache = new MemoryCache("DbQueryCache", new NameValueCollection
        {
            { "CacheMemoryLimitMegabytes", "100" }, // Limite de cache de 20 MB
            { "PhysicalMemoryLimitPercentage", "95" } // Limite de 95% da memória física
        });

        /// <summary>
        /// Verifica se a propriedade existe no tipo e a armazena no cache se encontrada.
        /// </summary>
        /// <param name="type">Tipo do qual a propriedade será verificada.</param>
        /// <param name="propertyName">Nome da propriedade a ser verificada.</param>
        /// <returns>Retorna true se a propriedade existe, caso contrário false.</returns>
        public static bool ContainsProperty(Type type, string propertyName)
        {
            var cacheKey = $"{type.FullName}.{propertyName}";

            // Tenta obter o valor do cache
            if (Cache.Contains(cacheKey))
            {
                return true; // A propriedade já está no cache
            }

            // Tenta encontrar a propriedade
            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo != null)
            {
                // Armazena no cache se encontrada
                Cache.Set(cacheKey, propertyInfo, DateTimeOffset.Now.AddMinutes(60));
                return true; // Propriedade encontrada
            }

            return false; // Propriedade não encontrada
        }

        /// <summary>
        /// Obtém o nome de exibição da propriedade, armazenando em cache o valor encontrado.
        /// </summary>
        /// <param name="prop">Informação da propriedade a ser analisada.</param>
        /// <returns>Nome de exibição da propriedade.</returns>
        public static string GetDisplayName(PropertyInfo prop)
        {
            var cacheKey = prop.Name;

            // Tenta obter o valor do cache
            if (Cache.Contains(cacheKey))
            {
                return (string)Cache.Get(cacheKey); // Retorna o valor armazenado no cache
            }

            // Busca o atributo ColumnAttribute para obter o nome de exibição
            var columnAttribute = prop.GetCustomAttribute<ColumnAttribute>();
            var displayName = columnAttribute?.DisplayName ?? prop.Name;

            // Armazena o nome de exibição no cache
            Cache.Set(cacheKey, displayName, DateTimeOffset.Now.AddMinutes(60));

            return displayName; // Retorna o nome de exibição
        }

        public static string GetCache(string key, Func<string> func)
        {
            if (Cache.Contains(key))
            {
                return (string)Cache.Get(key); // Retorna o valor armazenado no cache
            }
            else
            {
                var retorno = func();

                if (!string.IsNullOrEmpty(retorno))
                {
                    Cache.Set(key, retorno, DateTimeOffset.Now.AddMinutes(60));
                }

                return retorno;
            }
        }
    }
}
