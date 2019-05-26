using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.ServerInfo.Extensions;
using XI.Portal.Library.ServerInfo.Protocols;

namespace XI.Portal.Library.ServerInfo
{
    public class GameServerInfo
    {
        private bool debugMode;
        private Protocol serverInfo;

        public GameServerInfo(string host, int port, GameType type)
        {
            Host = host;
            Port = port;
            GameType = type;

            CheckServerType();
        }

        public GameServerInfo(string host, int port, GameType type, int timeout)
        {
            Host = host;
            Port = port;
            GameType = type;

            CheckServerType();
        }

        public int Timeout
        {
            get => serverInfo.Timeout;
            set => serverInfo.Timeout = value;
        }

        public StringDictionary Parameters => serverInfo.Parameters;

        public bool IsOnline => serverInfo.IsOnline;

        public DateTime ScanTime => serverInfo.ScanTime;

        public PlayerCollection Players => serverInfo.PlayerCollection;

        public StringCollection Teams => serverInfo.Teams;

        public int MaxPlayers => serverInfo.MaxPlayers;

        public int NumPlayers => serverInfo.NumPlayers;

        public string Name => serverInfo.Name;

        public string Mod => serverInfo.Mod;

        public string Map => serverInfo.Map;

        public bool Passworded => serverInfo.Passworded;

        public GameType GameType { get; }

        public string Host { get; }

        public int Port { get; }

        public bool DebugMode
        {
            get => debugMode;
            set
            {
                if (serverInfo != null) serverInfo.DebugMode = value;
                debugMode = value;
            }
        }

        private void CheckServerType()
        {
            var gameProtocol = GameType.Protocol();

            switch (gameProtocol)
            {
                case GameProtocol.Samp:
                    serverInfo = new Samp(Host, Port);
                    break;
                case GameProtocol.Ase:
                    serverInfo = new Ase(Host, Port);
                    break;
                case GameProtocol.Doom3:
                    serverInfo = new Doom3(Host, Port);
                    break;
                case GameProtocol.GameSpy:
                    serverInfo = new GameSpy(Host, Port);
                    break;
                case GameProtocol.GameSpy2:
                    serverInfo = new GameSpy2(Host, Port);
                    break;
                case GameProtocol.HalfLife:
                    serverInfo = new HalfLife(Host, Port);
                    break;
                case GameProtocol.Quake3:
                    serverInfo = new Quake3(Host, Port);
                    break;
                case GameProtocol.Source:
                    serverInfo = new Source(Host, Port);
                    break;
                default:
                    throw new NotImplementedException();
            }

            serverInfo.DebugMode = debugMode;
        }

        public void QueryServer()
        {
            serverInfo.GetServerInfo();
        }

        public static string CleanName(string name)
        {
            var regex = new Regex(@"(\^\d)|(\$\d)");
            return regex.Replace(name, "");
        }
    }
}