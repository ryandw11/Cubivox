using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace CubivoxClient.Protocol
{
    public interface ClientBoundPacket : Packet
    {
        bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream);
    }
}
