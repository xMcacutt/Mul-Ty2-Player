using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Riptide;

namespace MT2PClient
{
    public class HeroHandler
    {
        private const int TY_POS_ROT_BASE_ADDRESS = 0x4E4ABC;
        private readonly int[] TY_POS_POINTER_OFFSETS = new[] { 0x124, 0x444 };
        private readonly int[] TY_ROT_POINTER_OFFSETS = new[] { 0x594 };
        private bool _pathsValid;
        private int _tyPosAddr;
        private int _tyRotAddr;
        public PlayerPositionData PositionData;

        public HeroHandler()
        {
            PositionData = new PlayerPositionData();
        }

        public void GetTyPosRot()
        {
            ProcessHandler.TryRead(TY_POS_ROT_BASE_ADDRESS, out int test, true);
            if (test == 0)
            {
                _pathsValid = false;
                return;
            }
            if (!_pathsValid)
            {
                _tyPosAddr = PointerCalculations.GetPointerAddress(TY_POS_ROT_BASE_ADDRESS, TY_POS_POINTER_OFFSETS);
                _tyRotAddr = PointerCalculations.GetPointerAddress(TY_POS_ROT_BASE_ADDRESS, TY_ROT_POINTER_OFFSETS);
                _pathsValid = true;
            }
            ProcessHandler.TryRead(_tyPosAddr + 0, out PositionData.X, true);
            ProcessHandler.TryRead(_tyPosAddr + 4, out PositionData.Y, true);
            ProcessHandler.TryRead(_tyPosAddr + 8, out PositionData.Z, true);
            ProcessHandler.TryRead(_tyRotAddr + 0, out PositionData.Pitch, true);
            ProcessHandler.TryRead(_tyRotAddr + 4, out PositionData.Yaw, true);
            ProcessHandler.TryRead(_tyRotAddr + 8, out PositionData.Roll, true);
        }

        public void SendCoordinates()
        {
            //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
            Message message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);
            message.AddBool(!Client.HLevel.InMainWorld);
            message.AddFloats(PositionData.GetPosFloats());
            message.AddFloat(PositionData.Yaw);
            Client._client.Send(message);
        }
    }
}
