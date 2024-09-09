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
        private const int TY_POS_BASE_ADDRESS = 0x4ED32C;
        private const int TY_ROT_BASE_ADDRESS = 0x4EB524;
        public byte[] Coordinates;
        public float Yaw;

        public HeroHandler()
        {
            Coordinates = new byte[0xC];
        }

        public void GetTyPosRot()
        {
            ProcessHandler.TryRead(TY_POS_BASE_ADDRESS, 0xC, out Coordinates, true);
            ProcessHandler.TryRead(TY_ROT_BASE_ADDRESS, out Yaw, true);
        }

        public void SendCoordinates()
        {
            //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
            Message message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);
            message.AddBool(!Client.HLevel.InMainWorld);
            message.AddBytes(Coordinates);
            message.AddFloat(Yaw);
            Client._client.Send(message);
        }
    }
}
