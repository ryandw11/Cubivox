using System;
using System.Net.Sockets;

using CubivoxCore;

namespace CubivoxClient.Protocol.ServerBound
{
    public class UpdatePlayerPositionPacket : ServerBoundPacket
    {
        private Location location;
        public UpdatePlayerPositionPacket(Location location)
        {
            this.location = location;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes(location.x));
            stream.Write(BitConverter.GetBytes(location.y));
            stream.Write(BitConverter.GetBytes(location.z));
        }

        byte Packet.GetType()
        {
            return 0x02;
        }
    }
}
