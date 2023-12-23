using System;
using System.Net.Sockets;
using System.Text;

namespace CubivoxClient.Protocol.ServerBound
{
    public class ConnectPacket : ServerBoundPacket
    {
        private string username;
        private Guid uuid;

        public ConnectPacket(string username, Guid uuid)
        {
            this.username = username;
            this.uuid = uuid;
        }

        public void WritePacket(NetworkStream stream)
        {
            stream.Write(BitConverter.GetBytes(ClientCubivox.PROTOCOL_VERSION));

            byte[] name = Encoding.ASCII.GetBytes(username);
            stream.Write(name, 0, Math.Min(name.Length, 25));
            // Pad out the name if it is less that 25 characters.
            if (name.Length < 25)
            {
                byte[] zeroArray = new byte[25 - name.Length];
                stream.Write(zeroArray, 0, zeroArray.Length);
            }

            stream.Write(uuid.ToByteArray());
        }

        byte Packet.GetType()
        {
            return 0x00;
        }
    }
}
