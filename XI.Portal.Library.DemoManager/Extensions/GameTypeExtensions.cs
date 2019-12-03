using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Library.DemoManager.Extensions
{
    public static class GameTypeExtensions
    {
        public static string DemoExtension(this GameType gameType)
        {
            switch (gameType)
            {
                case GameType.CallOfDuty2:
                    return "dm_1";
                case GameType.CallOfDuty4:
                    return "dm_1";
                case GameType.CallOfDuty5:
                    return "dm_6";
                default:
                    throw new Exception("Game Type not supported for demos");
            }
        }
    }
}