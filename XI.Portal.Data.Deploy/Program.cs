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

            var contextProvider = new ContextProvider(new Da);

            using (var context = contextProvider.GetContext())
            {
                new DefaultDataSeed().SeedData(context);
            }

            Log.Information("Database has been deployed");
        }
    }
}