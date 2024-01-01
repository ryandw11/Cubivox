using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CubivoxClient.Protocol.ClientBound
{
    public class PlayerPositionUpdatePacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] rawUuid = new byte[16];
            NetworkingUtils.FillBufferFromNetwork(rawUuid, stream);
            Guid uuid = new Guid(rawUuid);

            byte[] locBuffer = new byte[8];
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            double x = BitConverter.ToDouble(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            double y = BitConverter.ToDouble(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            double z = BitConverter.ToDouble(locBuffer);

            CubivoxController.RunOnMainThread(() => {
                GameObject obj = GameObject.Find(uuid.ToString());
                if(obj == null)
                {
                    Debug.LogWarning($"Position update packet recievied, but can't find player with UUID {uuid}!");
                    return;
                }
                obj.transform.position = new Vector3((float)x, (float)y, (float)z);
            });

            return true;
        }

        byte Packet.GetType()
        {
            return 0x03;
        }
    }
}
