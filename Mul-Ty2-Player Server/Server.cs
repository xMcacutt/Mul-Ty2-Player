﻿using Riptide;
using Riptide.Utils;

namespace MT2PServer
{
    internal class Server
    {
        public static Riptide.Server _Server;
        public static bool _isRunning;

        static KoalaHandler HKoala => Program.HKoala;

        public static void StartServer()
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
            _isRunning = true;

            new Thread(new ThreadStart(Loop)).Start();
        }

        private static void Loop()
        {
            _Server = new Riptide.Server();
            _Server.Start(SettingsHandler.Settings.Port, 8);

            _Server.HandleConnection += (s, e) => HandleConnection(s, e);
            _Server.ClientConnected += (s, e) => ClientConnected(s, e);
            _Server.ClientDisconnected += (s, e) => ClientDisconnected(s, e);

            while (_isRunning)
            {
                _Server.Update();
                if (PlayerHandler.Players.Count != 0)
                {
                    foreach (Player player in PlayerHandler.Players.Values)
                    {
                        Program.HPlayer.SendCoordinates(player.ClientID, player.PositionData, player.OnMenu);
                    }
                }
                Thread.Sleep(10);
            }

            if (Program._inputStr == "y") { return; }
            _Server.Stop();
            Console.WriteLine("Would you like to restart this Mul-Ty2-Player server? [y/n]");
        }

        public static void RestartServer()
        {
            Program._inputStr = "y";
            _Server.Stop();
            _isRunning = false;
        }

        private static void HandleConnection(Connection pendingConnection, Message authenticationMessage)
        {
            string pass = authenticationMessage.GetString();
            if (!string.Equals(pass, SettingsHandler.Settings.Password, StringComparison.CurrentCultureIgnoreCase)
                && !string.Equals(SettingsHandler.Settings.Password, "XXXXX", StringComparison.CurrentCultureIgnoreCase)
                && !string.IsNullOrWhiteSpace(SettingsHandler.Settings.Password)
                && _Server.ClientCount > 0)
            {
                _Server.Reject(pendingConnection);
            }
            else _Server.Accept(pendingConnection);
        }

        private static void ClientConnected(object sender, ServerConnectedEventArgs e)
        {
            KoalaHandler.SendKoalaAvailability(e.Client.Id);
            //SettingsHandler.SendSettings(e.Client.Id);
        }

        private static void ClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            if (PlayerHandler.Players.TryGetValue(e.Client.Id, out _))
            {
                SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Name} has disconnected from the server.", true);
                SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Koala.KoalaName} was returned to the koala pool", true);
                if (PlayerHandler.Players[e.Client.Id].Koala != null) HKoala.ReturnKoala(PlayerHandler.Players[e.Client.Id]);
                PlayerHandler.RemovePlayer(e.Client.Id);
                PlayerHandler.AnnounceDisconnect(e.Client.Id);
            }
        }

        public static void SendMessageToClient(string str, bool printToServer, ushort to)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
            _Server.Send(message, to);
            if (printToServer) { Console.WriteLine(str); }
        }

        public static void SendMessageToClients(string str, bool printToServer, ushort except)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
            _Server.SendToAll(message, except);
            if (printToServer) { Console.WriteLine(str); }
        }

        public static void SendMessageToClients(string str, bool printToServer)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
            message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
            _Server.SendToAll(message);
            if (printToServer) { Console.WriteLine(str); }
        }
    }
}