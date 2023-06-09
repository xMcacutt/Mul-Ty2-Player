using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT2PServer
{
    internal class Player
    {
        public Koala Koala { get; set; }
        public string Name;
        public ushort ClientID;
        public bool IsHost;
        public bool IsReady;
        public bool OnMenu;
        public byte[] Coordinates;
        public float Yaw;
        public string CurrentLevel;
        public string PreviousLevel = "mainmenu";

        public Player(Koala koala, string name, ushort id, bool isHost, bool isReady, bool onMenu)
        {
            Koala = koala;
            Name = name;
            ClientID = id;
            IsHost = isHost;
            IsReady = isReady;
            OnMenu = onMenu;
        }
    }
}
