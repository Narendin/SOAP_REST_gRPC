using ClinicService.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClinicService.Data.EF
{
    public static class ApplicationContextHelper
    {
        public static IConfiguration GetConfiguration()
        {
            // получаем конфигурацию
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile(Constants.AppSettingsJSON, true);

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (File.Exists($"appsettings.{environmentName}.json"))
                builder.AddJsonFile($"appsettings.{environmentName}.json", true);

            IConfiguration config = builder.Build();
            return config;
        }

        public static DbContextOptionsBuilder ConfigureDbContextOptions(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            string? connectionString = configuration[Constants.SQLProvider.ConnectionStringPath];

            return ConfigureDbContextOptions(optionsBuilder, connectionString);
        }

        public static DbContextOptionsBuilder ConfigureDbContextOptions(DbContextOptionsBuilder optionsBuilder, string? connectionString, string? sqlServerProvider = null)
        {
            optionsBuilder.UseSqlServer(connectionString, bo =>
            {
                bo.CommandTimeout(200);
                bo.MigrationsAssembly("ClinicService.Data.MSSQL");
            });

            return optionsBuilder;
        }
    }
}