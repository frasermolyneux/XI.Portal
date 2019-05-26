using System;
using System.Threading.Tasks;

namespace XI.Portal.Library.Logging
{
    public interface IDatabaseLogger
    {
        Task CreateSystemLogAsync(string level, string message, Exception ex);
        Task CreateSystemLogAsync(string level, string message, string error);
        Task CreateSystemLogAsync(string level, string message);
        Task CreateUserLogAsync(string applicationUserId, string message);
    }
}