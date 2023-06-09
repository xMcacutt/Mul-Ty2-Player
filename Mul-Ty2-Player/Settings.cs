using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT2PClient
{
    internal class Settings
    {
        public bool DoGetSteamName { get; set; }
        public string DefaultName { get; set; }
        public ushort Port { get; set; }
        public bool DoKoalaCollision { get; set; }
        public bool CreateLogFile { get; set; }
        public bool AttemptReconnect { get; set; }
    }
}
