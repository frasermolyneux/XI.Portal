using System;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Library.Forums
{
    public class ManageTopics : IManageTopics
    {
        private readonly IContextProvider contextProvider;

        public ManageTopics(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public string ApiKey { get; set; } = "TODO-SECRETANDMANAGEMENT";

        public async Task CreateTopicForAdminAction(Guid id)
        {
            using (var context = contextProvider.GetContext())
            {
                var adminAction = await context.AdminActions.Include(aa => aa.Player).Include(aa => aa.Admin).Where(p => p.AdminActionId == id).SingleAsync();

                var userId = 21145;
                if (adminAction.Admin != null)
                    userId = Convert.ToInt32(adminAction.Admin.XtremeIdiotsId);

                int forumId;

                switch (adminAction.Player.GameType)
                {
                    case GameType.CallOfDuty2:
                        if (adminAction.Type == AdminActionType.Ban || adminAction.Type == AdminActionType.TempBan)
                            forumId = 68;
                        else
                            forumId = 58;
                        break;
                    case GameType.CallOfDuty4:
                        if (adminAction.Type == AdminActionType.Ban || adminAction.Type == AdminActionType.TempBan)
                            forumId = 69;
                        else
                            forumId = 59;
                        break;
                    case GameType.CallOfDuty5:
                        if (adminAction.Type == AdminActionType.Ban || adminAction.Type == AdminActionType.TempBan)
                            forumId = 70;
                        else
                            forumId = 60;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var adminActionThread = PostTopic(forumId, userId, $"{adminAction.Player.Username} - {adminAction.Type}", PostContent(adminAction), adminAction.Type.ToString());
                var topicId = adminActionThread.Item1;

                adminAction.ForumTopicId = topicId;

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateTopicForAdminAction(Guid id)
        {
            using (var context = contextProvider.GetContext())
            {
                var adminAction = await context.AdminActions.Include(aa => aa.Player).Include(aa => aa.Admin).Where(p => p.AdminActionId == id).SingleAsync();

                if (adminAction.ForumTopicId == 0)
                {
                    await CreateTopicForAdminAction(id);
                    return;
                }

                var userId = 21145;
                if (adminAction.Admin != null)
                    userId = Convert.ToInt32(adminAction.Admin.XtremeIdiotsId);

                UpdateTopic(adminAction.ForumTopicId, userId, PostContent(adminAction));
            }
        }


        private Tuple<int, int> PostTopic(int forumId, int authorId, string title, string post, string prefix)
        {
            using (var client = new WebClient())
            {
                var requestParams = new NameValueCollection
                {
                    {"key", ApiKey},
                    {"forum", forumId.ToString()},
                    {"author", authorId.ToString()},
                    {"title", title},
                    {"post", post},
                    {"prefix", prefix}
                };

                var responseBytes = client.UploadValues("https://www.xtremeidiots.com/api/forums/topics", "POST", requestParams);
                var responseBody = Encoding.UTF8.GetString(responseBytes);

                var responseAsDynamic = JsonConvert.DeserializeObject<dynamic>(responseBody);

                int topicId = responseAsDynamic.id;
                int firstPostId = responseAsDynamic.firstPost.id;

                return new Tuple<int, int>(topicId, firstPostId);
            }
        }

        private void UpdateTopic(int topicId, int authorId, string post)
        {
            using (var client = new WebClient())
            {
                var requestParams = new NameValueCollection
                {
                    {"key", ApiKey},
                    {"author", authorId.ToString()},
                    {"post", post}
                };

                var requestUrl = $"https://www.xtremeidiots.com/api/forums/topics/{topicId}";
                client.UploadValues(requestUrl, "POST", requestParams);
            }
        }

        private string PostContent(AdminAction adminAction)
        {
            return $"<p>" +
                   $"   Username: {adminAction.Player.Username}<br>" +
                   $"   IP Address: {adminAction.Player.IpAddress}<br>" +
                   $"   GUID: {adminAction.Player.Guid}<br>" +
                   $"   Player Link: <a href=\"https://portal.xtremeidiots.com/Players/Details/{adminAction.Player.PlayerId.ToString()}\">Portal</a><br>" +
                   $"   Admin Action Created: {adminAction.Created.ToString(CultureInfo.InvariantCulture)}" +
                   $"</p>" +
                   $"<p>" +
                   $"   {adminAction.Text}" +
                   $"</p>" +
                   $"<p>" +
                   $"   <small>Do not edit this post directly as it will be overwritten by the Portal. Add comments on posts below or edit the record in the Portal.</small>" +
                   $"</p>";
        }
    }
}