using System;
using System.Linq;

namespace DB.Query.Tests.Helpers
{
    public static class StringHelper
    {
        // Método para normalizar a consulta (remover espaços extras de formatação)
        public static string NormalizeQuery(string input)
        {
            // Remove espaços extras antes ou depois de palavras e também as quebras de linha
            return string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
        }
    }
}
