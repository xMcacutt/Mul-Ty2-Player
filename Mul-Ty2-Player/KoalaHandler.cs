using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MT2PClient
{
    internal class KoalaHandler
    {
        public static int[] KoalaPathOffsets = { 0x19c, 0x158, 0x3c, 0x3c, 0xc, 0x0  };
        public static int KoalaBase;
        public static string[] KoalaNames = new string[] { "Dubbo", "_", "Elizabeth", "Boonie", "Katie", "Gummy", "Kiki", "Snugs", "Mim" };
        public Dictionary<string, int> KoalaBaseAddrs;

        public KoalaHandler()
        {
            KoalaBaseAddrs = new Dictionary<string, int>();
            foreach(string koalaName in KoalaNames)
            {
                KoalaBaseAddrs.Add(koalaName, 0);
            }
        }

        public static void SetCoordAddresses()
        {
            int koalaPath = 0x4E4ABC;
            string indicatorString = "notset";
            while(indicatorString != "A085_KoalaBoy")
            {
                KoalaBase = PointerCalculations.GetPointerAddress(koalaPath, KoalaPathOffsets);
                ProcessHandler.TryRead(KoalaBase, out int result, false);
                ProcessHandler.TryRead(result, 0xD, out var indicator, false);
                if (indicator != null) 
                    indicatorString = Encoding.ASCII.GetString(indicator);
                Console.WriteLine(indicatorString);
            }
            koalaPath = KoalaBase;
            foreach(var koalaName in KoalaNames)
            {
                Client.HKoala.KoalaBaseAddrs[koalaName] = koalaPath;
                ProcessHandler.TryRead(koalaPath + 0x18, out koalaPath, false);
            }
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void HandleKoalaSelected(Message message)
        {
            var clientId = message.GetUShort();
            var koalaName = message.GetString();
            var playerName = message.GetString();
            var isHost = message.GetBool();
            PlayerHandler.AddPlayer(koalaName, playerName, clientId, isHost);
        }

        [MessageHandler((ushort)MessageID.KoalaAvail)]
        private static void ShowKoalaAvailability(Message message)
        {
            Thread koalaSelectThread = new(ShowKoalaAvailability);
            koalaSelectThread.Start();
        }

        private static void ShowKoalaAvailability()
        {
            Console.WriteLine("Please select your koala by typing its number:");
            Dictionary<int, string> availableKoalas = new();
            var index = 1;
            var unassignedKoalaNames = KoalaNames.Where(koalaName => koalaName != "_" && PlayerHandler.Players.Values.All(player => player.Koala.KoalaName != koalaName));
            foreach (var unassignedKoalaName in unassignedKoalaNames)
            {
                var listing = index + ". " + unassignedKoalaName;
                Console.WriteLine(listing);
                availableKoalas.Add(index, unassignedKoalaName);
                index++;
            }
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || !availableKoalas.TryGetValue(int.Parse(input), out string? koala))
                {
                    Console.WriteLine("Please enter a valid koala ID.");
                    continue;
                }
                if (PlayerHandler.Players.Values.Any(player => player.Koala.KoalaName == koala))
                {
                    Console.WriteLine("That koala has been taken since being listed as available. Please select another.");
                    continue;
                }
                PlayerHandler.AddPlayer(koala, Client.Name, Client._client.Id, false);
                PlayerHandler.AnnounceSelection(koala, Client.Name, false);
                break;
            }
            Client.IsRunning = true;
        }

        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            if (!Client.KoalaSelected || Client.Relaunching) 
                return;
            var clientId = message.GetUShort();
            if (!PlayerHandler.Players.TryGetValue(clientId, out var player))
                return; 
            player.OnMenu = message.GetBool();
            if (player.OnMenu) 
                return;
            player.PositionData.SetPos(message.GetFloats());
            player.PositionData.Yaw = message.GetFloat();
            if (!PlayerHandler.Players.TryGetValue(Client._client.Id, out var self) ||
                !Client.HLevel.InMainWorld 
                || self.Koala.KoalaName == player.Koala.KoalaName)
                return;
            
            
            ProcessHandler.WriteData(Client.HKoala.KoalaBaseAddrs[player.Koala.KoalaName] + 0x40, new RotationMatrix(player.PositionData.Yaw).GetBytes(), $"{player.Koala.KoalaName} rotation");
            ProcessHandler.WriteData(Client.HKoala.KoalaBaseAddrs[player.Koala.KoalaName] + 0x70, player.PositionData.GetPosBytes(), $"Writing coordinates for koala {player.Koala.KoalaName}");
        }
    }
}
