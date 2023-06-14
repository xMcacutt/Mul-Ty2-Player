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
        public static int[] KoalaPathOffsets = { 0xB0, 0x34, 0x4C, 0x8, 0x8, 0x8, 0x38, 0x40, 0x18, 0x0 };
        public static int KoalaBase;
        public static string[] KoalaNames = new string[] { "Dubbo", "Mim", "Snugs", "Gummy", "_", "Elizabeth", "Katie", "Kiki", "Bonnie" };
        public Dictionary<string, int> KoalaBaseAddrs;

        public KoalaHandler()
        {
            KoalaBaseAddrs = new Dictionary<string, int>();
        }

        public static void SetCoordAddresses()
        {
            int koalaPath = 0x4EBFF4;
            KoalaBase = PointerCalculations.GetPointerAddress(koalaPath, KoalaPathOffsets);
            koalaPath = KoalaBase;
            foreach(string koalaName in KoalaNames)
            {
                ProcessHandler.TryRead(koalaPath + 0x18, out koalaPath, false);
                Client.HKoala.KoalaBaseAddrs.Add(koalaName, koalaPath);
            }
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void HandleKoalaSelected(Message message)
        {
            string koalaName = message.GetString();
            string playerName = message.GetString();
            ushort clientID = message.GetUShort();
            bool isHost = message.GetBool();
            PlayerHandler.AddPlayer(koalaName, playerName, clientID, isHost);
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
            int index = 1;
            var unassignedKoalaNames = KoalaNames.Where(koalaName => koalaName != "_" && !PlayerHandler.Players.Values.Any(player => player.Koala.KoalaName == koalaName));
            foreach (string unassignedKoalaName in unassignedKoalaNames)
            {
                string listing = index + ". " + unassignedKoalaName;
                Console.WriteLine(listing);
                availableKoalas.Add(index, unassignedKoalaName);
                index++;
            }
            while (true)
            {
                string input = Console.ReadLine();
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
            if (!Client.KoalaSelected || Client.Relaunching) return;
            bool onMenu = message.GetBool();
            ushort clientID = message.GetUShort();
            if (onMenu) return;
            string koalaName = message.GetString();
            string level = message.GetString();
            byte[] coordinates = message.GetBytes();
            float yaw = message.GetFloat();
            Console.WriteLine("Writing " + coordinates[0] + ", " + coordinates[1] + ", " + coordinates[2] + " for koala: " + koalaName + " menu status: " + onMenu);
            //SANITY CHECK THAT WE HAVEN'T BEEN SENT OUR OWN COORDINATES AND WE AREN'T LOADING, ON THE MENU, OR IN A DIFFERENT LEVEL 
            if (!PlayerHandler.Players.TryGetValue(Client._client.Id, out Player p) ||
                !Client.HLevel.InMainWorld ||
                level != Client.HLevel.CurrentLevel ||
                p.Koala.KoalaName == koalaName
            )
            {
                Console.WriteLine("Did not write");
                return;
            }
            //WRITE POSITION AND ROTATION
            ProcessHandler.WriteData(Client.HKoala.KoalaBaseAddrs[koalaName] + 0x40, new RotationMatrix(yaw).GetBytes(), $"{koalaName} rotation");
            ProcessHandler.WriteData(Client.HKoala.KoalaBaseAddrs[koalaName] + 0x70, coordinates, $"Writing coordinates for koala {koalaName} in level {level}");
            
        }
    }
}
