﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT2PServer
{
    public enum MessageID : ushort
    {
        Connected,
        PlayerInfo,
        KoalaCoordinates,
        ConsoleSend,
        ServerDataUpdate,
        ClientDataUpdate,
        Disconnect,
        ResetSync,
        ReqSync,
        SyncSettings,
        ReqHost,
        HostChange,
        HostCommand,
        KoalaSelected,
        AnnounceDisconnect,
        P2PMessage,
        Ready,
        Countdown,
        KoalaAvail
    }
}
