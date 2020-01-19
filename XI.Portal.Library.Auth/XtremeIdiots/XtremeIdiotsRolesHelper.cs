using System;

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
                case XtremeIdiotsGroups.InactiveRegisteredUser:
                case XtremeIdiotsGroups.ClanMember:
                case XtremeIdiotsGroups.InactiveClanMember:
                    return false;
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
                case XtremeIdiotsGroups.SeniorAdmin:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool IsGroupIdSeniorAdmin(int groupId)
        {
            var primaryGroup = (XtremeIdiotsGroups) groupId;

            switch (primaryGroup)
            {
                case XtremeIdiotsGroups.SeniorAdmin:
                    return true;
                default:
                    return false;
            }
        }
    }
}