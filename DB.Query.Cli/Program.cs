using DB.Query.Cli.CodeForge;
using Microsoft.Extensions.Configuration;
using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;

// dotnet tool update dbquerycli
// dotnet new tool-manifest
// dotnet tool install dbquerycli --version 1.2.0
// dotnet tool run dbquerycli -- --help

// Configure appsettings.json


class Program
{
    static async Task Main(string[] args)
    {

        // Define the root command with its options

        var type = new Option<string>(
            "--type",
            description: "Tipo de operação (table ou stored)",
            getDefaultValue: () => "table");

        var database = new Option<string>(
            "--database",
            description: "Nome do banco de dados");

        var tableName = new Option<string>(
            "--tableName",
            description: "Nome da tabela");

        var className = new Option<string>(
            "--className",
            description: "Nome da classe");

        var normalizeColumns = new Option<bool>(
            "--normalizeColumns",
            description: "Normalizar colunas");

        var rootCommand = new RootCommand();
        rootCommand.AddOption(type);
        rootCommand.AddOption(database);
        rootCommand.AddOption(tableName);
        rootCommand.AddOption(className);
        rootCommand.AddOption(normalizeColumns);

        rootCommand.Description = "Ferramenta CLI para consultas de banco de dados";

        // Set the handler for the root command
        rootCommand.SetHandler((string type, string database, string tableName, string className, bool normalizeColumns) =>
        {
            if (type != "table" && type != "stored")
            {
                Console.WriteLine("Erro: O parâmetro --type deve ser 'table' ou 'stored'.");
                return;
            }

            var currentDirectory = Directory.GetCurrentDirectory();
            var configFilePath = Path.Combine(currentDirectory, "appsettings.json");

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine($"Erro: O arquivo de configuração 'appsettings.json' não foi encontrado no diretório atual: {currentDirectory}");
                return;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            if (type.Equals("table"))
            {
                var forge = new EntityForge(configuration.GetSection("ConnectionStrings:DefaultConnection").Value, database, tableName, className, normalizeColumns);

                forge.Init();

                Console.WriteLine("Arquivo gerado com sucesso.");
            }
            else if (type.Equals("stored"))
            {
                var forge = new StoredForge(configuration.GetSection("ConnectionStrings:DefaultConnection").Value, database, tableName, className, normalizeColumns);

                forge.Init();

                Console.WriteLine("Arquivo gerado com sucesso.");
            }

        }, type, database, tableName, className, normalizeColumns);

        // Invoke the command
        await rootCommand.InvokeAsync(args);
    }
}