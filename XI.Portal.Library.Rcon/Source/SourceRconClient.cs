using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace XI.Portal.Library.Rcon.Source
{
    public class SourceRconClient
    {
        public static string ConnectionClosed = "Connection closed by remote host";
        public static string ConnectionFailedString = "Connection Failed!";
        public static string UnknownResponseType = "Unknown response";

        private readonly Socket socket;

        private bool hadjunkpacket;

        private int packetCount;

        public SourceRconClient(string hostname, int queryPort, string rconPassword)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            packetCount = 0;

            Connected = Connect(new IPEndPoint(IPAddress.Parse(hostname), queryPort), rconPassword);
        }

        public bool Connected { get; private set; }

        public bool Connect(IPEndPoint server, string password)
        {
            try
            {
                socket.Connect(server);
            }
            catch (SocketException)
            {
                OnError(ConnectionFailedString);
                OnConnectionSuccess(false);
                return false;
            }

            var serverAuth = new RconPacket
            {
                RequestId = 1,
                String1 = password,
                ServerDataSent = RconPacket.SERVERDATA_sent.SERVERDATA_AUTH
            };

            SendRconPacket(serverAuth);
            StartGetNewPacket();

            return true;
        }

        public void ExecuteCommand(string command)
        {
            if (!Connected) return;

            var packetToSend = new RconPacket
            {
                RequestId = 2,
                ServerDataSent = RconPacket.SERVERDATA_sent.SERVERDATA_EXECCOMMAND,
                String1 = command
            };

            SendRconPacket(packetToSend);
        }

        private void SendRconPacket(RconPacket p)
        {
            var packet = p.OutputAsBytes();
            socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, SendCallback, this);
        }

        private void SendCallback(IAsyncResult ar)
        {
            socket.EndSend(ar);
        }

        private void StartGetNewPacket()
        {
            var state = new RecState
            {
                IsPacketLength = true,
                Data = new byte[4],
                PacketCount = packetCount
            };

            packetCount++;

            socket.BeginReceive(state.Data, 0, 4, SocketFlags.None, ReceiveCallback, state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var recsuccess = false;
            RecState state = null;

            try
            {
                var bytesgotten = socket.EndReceive(ar);
                state = (RecState) ar.AsyncState;
                state.BytesSoFar += bytesgotten;
                recsuccess = true;
            }
            catch (SocketException)
            {
                OnError(ConnectionClosed);
            }

            if (recsuccess)
                ProcessIncomingData(state);
        }

        private void ProcessIncomingData(RecState state)
        {
            if (state.IsPacketLength)
            {
                // First 4 bytes of a new packet.
                state.PacketLength = BitConverter.ToInt32(state.Data, 0);

                state.IsPacketLength = false;
                state.BytesSoFar = 0;
                state.Data = new byte[state.PacketLength];
                socket.BeginReceive(state.Data, 0, state.PacketLength, SocketFlags.None, ReceiveCallback, state);
            }
            else
            {
                // Do something with data...

                if (state.BytesSoFar < state.PacketLength)
                {
                    // Missing data.
                    socket.BeginReceive(state.Data, state.BytesSoFar, state.PacketLength - state.BytesSoFar, SocketFlags.None, ReceiveCallback, state);
                }
                else
                {
                    // Process data.
                    var returnPacket = new RconPacket();
                    returnPacket.ParseFromBytes(state.Data, this);

                    ProcessResponse(returnPacket);

                    // Wait for new packet.
                    StartGetNewPacket();
                }
            }
        }

        private void ProcessResponse(RconPacket packet)
        {
            switch (packet.ServerDataReceived)
            {
                case RconPacket.SERVERDATA_rec.SERVERDATA_AUTH_RESPONSE:
                    if (packet.RequestId != -1)
                    {
                        // Connected.
                        Connected = true;
                        OnConnectionSuccess(true);
                    }
                    else
                    {
                        // Failed!
                        OnError(ConnectionFailedString);
                        OnConnectionSuccess(false);
                    }

                    break;
                case RconPacket.SERVERDATA_rec.SERVERDATA_RESPONSE_VALUE:
                    if (hadjunkpacket)
                        OnServerOutput(packet.String1);
                    else
                        hadjunkpacket = true;

                    break;
                default:
                    OnError(UnknownResponseType);
                    break;
            }
        }

        internal void OnServerOutput(string output)
        {
            ServerOutput?.Invoke(output);
        }

        internal void OnError(string error)
        {
            Errors?.Invoke(error);
        }

        internal void OnConnectionSuccess(bool info)
        {
            ConnectionSuccess?.Invoke(info);
        }

        public event StringOutput ServerOutput;
        public event StringOutput Errors;
        public event BoolInfo ConnectionSuccess;
    }

    public delegate void StringOutput(string output);

    public delegate void BoolInfo(bool info);

    internal class RecState
    {
        public int BytesSoFar;
        public byte[] Data;
        public bool IsPacketLength;

        public int PacketCount;
        public int PacketLength;

        internal RecState()
        {
            PacketLength = -1;
            BytesSoFar = 0;
            IsPacketLength = false;
        }
    }


    internal class RconPacket
    {
        public enum SERVERDATA_rec
        {
            SERVERDATA_RESPONSE_VALUE = 0,
            SERVERDATA_AUTH_RESPONSE = 2,
            None = 255
        }

        public enum SERVERDATA_sent
        {
            SERVERDATA_AUTH = 3,
            SERVERDATA_EXECCOMMAND = 2,
            None = 255
        }

        internal int RequestId;
        internal SERVERDATA_rec ServerDataReceived;
        internal SERVERDATA_sent ServerDataSent;
        internal string String1;
        internal string String2;

        internal RconPacket()
        {
            RequestId = 0;
            String1 = "blah";
            String2 = string.Empty;
            ServerDataSent = SERVERDATA_sent.None;
            ServerDataReceived = SERVERDATA_rec.None;
        }

        internal byte[] OutputAsBytes()
        {
            var utf = new UTF8Encoding();

            var bstring1 = utf.GetBytes(String1);
            var bstring2 = utf.GetBytes(String2);

            var serverdata = BitConverter.GetBytes((int) ServerDataSent);
            var reqid = BitConverter.GetBytes(RequestId);

            // Compose into one packet.
            var finalPacket = new byte[4 + 4 + 4 + bstring1.Length + 1 + bstring2.Length + 1];
            var packetsize = BitConverter.GetBytes(finalPacket.Length - 4);

            var bPtr = 0;
            packetsize.CopyTo(finalPacket, bPtr);
            bPtr += 4;

            reqid.CopyTo(finalPacket, bPtr);
            bPtr += 4;

            serverdata.CopyTo(finalPacket, bPtr);
            bPtr += 4;

            bstring1.CopyTo(finalPacket, bPtr);
            bPtr += bstring1.Length;

            finalPacket[bPtr] = 0;
            bPtr++;

            bstring2.CopyTo(finalPacket, bPtr);
            bPtr += bstring2.Length;

            finalPacket[bPtr] = 0;
            // ReSharper disable once RedundantAssignment
            bPtr++;

            return finalPacket;
        }

        internal void ParseFromBytes(byte[] bytes, SourceRconClient parent)
        {
            try
            {
                var bPtr = 0;
                var utf = new UTF8Encoding();

                // First 4 bytes are ReqId.
                RequestId = BitConverter.ToInt32(bytes, bPtr);
                bPtr += 4;
                // Next 4 are server data.
                ServerDataReceived = (SERVERDATA_rec) BitConverter.ToInt32(bytes, bPtr);
                bPtr += 4;
                // string1 till /0
                var stringcache = new ArrayList();
                while (bytes[bPtr] != 0)
                {
                    stringcache.Add(bytes[bPtr]);
                    bPtr++;
                }

                String1 = utf.GetString((byte[]) stringcache.ToArray(typeof(byte)));
                bPtr++;

                // string2 till /0

                stringcache = new ArrayList();
                while (bytes[bPtr] != 0)
                {
                    stringcache.Add(bytes[bPtr]);
                    bPtr++;
                }

                String2 = utf.GetString((byte[]) stringcache.ToArray(typeof(byte)));
                bPtr++;

                // Repeat if there's more data?

                if (bPtr != bytes.Length) parent.OnError("Urk, extra data!");
            }
            catch (Exception ex)
            {
                parent.OnError(ex.Message);
            }
        }
    }
}