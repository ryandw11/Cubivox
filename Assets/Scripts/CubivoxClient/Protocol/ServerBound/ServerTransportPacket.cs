using System;
using System.Net.Sockets;
using System.Text;
using CubivoxCore;
using CubivoxCore.Networking.DataFormat;

namespace CubivoxClient.Protocol.ServerBound
{
    public class ServerTransportPacket : ServerBoundPacket
    {
        private ControllerKey controllerKey;
        private object[] parameters;
        public ServerTransportPacket(ControllerKey controllerKey, object[] parameters)
        {
            this.controllerKey = controllerKey;
            this.parameters = parameters;
        }

        public void WritePacket(NetworkStream stream)
        {
            string key = controllerKey.ToString();
            if( key.Length >= short.MaxValue )
            {
                return;
            }

            byte[] data = TransportFormat.WriteObjects(parameters);

            if(data == null)
            {
                return;
            }

            if(data.Length > TransportFormat.MaxSize)
            {
                return;
            }

            stream.Write(BitConverter.GetBytes((short)key.Length));
            stream.Write(Encoding.ASCII.GetBytes(key));
            stream.Write(BitConverter.GetBytes(data.Length));
            stream.Write(data);
        }

        byte Packet.GetType()
        {
            return 0x05;
        }
    }
}
