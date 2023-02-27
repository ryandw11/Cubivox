using CubivoxClient.Players;
using System;
using System.Text;
using System.Net.Sockets;

using UnityEngine;

namespace CubivoxClient.Protocol.ClientBound
{
    public class DisconnectPacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] rawReasonLength = new byte[2];
            if (stream.Read(rawReasonLength, 0, 2) != 2) return false;

            ushort reasonLength = BitConverter.ToUInt16(rawReasonLength);

            // TODO :: Add a limited length.
            byte[] rawReason = new byte[reasonLength];
            if (stream.Read(rawReason, 0, reasonLength) != reasonLength) return false;

            string reason = Encoding.ASCII.GetString(rawReason);

            clientCubivox.DisconnectClient(reason);

            return true;
        }

        byte Packet.GetType()
        {
            return 0x05;
        }
    }
}
