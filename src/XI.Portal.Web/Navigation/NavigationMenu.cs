﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.Extensions;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Web.Extensions;
using XI.Portal.Web.Navigation.Models;

namespace XI.Portal.Web.Navigation
{
    public class NavigationMenu : INavigationMenu
    {
        private readonly IContextProvider contextProvider;

        public NavigationMenu(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public List<MenuItemModel> GetMenu(IIdentity identity)
        {
            var menuItems = new List<MenuItemModel>();

            if (!identity.IsAuthenticated)
            {
                HomeMenu(menuItems);
                ServersMenu(menuItems, false);
                CommunityMenu(menuItems);
                MapsMenu(menuItems);
                return menuItems;
            }

            var primaryGroup = (XtremeIdiotsGroups) Convert.ToInt32(identity.XtremeIdiotsPrimaryGroupId());

            switch (primaryGroup)
            {
                case XtremeIdiotsGroups.RegisteredUser:
                case XtremeIdiotsGroups.InactiveRegisteredUser:
                case XtremeIdiotsGroups.ClanMember:
                case XtremeIdiotsGroups.InactiveClanMember:
                    //All Users
                    HomeMenu(menuItems);
                    ServersMenu(menuItems, false);
                    CommunityMenu(menuItems);
                    DemoManagerMenu(menuItems);
                    MapsMenu(menuItems);
                    AnalyticsMenu(menuItems);
                    break;
                case XtremeIdiotsGroups.ArmaModerator:
                case XtremeIdiotsGroups.Cod2Moderator:
                case XtremeIdiotsGroups.Cod4Moderator:
                case XtremeIdiotsGroups.Cod5Moderator:
                case XtremeIdiotsGroups.InsurgencyModerator:
                case XtremeIdiotsGroups.MinecraftModerator:
                case XtremeIdiotsGroups.RustModerator:
                case XtremeIdiotsGroups.ArmaAdmin:
                case XtremeIdiotsGroups.Cod2Admin:
                case XtremeIdiotsGroups.Cod4Admin:
                case XtremeIdiotsGroups.Cod5Admin:
                case XtremeIdiotsGroups.InsurgencyAdmin:
                case XtremeIdiotsGroups.MinecraftAdmin:
                case XtremeIdiotsGroups.RustAdmin:
                case XtremeIdiotsGroups.ArmaHeadAdmin:
                case XtremeIdiotsGroups.Cod2HeadAdmin:
                case XtremeIdiotsGroups.Cod4HeadAdmin:
                case XtremeIdiotsGroups.Cod5HeadAdmin:
                case XtremeIdiotsGroups.InsurgencyHeadAdmin:
                case XtremeIdiotsGroups.MinecraftHeadAdmin:
                case XtremeIdiotsGroups.RustHeadAdmin:
                    //All Users
                    HomeMenu(menuItems);
                    ServersMenu(menuItems, true);
                    CommunityMenu(menuItems);
                    DemoManagerMenu(menuItems);
                    MapsMenu(menuItems);
                    AnalyticsMenu(menuItems);

                    //Admins
                    PlayersMenu(menuItems);
                    MonitorMenu(menuItems);
                    break;
                case XtremeIdiotsGroups.SeniorAdmin:
                    //All Users
                    HomeMenu(menuItems);
                    ServersMenu(menuItems, true);
                    CommunityMenu(menuItems);
                    DemoManagerMenu(menuItems);
                    MapsMenu(menuItems);
                    AnalyticsMenu(menuItems);

                    //Admins
                    PlayersMenu(menuItems);
                    MonitorMenu(menuItems);

                    //Senior Admins
                    AdmServersMenu(menuItems);
                    AdmFileMonitorsMenu(menuItems);
                    AdmBanFileMonitorsMenu(menuItems);
                    AdmRconMonitorsMenu(menuItems);
                    AdmMapsMenu(menuItems);
                    AdmUsersMenu(menuItems);
                    AdmLogsMenu(menuItems);
                    AdmMaintenanceMenu(menuItems);
                    break;
            }

            return menuItems;
        }

        private static void ServersMenu(ICollection<MenuItemModel> menuItems, bool globalChatLog)
        {
            var livePlayersMenu = new MenuItemModel("Servers", "Servers", "Index", "server");
            livePlayersMenu.SubMenuItems.Add(new SubMenuItemModel("Index", "Servers", "Index"));

            if (globalChatLog)
                livePlayersMenu.SubMenuItems.Add(new SubMenuItemModel("Global ChatLog", "Servers", "GlobalChatLog"));

            menuItems.Add(livePlayersMenu);
        }

        private static void HomeMenu(ICollection<MenuItemModel> menuItems)
        {
            var homeMenu = new MenuItemModel("Home", "Landing", "Index", "home");
            menuItems.Add(homeMenu);
        }

        private static void CommunityMenu(ICollection<MenuItemModel> menuItems)
        {
            var communityMenu = new MenuItemModel("Community", "Community", "Clubs", "home");
            communityMenu.SubMenuItems.Add(new SubMenuItemModel("Clubs", "Community", "Clubs"));
            communityMenu.SubMenuItems.Add(new SubMenuItemModel("Teamspeak", "Community", "Teamspeak"));
            communityMenu.SubMenuItems.Add(new SubMenuItemModel("Discord", "Community", "Discord"));
            communityMenu.SubMenuItems.Add(new SubMenuItemModel("Forums", "Redirect", "RedirectToUrl", new {url = "https://www.xtremeidiots.com"}));
            menuItems.Add(communityMenu);
        }

        private static void DemoManagerMenu(ICollection<MenuItemModel> menuItems)
        {
            var demoManagerMenu = new MenuItemModel("Demo Manager", "Demos", "Index", "camera");
            demoManagerMenu.SubMenuItems.Add(new SubMenuItemModel("Demos", "Demos", "Index"));
            demoManagerMenu.SubMenuItems.Add(new SubMenuItemModel("Upload Demo", "Demos", "UploadDemo"));
            menuItems.Add(demoManagerMenu);
        }

        private static void MapsMenu(ICollection<MenuItemModel> menuItems)
        {
            var mapsMenu = new MenuItemModel("Maps", "Maps", "Index", "download");
            mapsMenu.SubMenuItems.Add(new SubMenuItemModel("Map List", "Maps", "Index"));
            menuItems.Add(mapsMenu);
        }

        private void PlayersMenu(ICollection<MenuItemModel> menuItems)
        {
            var playersMenu = new MenuItemModel("Players", "Players", "Home", "users");
            playersMenu.SubMenuItems.Add(new SubMenuItemModel("Home", "Players", "Home"));
            playersMenu.SubMenuItems.Add(new SubMenuItemModel("My Actions", "Players", "MyActions"));
            playersMenu.SubMenuItems.Add(new SubMenuItemModel("Unclaimed Bans", "Players", "Unclaimed"));
            playersMenu.SubMenuItems.Add(new SubMenuItemModel("IP Search", "Players", "IPSearch"));
            using (var context = contextProvider.GetContext())
            {
                var games = context.Players.Select(p => p.GameType).Distinct();

                foreach (var gameType in games.OrderBy(gt => gt.ToString())) playersMenu.SubMenuItems.Add(new SubMenuItemModel(gameType.DisplayName(), "Players", "Index", new {id = gameType}));
            }

            menuItems.Add(playersMenu);
        }

        private static void MonitorMenu(ICollection<MenuItemModel> menuItems)
        {
            var mapsMenu = new MenuItemModel("Monitoring", "Monitor", "BanFileMonitor", "desktop");
            mapsMenu.SubMenuItems.Add(new SubMenuItemModel("Ban Files", "Monitor", "BanFileMonitor"));
            mapsMenu.SubMenuItems.Add(new SubMenuItemModel("Log Files", "Monitor", "LogFileMonitor"));
            mapsMenu.SubMenuItems.Add(new SubMenuItemModel("Rcon", "Monitor", "RconMonitor"));
            mapsMenu.SubMenuItems.Add(new SubMenuItemModel("Direct Query", "Monitor", "DirectQueryMonitor"));
            menuItems.Add(mapsMenu);
        }

        private static void AdmServersMenu(ICollection<MenuItemModel> menuItems)
        {
            var serversMenu = new MenuItemModel("AdmServers", "AdmServers", "Index", "cog");
            serversMenu.SubMenuItems.Add(new SubMenuItemModel("Servers", "AdmServers", "Index"));
            serversMenu.SubMenuItems.Add(new SubMenuItemModel("Create Server", "AdmServers", "Create"));
            menuItems.Add(serversMenu);
        }

        private static void AdmFileMonitorsMenu(ICollection<MenuItemModel> menuItems)
        {
            var fileMonitorsMenu = new MenuItemModel("AdmFileMonitors", "AdmFileMonitors", "Index", "cog");
            fileMonitorsMenu.SubMenuItems.Add(new SubMenuItemModel("File Monitors", "AdmFileMonitors", "Index"));
            fileMonitorsMenu.SubMenuItems.Add(new SubMenuItemModel("Create File Monitor", "AdmFileMonitors", "Create"));
            menuItems.Add(fileMonitorsMenu);
        }

        private static void AdmBanFileMonitorsMenu(ICollection<MenuItemModel> menuItems)
        {
            var fileMonitorsMenu = new MenuItemModel("AdmBanFileMonitors", "AdmBanFileMonitors", "Index", "cog");
            fileMonitorsMenu.SubMenuItems.Add(new SubMenuItemModel("Ban File Monitors", "AdmBanFileMonitors", "Index"));
            fileMonitorsMenu.SubMenuItems.Add(new SubMenuItemModel("Create Ban File Monitor", "AdmBanFileMonitors", "Create"));
            menuItems.Add(fileMonitorsMenu);
        }

        private static void AdmRconMonitorsMenu(ICollection<MenuItemModel> menuItems)
        {
            var rconMonitorsMenu = new MenuItemModel("AdmRconMonitors", "AdmRconMonitors", "Index", "cog");
            rconMonitorsMenu.SubMenuItems.Add(new SubMenuItemModel("Rcon Monitors", "AdmRconMonitors", "Index"));
            rconMonitorsMenu.SubMenuItems.Add(new SubMenuItemModel("Create Rcon Monitor", "AdmRconMonitors", "Create"));
            menuItems.Add(rconMonitorsMenu);
        }

        private static void AdmMapsMenu(ICollection<MenuItemModel> menuItems)
        {
            var mapsMenu = new MenuItemModel("AdmMaps", "AdmMaps", "Index", "cog");
            mapsMenu.SubMenuItems.Add(new SubMenuItemModel("Maps", "AdmMaps", "Index"));
            menuItems.Add(mapsMenu);
        }

        private static void AdmUsersMenu(ICollection<MenuItemModel> menuItems)
        {
            var userMenu = new MenuItemModel("AdmUsers", "AdmUsers", "Index", "cog");
            userMenu.SubMenuItems.Add(new SubMenuItemModel("Users", "AdmUsers", "Index"));
            menuItems.Add(userMenu);
        }

        private static void AdmLogsMenu(ICollection<MenuItemModel> menuItems)
        {
            var logsMenu = new MenuItemModel("AdmLogs", "AdmLogs", "UserLogs", "cog");
            logsMenu.SubMenuItems.Add(new SubMenuItemModel("User Logs", "AdmLogs", "UserLogs"));
            logsMenu.SubMenuItems.Add(new SubMenuItemModel("System Logs", "AdmLogs", "SystemLogs"));
            menuItems.Add(logsMenu);
        }

        private static void AdmMaintenanceMenu(List<MenuItemModel> menuItems)
        {
            var maintenanceMenu = new MenuItemModel("AdmMaintenance", "AdmMaintenance", "Index", "cog");
            maintenanceMenu.SubMenuItems.Add(new SubMenuItemModel("Tasks", "AdmMaintenance", "Index"));
            maintenanceMenu.SubMenuItems.Add(new SubMenuItemModel("FTP Credential Check", "AdmMaintenance", "FtpCredentialCheck"));
            menuItems.Add(maintenanceMenu);
        }

        private static void AnalyticsMenu(ICollection<MenuItemModel> menuItems)
        {
            var logsMenu = new MenuItemModel("Analytics", "Analytics", "AdminActions", "line-chart");
            logsMenu.SubMenuItems.Add(new SubMenuItemModel("Admin Actions", "Analytics", "AdminActions"));
            logsMenu.SubMenuItems.Add(new SubMenuItemModel("Players", "Analytics", "Players"));
            menuItems.Add(logsMenu);
        }
    }
}