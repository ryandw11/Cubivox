using System;
using System.Net.Sockets;

using CubivoxCore;
using CubivoxCore.BaseGame;

namespace CubivoxClient.Protocol.ServerBound
{
    /// <summary>
    /// The packet responsible for sending the place voxel request to the server.
    /// </summary>
    public class PlaceVoxelPacket : ServerBoundPacket
    {
        private short voxelType;
        private Location location;

        public PlaceVoxelPacket(VoxelDef voxelDef, Location location)
        {
            this.location = location;
            this.voxelType = ClientCubivox.GetClientInstance().GetClientItemRegistry().GetVoxelDefId(voxelDef);
        }

        public PlaceVoxelPacket(short voxelType, Location location)
        {
            this.location = location;
            this.voxelType = voxelType;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes(voxelType));
            stream.Write(BitConverter.GetBytes((int)location.x));
            stream.Write(BitConverter.GetBytes((int)location.y));
            stream.Write(BitConverter.GetBytes((int)location.z));
        }

        byte Packet.GetType()
        {
            return 0x04;
        }
    }
}
