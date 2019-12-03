using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using XI.Portal.App.SyncService.Models;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.Forums;

namespace XI.Portal.App.SyncService.PlayerSync
{
    public class PlayerSynchronizer : IPlayerSynchronizer
    {
        private readonly IContextProvider contextProvider;
        private readonly ILogger logger;
        private readonly IManageTopics manageTopics;

        public PlayerSynchronizer(ILogger logger, IContextProvider contextProvider, IManageTopics manageTopics)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.manageTopics = manageTopics ?? throw new ArgumentNullException(nameof(manageTopics));
        }

        public void SynchronizeLocalPlayers(GameType gameType, List<LocalPlayer> localPlayers, Guid serverId)
        {
            using (var context = contextProvider.GetContext())
            {
                foreach (var localPlayer in localPlayers)
                {
                    var player = context.Players.SingleOrDefault(
                        p => p.Guid == localPlayer.Guid && p.GameType == gameType);

                    if (player != null)
                    {
                        if (context.AdminActions.Any(aa => aa.Player.PlayerId == player.PlayerId && aa.Type == AdminActionType.Ban && aa.Expires == null))
                            continue;

                        var adminAction = new AdminAction
                        {
                            Created = DateTime.UtcNow,
                            Player = player,
                            Text = "Imported from server",
                            Type = AdminActionType.Ban
                        };

                        context.AdminActions.Add(adminAction);
                        context.SaveChanges();
                        manageTopics.CreateTopicForAdminAction(adminAction.AdminActionId);
                    }
                    else
                    {
                        player = new Player2
                        {
                            FirstSeen = DateTime.UtcNow,
                            GameType = gameType,
                            Guid = localPlayer.Guid.ToLower(),
                            IpAddress = null,
                            LastSeen = DateTime.UtcNow,
                            Username = localPlayer.Name
                        };

                        context.Players.Add(player);
                        context.SaveChanges();

                        player = context.Players.SingleOrDefault(
                            p => p.Guid == localPlayer.Guid && p.GameType == gameType);

                        var adminAction = new AdminAction
                        {
                            Created = DateTime.UtcNow,
                            Player = player,
                            Text = "Imported from server",
                            Type = AdminActionType.Ban
                        };

                        context.AdminActions.Add(adminAction);
                        context.SaveChanges();
                        manageTopics.CreateTopicForAdminAction(adminAction.AdminActionId);
                    }
                }
            }
        }
    }
}