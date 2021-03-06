﻿namespace XI.Portal.Data.CommonTypes.Extensions
{
    public static class GameTypeExtensions
    {
        public static string ToShortGameName(this GameType gameType)
        {
            switch (gameType)
            {
                case GameType.Unknown:
                    break;
                case GameType.CallOfDuty2:
                    return "cod2";
                case GameType.CallOfDuty4:
                    return "cod4";
                case GameType.CallOfDuty5:
                    return "cod5";
                case GameType.Insurgency:
                    break;
                case GameType.ArkSurvivalEvolved:
                    break;
                case GameType.Battlefield1:
                    break;
                case GameType.Battlefield3:
                    break;
                case GameType.Battlefield4:
                    break;
                case GameType.Battlefield5:
                    break;
                case GameType.BattlefieldBadCompany2:
                    break;
                case GameType.CrysisWars:
                    break;
                case GameType.Left4Dead2:
                    break;
                case GameType.Minecraft:
                    break;
                case GameType.PlayerUnknownsBattleground:
                    break;
                case GameType.RisingStormVietnam:
                    break;
                case GameType.Rust:
                    break;
                case GameType.WarThunder:
                    break;
                case GameType.WorldOfWarships:
                    break;
                case GameType.WorldWar3:
                    break;
                case GameType.UnrealTournament2004:
                    break;
                default:
                    break;
            }

            return gameType.ToString();
        }
    }
}
