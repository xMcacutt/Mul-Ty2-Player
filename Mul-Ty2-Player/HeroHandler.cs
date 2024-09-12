using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Riptide;

namespace MT2PClient
{
    public class HeroHandler
    {
        private const int TY_POS_BASE_ADDRESS = 0x4ED328;
        private const int TY_ROT_BASE_ADDRESS = 0x4EB524;
        public PlayerPositionData PositionData;

        public HeroHandler()
        {
            PositionData = new PlayerPositionData();
        }

        public void GetTyPosRot()
        {
            ProcessHandler.TryRead(TY_POS_BASE_ADDRESS + 0, out PositionData.X, true);
            ProcessHandler.TryRead(TY_POS_BASE_ADDRESS + 4, out PositionData.Y, true);
            ProcessHandler.TryRead(TY_POS_BASE_ADDRESS + 8, out PositionData.Z, true);
            ProcessHandler.TryRead(TY_ROT_BASE_ADDRESS, out PositionData.Yaw, true);
        }

        public void SendCoordinates()
        {
            //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
            Message message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);
            message.AddBool(!Client.HLevel.InMainWorld);
            message.AddFloats(PositionData.GetFloats());
            message.AddFloat(PositionData.Yaw);
            Client._client.Send(message);
        }
    }
}
