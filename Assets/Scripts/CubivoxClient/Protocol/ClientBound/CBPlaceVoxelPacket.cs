using CubivoxClient.Worlds;
using CubivoxCore.Scheduler;
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
            NetworkingUtils.FillBufferFromNetwork(typeBuffer, stream);
            short type = BitConverter.ToInt16(typeBuffer);

            byte[] locBuffer = new byte[4];
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int x = BitConverter.ToInt32(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int y = BitConverter.ToInt32(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int z = BitConverter.ToInt32(locBuffer);

            CubivoxScheduler.RunOnMainThread(() => {
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
