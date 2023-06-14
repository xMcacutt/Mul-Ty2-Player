using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MT2PServer
{
    internal class KoalaHandler
    {
        public static string[] KoalaNames = { "Dubbo", "Mim", "Snugs", "Gummy", "_", "Elizabeth", "Katie", "Kiki", "Bonnie" };

        public KoalaHandler()
        {
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void AssignKoala(ushort fromClientId, Message message)
        {
            string koalaName = message.GetString();
            string playerName = message.GetString();
            ushort clientID = message.GetUShort();
            bool isHost = message.GetBool();
            PlayerHandler.AddPlayer(koalaName, playerName, clientID, isHost);
            AnnounceKoalaAssigned(koalaName, playerName, clientID, isHost, fromClientId, true);
        }

        private static void AnnounceKoalaAssigned(string koalaName, string playerName, ushort clientID, bool isHost, ushort fromToClientId, bool bSendToAll)
        {
            Message announcement = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            announcement.AddString(koalaName);
            announcement.AddString(playerName);
            announcement.AddUShort(clientID);
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
            foreach (Player player in PlayerHandler.Players.Values) AnnounceKoalaAssigned(player.Koala.KoalaName, player.Name, player.ClientID, player.IsHost, recipient, false);
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaAvail);
            Server._Server.Send(message, recipient);
        }

        public void ReturnKoala(Player player)
        {
            foreach (Player otherPlayer in PlayerHandler.Players.Values)
            {
                if (otherPlayer.CurrentLevel == player.PreviousLevel && otherPlayer.Name != player.Name)
                {
                    float[] defaultCoords =
                    {
                        0.1f,
                        8750f,
                        0.2f,
                    };
                    byte[] byteArray = defaultCoords.SelectMany(BitConverter.GetBytes).ToArray();
                    Server.SendCoordinates(player.ClientID, player.Koala.KoalaName, player.PreviousLevel, byteArray, 0f, player.OnMenu);
                }

            }
        }

    }
}