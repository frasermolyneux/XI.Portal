using System;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Library.Logging
{
    public class DatabaseLogger : IDatabaseLogger
    {
        private readonly IContextProvider contextProvider;

        public DatabaseLogger(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public async Task CreateSystemLogAsync(string level, string message, Exception ex)
        {
            using (var context = contextProvider.GetContext())
            {
                context.SystemLogs.Add(new SystemLog
                {
                    Level = level,
                    Message = message,
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });

                if (ex.InnerException != null)
                    context.SystemLogs.Add(new SystemLog
                    {
                        Level = level,
                        Message = message,
                        Error = ex.InnerException.Message,
                        Timestamp = DateTime.UtcNow
                    });

                await context.SaveChangesAsync();
            }
        }

        public async Task CreateSystemLogAsync(string level, string message, string error)
        {
            using (var context = contextProvider.GetContext())
            {
                context.SystemLogs.Add(new SystemLog
                {
                    Level = level,
                    Message = message,
                    Error = error,
                    Timestamp = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }

        public async Task CreateSystemLogAsync(string level, string message)
        {
            using (var context = contextProvider.GetContext())
            {
                context.SystemLogs.Add(new SystemLog
                {
                    Level = level,
                    Message = message,
                    Error = string.Empty,
                    Timestamp = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }

        public async Task CreateUserLogAsync(string applicationUserId, string message)
        {
            using (var context = contextProvider.GetContext())
            {
                var user = context.Users.SingleOrDefault(u => u.Id == applicationUserId);

                //if (user == null)
                //{
                //    await CreateSystemLogAsync("Error", $"Failed to create user log as user not found for {applicationUserId}", message);
                //    return;
                //}

                context.UserLogs.Add(new UserLog
                {
                    ApplicationUser = user,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }
    }
}