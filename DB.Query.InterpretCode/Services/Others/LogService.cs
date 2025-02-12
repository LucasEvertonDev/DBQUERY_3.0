using System;
using System.Runtime.InteropServices;

namespace DB.Query.InterpretCode.Services.Others
{
    /// <summary>
    /// Serviço para log de consultas no console.
    /// </summary>
    public class LogService
    {
        /// <summary>
        /// Aloca um console para o aplicativo, permitindo a exibição de saídas de texto.
        /// </summary>
        /// <returns>Retorna true se o console for alocado com sucesso; caso contrário, false.</returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        /// <summary>
        /// 
        /// </summary>
        public static void PrintQuery(string query)
        {
            AllocConsole();
            Console.WriteLine(query);
            Console.WriteLine("");
        }
    }
}
