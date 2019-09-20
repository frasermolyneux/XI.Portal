using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace XI.Portal.Library.Rcon.Client
{
    public class RconClient : IRconClient, IDisposable
    {
        private readonly string hostname;
        private readonly int queryPort;
        private readonly string rconPassword;

        public RconClient(string hostname, int queryPort, string rconPassword)
        {
            this.hostname = hostname;
            this.queryPort = queryPort;
            this.rconPassword = rconPassword;
        }

        public void Dispose()
        {
        }

        public string StatusCommand()
        {
            return ExecuteCommand("status");
        }

        public string KickCommand(string targetPlayerNum)
        {
            return ExecuteCommand($"clientkick {targetPlayerNum}");
        }

        public string BanCommand(string targetPlayerNum)
        {
            return ExecuteCommand($"banclient {targetPlayerNum}");
        }

        public string RestartServer()
        {
            return ExecuteCommand("quit");
        }

        public string RestartMap()
        {
            return ExecuteCommand("map_restart");
        }

        public string NextMap()
        {
            return ExecuteCommand("map_rotate");
        }

        public string MapRotation()
        {
            return ExecuteCommand("sv_maprotation");
        }

        public string SayCommand(string message)
        {
            return ExecuteCommand($"say \"{message}\"");
        }

        private string ExecuteCommand(string rconCommand)
        {
            var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                SendTimeout = 30000,
                ReceiveTimeout = 30000
            };
            client.Connect(IPAddress.Parse(hostname), queryPort);

            var command = $"rcon {rconPassword} {rconCommand}";
            var bufferTemp = Encoding.ASCII.GetBytes(command);
            var bufferSend = new byte[bufferTemp.Length + 4];

            bufferSend[0] = 0xFF;
            bufferSend[1] = 0xFF;
            bufferSend[2] = 0xFF;
            bufferSend[3] = 0xFF;

            var j = 4;
            foreach (var commandBytes in bufferTemp)
            {
                bufferSend[j] = commandBytes;
                j++;
            }

            client.Send(bufferSend, SocketFlags.None);

            var response = new StringBuilder();

            do
            {
                var recieveBuffer = new byte[65536];
                var bytesReceived = client.Receive(recieveBuffer);
                var data = Encoding.ASCII.GetString(recieveBuffer, 0, bytesReceived);

                if (data.IndexOf("print", StringComparison.Ordinal) == 4) data = data.Substring(10);

                response.Append(data);
            } while (client.Available > 0);

            return response.ToString().Replace("\0", "");
        }
    }
}