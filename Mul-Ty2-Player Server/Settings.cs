using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT2PServer
{
    internal class Settings
    {
        public string Password { get; set; }
        public ushort Port { get; set; }
        public bool DoSyncMissions { get; set; }
        public bool DoSyncCogs { get; set; }
        public bool DoSyncBilbies { get; set; }
        public bool DoSyncPurchases { get; set; }
        public bool DoSyncOpals { get; set; }
        public bool DoSyncZones { get; set; }
    }
}
