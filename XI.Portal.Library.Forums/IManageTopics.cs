using System;
using System.Threading.Tasks;

namespace XI.Portal.Library.Forums
{
    public interface IManageTopics
    {
        Task CreateTopicForAdminAction(Guid id);
        Task UpdateTopicForAdminAction(Guid id);
    }
}