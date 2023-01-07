using System.Net.Sockets;

namespace CubivoxClient.Protocol
{
    public interface ServerBoundPacket : Packet
    {
        void WritePacket(NetworkStream stream);
    }
}
