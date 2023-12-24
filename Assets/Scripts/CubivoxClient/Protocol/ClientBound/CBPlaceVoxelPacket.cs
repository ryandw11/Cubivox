using CubivoxClient.Worlds;
using System;
using System.Net.Sockets;

using UnityEngine;

namespace CubivoxClient.Protocol.ClientBound
{
    public class CBPlaceVoxelPacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] typeBuffer = new byte[2];
            if (stream.Read(typeBuffer, 0, 2) != 2)
                return false;
            short type = BitConverter.ToInt16(typeBuffer);

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
                world.SetVoxel(x, y, z, clientCubivox.GetClientItemRegistry().GetVoxelDef(type));
            });

            return true;
        }

        byte Packet.GetType()
        {
            return 0x07;
        }
    }
}
