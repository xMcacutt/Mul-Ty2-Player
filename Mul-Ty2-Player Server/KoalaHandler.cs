using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MT2PClient;

namespace MT2PServer
{
    internal class KoalaHandler
    {
        public static readonly string[] KoalaNames = { "Dubbo", "Mim", "Snugs", "Gummy", "_", "Elizabeth", "Katie", "Kiki", "Bonnie" };

        public KoalaHandler()
        {
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void AssignKoala(ushort fromClientId, Message message)
        {
            var clientId = message.GetUShort();
            var koalaName = message.GetString();
            var playerName = message.GetString();
            var isHost = message.GetBool();
            PlayerHandler.AddPlayer(koalaName, playerName, clientId, isHost);
            AnnounceKoalaAssigned(koalaName, playerName, clientId, isHost, fromClientId, true);
        }

        private static void AnnounceKoalaAssigned(string koalaName, string playerName, ushort clientId, bool isHost, ushort fromToClientId, bool bSendToAll)
        {
            Message announcement = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            announcement.AddUShort(clientId);
            announcement.AddString(koalaName);
            announcement.AddString(playerName);
            announcement.AddBool(isHost);
            if (bSendToAll)
            {
                Server._Server.SendToAll(announcement, fromToClientId);
                Server.SendMessageToClients($"{playerName} (Client{fromToClientId}) selected {koalaName}", true);
            }
            else Server._Server.Send(announcement, fromToClientId);
        }

        public static void SendKoalaAvailability(ushort recipient)
        {
            foreach (Player player in PlayerHandler.Players.Values) 
                AnnounceKoalaAssigned(player.Koala.KoalaName, player.Name, player.ClientID, player.IsHost, recipient, false);
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaAvail);
            Server._Server.Send(message, recipient);
        }

        public void ReturnKoala(Player player)
        {
            foreach (var otherPlayer in PlayerHandler.Players.Values)
            {
                if (otherPlayer.CurrentLevel != player.PreviousLevel || otherPlayer.Name == player.Name) 
                    continue;
                var posData = new PlayerPositionData { X = 0.1f, Y = 8750f, Z = 0.2f };
                Program.HPlayer.SendCoordinates(player.ClientID, posData, player.OnMenu);
            }
        }

    }
}