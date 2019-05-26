namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public static class XtremeIdiotsRolesHelper
    {
        public static bool IsGroupIdAdminOrModerator(int groupId)
        {
            var primaryGroup = (XtremeIdiotsGroups) groupId;

            switch (primaryGroup)
            {
                case XtremeIdiotsGroups.RegisteredUser:
                    return false;
                case XtremeIdiotsGroups.Webmaster:
                    return true;
                case XtremeIdiotsGroups.ClanMember:
                    return false;
                case XtremeIdiotsGroups.Cod2Admin:
                    return true;
                case XtremeIdiotsGroups.Cod4Admin:
                    return true;
                case XtremeIdiotsGroups.Cod5Admin:
                    return true;
                case XtremeIdiotsGroups.CrysisAdmin:
                    return true;
                case XtremeIdiotsGroups.Cod2HeadAdmin:
                    return true;
                case XtremeIdiotsGroups.Cod4HeadAdmin:
                    return true;
                case XtremeIdiotsGroups.Cod5HeadAdmin:
                    return true;
                case XtremeIdiotsGroups.CrysisHeadAdmin:
                    return true;
                case XtremeIdiotsGroups.Politics:
                    return false;
                case XtremeIdiotsGroups.UsFest:
                    return false;
                case XtremeIdiotsGroups.DownloaderAdmin:
                    return false;
                case XtremeIdiotsGroups.BattlefieldHeadAdmin:
                    return true;
                case XtremeIdiotsGroups.BattlefieldAdmin:
                    return true;
                case XtremeIdiotsGroups.MinecraftAdmin:
                    return true;
                case XtremeIdiotsGroups.MinecraftHeadAdmin:
                    return true;
                case XtremeIdiotsGroups.EuroFest:
                    return false;
                case XtremeIdiotsGroups.InsurgencyAdmin:
                    return true;
                case XtremeIdiotsGroups.InsurgencyHeadAdmin:
                    return true;
                case XtremeIdiotsGroups.WebEdit:
                    return false;
                case XtremeIdiotsGroups.SeniorAdmin:
                    return true;
                case XtremeIdiotsGroups.StormCrow:
                    return false;
                case XtremeIdiotsGroups.GameNews:
                    return false;
                case XtremeIdiotsGroups.BattlefieldModerator:
                    return true;
                case XtremeIdiotsGroups.Cod2Moderator:
                    return true;
                case XtremeIdiotsGroups.Cod4Moderator:
                    return true;
                case XtremeIdiotsGroups.Cod5Moderator:
                    return true;
                case XtremeIdiotsGroups.InsurgencyModerator:
                    return true;
                case XtremeIdiotsGroups.MinecraftModerator:
                    return true;
                case XtremeIdiotsGroups.Rs2Moderator:
                    return true;
                case XtremeIdiotsGroups.Rs2Admin:
                    return true;
                case XtremeIdiotsGroups.Rs2HeadAdmin:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsGroupIdSeniorAdmin(int groupId)
        {
            var primaryGroup = (XtremeIdiotsGroups) groupId;

            switch (primaryGroup)
            {
                case XtremeIdiotsGroups.SeniorAdmin:
                case XtremeIdiotsGroups.Webmaster:
                    return true;
                default:
                    return false;
            }
        }
    }
}