using CubivoxClient.Worlds;
using System;
using System.Net.Sockets;

using UnityEngine;

namespace CubivoxClient.Protocol.ClientBound
{
    public class CBBreakVoxelPacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] locBuffer = new byte[4];
            if (stream.Read(locBuffer, 0, 4) != 4)
                return false;
            int x = BitConverter.ToInt32(locBuffer);
            if (stream.Read(locBuffer, 0, 4) != 4)
                return false;
            int y = BitConverter.ToInt32(locBuffer);
            if (stream.Read(locBuffer, 0, 4) != 4)
                return false;
            int z = BitConverter.ToInt32(locBuffer);

            CubivoxController.RunOnMainThread(() => {
                ClientWorld world = WorldManager.GetInstance().GetCurrentWorld();
                world.SetVoxel(x, y, z, clientCubivox.GetClientItemRegistry().GetVoxelDef(0));
            });

            return true;
        }

        byte Packet.GetType()
        {
            return 0x06;
        }
    }
}
