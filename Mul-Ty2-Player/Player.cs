using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT2PClient
{
    internal class Player
    {
        public Koala Koala { get; set; }
        public string Name;
        public ushort Id;
        public bool IsHost;
        public bool IsReady;
        public bool OnMenu;
        public PlayerPositionData PositionData;

        public Player(Koala koala, string name, ushort id, bool isHost, bool isReady, bool onMenu)
        {
            Koala = koala;
            Name = name;
            Id = id;
            IsHost = isHost;
            IsReady = isReady;
            OnMenu = onMenu;
            PositionData = new PlayerPositionData();
        }
    }
}
