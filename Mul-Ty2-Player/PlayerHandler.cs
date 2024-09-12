using System;
using System.Collections.Generic;
using Riptide;
using System.Linq;
using System.Media;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows;
using System.Numerics;
using MT2PClient;
using static System.Net.Mime.MediaTypeNames;

namespace MT2PClient
{
    internal class PlayerHandler
    {
        public static Dictionary<ushort, Player> Players = new();

        public PlayerHandler()
        {
            Players = new();
        }

        public static void AddPlayer(string koalaName, string name, ushort clientId, bool isHost)
        {
            Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
            Players.Add(clientId, new Player(koala, name, clientId, isHost, false, true));
        }

        public static void AnnounceSelection(string koalaName, string name, bool isHost)
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            message.AddUShort(Client._client.Id);
            message.AddString(koalaName);
            message.AddString(name);
            message.AddBool(isHost);
            Client._client.Send(message);
            Client.KoalaSelected = true;
        }

        public static void RemovePlayer(ushort id)
        {
            Players.Remove(id);
        }

        [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
        public static void PeerDisconnected(Message message)
        {
            RemovePlayer(message.GetUShort());
        }
    }
}