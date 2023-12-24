using System;
using System.Net.Sockets;

using CubivoxCore;

namespace CubivoxClient.Protocol.ServerBound
{
    /// <summary>
    /// The packet responsible for sending the break voxel request to the server.
    /// </summary>
    public class BreakVoxelPacket : ServerBoundPacket
    {
        private Location location;
        public BreakVoxelPacket(Location location)
        {
            this.location = location;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes((int) location.x));
            stream.Write(BitConverter.GetBytes((int) location.y));
            stream.Write(BitConverter.GetBytes((int) location.z));
        }

        byte Packet.GetType()
        {
            return 0x03;
        }
    }
}
