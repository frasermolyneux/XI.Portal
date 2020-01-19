namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public static class XtremeIdiotsRoles
    {
        public const string LoggedInUser = "RegisteredUser," +
                                           "InactiveRegisteredUser," +
                                           "ClanMember," +
                                           "InactiveClanMember," +
                                           "ArmaModerator," +
                                           "Cod2Moderator," +
                                           "Cod4Moderator," +
                                           "Cod5Moderator," +
                                           "InsurgencyModerator," +
                                           "MinecraftModerator," +
                                           "RustModerator," +
                                           "ArmaAdmin," +
                                           "Cod2Admin," +
                                           "Cod4Admin," +
                                           "Cod5Admin," +
                                           "InsurgencyAdmin," +
                                           "MinecraftAdmin," +
                                           "RustAdmin," +
                                           "ArmaHeadAdmin," +
                                           "Cod2HeadAdmin," +
                                           "Cod4HeadAdmin," +
                                           "Cod5HeadAdmin," +
                                           "InsurgencyHeadAdmin," +
                                           "MinecraftHeadAdmin," +
                                           "RustHeadAdmin," +
                                           "SeniorAdmin";

        public const string SeniorAdmins = "SeniorAdmin";

        public const string Admins = "ArmaAdmin," +
                                     "Cod2Admin," +
                                     "Cod4Admin," +
                                     "Cod5Admin," +
                                     "InsurgencyAdmin," +
                                     "MinecraftAdmin," +
                                     "RustAdmin," +
                                     "ArmaHeadAdmin," +
                                     "Cod2HeadAdmin," +
                                     "Cod4HeadAdmin," +
                                     "Cod5HeadAdmin," +
                                     "InsurgencyHeadAdmin," +
                                     "MinecraftHeadAdmin," +
                                     "RustHeadAdmin," +
                                     "SeniorAdmin";

        public const string AdminAndModerators = "ArmaModerator," +
                                                 "Cod2Moderator," +
                                                 "Cod4Moderator," +
                                                 "Cod5Moderator," +
                                                 "InsurgencyModerator," +
                                                 "MinecraftModerator," +
                                                 "RustModerator," +
                                                 "ArmaAdmin," +
                                                 "Cod2Admin," +
                                                 "Cod4Admin," +
                                                 "Cod5Admin," +
                                                 "InsurgencyAdmin," +
                                                 "MinecraftAdmin," +
                                                 "RustAdmin," +
                                                 "ArmaHeadAdmin," +
                                                 "Cod2HeadAdmin," +
                                                 "Cod4HeadAdmin," +
                                                 "Cod5HeadAdmin," +
                                                 "InsurgencyHeadAdmin," +
                                                 "MinecraftHeadAdmin," +
                                                 "RustHeadAdmin," +
                                                 "SeniorAdmin";
    }

    public enum XtremeIdiotsGroups
    {
        RegisteredUser = 82,
        InactiveRegisteredUser = 83,

        ClanMember = 80,
        InactiveClanMember = 81,

        ArmaModerator = 90,
        Cod2Moderator = 84,
        Cod4Moderator = 85,
        Cod5Moderator = 86,
        InsurgencyModerator = 87,
        MinecraftModerator = 88,
        RustModerator = 89,

        ArmaAdmin = 92,
        Cod2Admin = 91,
        Cod4Admin = 93,
        Cod5Admin = 94,
        InsurgencyAdmin = 95,
        MinecraftAdmin = 96,
        RustAdmin = 97,

        ArmaHeadAdmin = 98,
        Cod2HeadAdmin = 99,
        Cod4HeadAdmin = 100,
        Cod5HeadAdmin = 101,
        InsurgencyHeadAdmin = 102,
        MinecraftHeadAdmin = 103,
        RustHeadAdmin = 104,

        SeniorAdmin = 61
    }
}