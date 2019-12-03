using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.BanSource
{
    public class DatabaseBanSource : IDatabaseBanSource
    {
        private readonly IContextProvider contextProvider;

        public DatabaseBanSource(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public IDictionary<string, string> GetBans(GameType gameType)
        {
            using (var context = contextProvider.GetContext())
            {
                var adminActions = context.AdminActions.Include(aa => aa.Player)
                    .Where(aa => aa.Player.GameType == gameType
                                 && (aa.Type == AdminActionType.Ban && aa.Expires == null
                                     || aa.Type == AdminActionType.TempBan && aa.Expires > DateTime.UtcNow))
                    .OrderByDescending(aa => aa.Created).ToList();

                var dictionary = new Dictionary<string, string>();
                foreach (var aa in adminActions)
                    if (!dictionary.Keys.Contains(aa.Player.Guid))
                        dictionary.Add(aa.Player.Guid, aa.Player.Username);

                return dictionary;
            }
        }
    }
}