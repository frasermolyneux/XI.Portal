using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Library.Logging.Sinks
{
    public class DatabaseSink : ILogEventSink
    {
        private readonly IContextProvider contextProvider;
        private readonly int minimumLevel;

        public DatabaseSink(IContextProvider contextProvider, int minimumLevel)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.minimumLevel = minimumLevel;
        }

        public void Emit(LogEvent logEvent)
        {
            var logEventLevel = (int) logEvent.Level;
            if (logEventLevel < minimumLevel)
                return;

            using (var context = contextProvider.GetContext())
            {
                var message = logEvent.RenderMessage();

                context.SystemLogs.Add(new SystemLog
                {
                    Level = logEvent.Level.ToString(),
                    Message = message,
                    Error = logEvent.Exception?.ToString(),
                    Timestamp = DateTime.UtcNow
                });

                context.SaveChanges();
            }
        }
    }

    public static class DatabaseSinkExtensions
    {
        public static LoggerConfiguration DatabaseSink(this LoggerSinkConfiguration loggerConfiguration, IContextProvider contextProvider, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            var minumumLevel = (int) restrictedToMinimumLevel;
            return loggerConfiguration.Sink(new DatabaseSink(contextProvider, minumumLevel));
        }
    }
}