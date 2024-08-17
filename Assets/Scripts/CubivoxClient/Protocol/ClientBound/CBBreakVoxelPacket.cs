using CubivoxClient.Worlds;
using CubivoxCore.Scheduler;
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
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int x = BitConverter.ToInt32(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int y = BitConverter.ToInt32(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int z = BitConverter.ToInt32(locBuffer);

            CubivoxScheduler.RunOnMainThread(() => {
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
