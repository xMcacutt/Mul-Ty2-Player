using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT2PClient;

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
        public PlayerPositionData PositionData;
        public string CurrentLevel;
        public string PreviousLevel = "mainmenu";

        public Player(Koala koala, string name, ushort id, bool isHost, bool isReady, bool onMenu)
        {
            Koala = koala;
            Name = name;
            ClientID = id;
            IsHost = isHost;
            PositionData = new PlayerPositionData();
            CurrentLevel = "mainmenu";
            IsReady = isReady;
            OnMenu = onMenu;
        }
    }
}
