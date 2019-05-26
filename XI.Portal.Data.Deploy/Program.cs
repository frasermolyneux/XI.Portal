using System;
using System.Configuration;
using Serilog;
using XI.Portal.Data.Core.Context;

namespace XI.Portal.Data.Deploy
{
    internal class Program
    {
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Log.Information("Ensuring that database has been deployed");

            var connectionString = ConfigurationManager.ConnectionStrings["PortalContext"];

            if (connectionString == null || connectionString.ConnectionString == "__ConnectionString__")
            {
                Log.Error("Connection string has not been configured correctly in app settings");
                Environment.Exit(-1);
            }

            var contextProvider = new ContextProvider(new ContextOptions
            {
                ConnectionString = connectionString.Name
            });

            using (var context = contextProvider.GetContext())
            {
                new DefaultDataSeed().SeedData(context);
            }

            Log.Information("Database has been deployed");
        }
    }
}