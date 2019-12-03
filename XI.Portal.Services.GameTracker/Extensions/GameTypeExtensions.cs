using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Library.GameTracker.Extensions
{
    public static class GameTypeExtensions
    {
        public static string ToGameTrackerGameType(this GameType gameType)
        {
            switch (gameType)
            {
                case GameType.CallOfDuty2:
                    return "cod2";
                case GameType.CallOfDuty4:
                    return "cod4";
                case GameType.CallOfDuty5:
                    return "codww";
                case GameType.Insurgency:
                    return "ins";
                case GameType.Rust:
                    return "rust";
                case GameType.Left4Dead2:
                    return "left4dead2";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }
    }
}