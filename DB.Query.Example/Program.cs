using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
          .SetBasePath(AppContext.BaseDirectory)
          .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

IConfiguration configuration = builder.Build();




