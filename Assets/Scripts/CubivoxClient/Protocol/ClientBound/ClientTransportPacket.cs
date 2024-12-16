using CubivoxCore;
using CubivoxClient.Networking;
using System;
using System.Net.Sockets;
using CubivoxCore.Networking.DataFormat;
using System.Text;

namespace CubivoxClient.Protocol.ClientBound
{
    public class ClientTransportPacket : ClientBoundPacket
    {
        private ClientTransportRegistry transportRegistery;

        public ClientTransportPacket()
        {
            transportRegistery = (ClientTransportRegistry) Cubivox.GetTransportRegistry();
        }

        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] primitiveBuffer = new byte[4];

            NetworkingUtils.ReadFromNetwork(primitiveBuffer, 2, stream);
            short keyLength = BitConverter.ToInt16(primitiveBuffer, 0);

            byte[] keyBuffer = new byte[keyLength];
            NetworkingUtils.ReadFromNetwork(keyBuffer, keyLength, stream);
            string keyString = Encoding.ASCII.GetString(keyBuffer, 0, keyLength);

            ControllerKey key;
            try
            {
                key = new ControllerKey(keyString);
            }
            catch(ArgumentException)
            {
                return false;
            }

            NetworkingUtils.ReadFromNetwork(primitiveBuffer, 4, stream);
            int dataLength = BitConverter.ToInt32(primitiveBuffer);

            if( dataLength > TransportFormat.MaxSize )
            {
                return false;
            }

            byte[] dataBuffer = new byte[dataLength];
            NetworkingUtils.ReadFromNetwork(dataBuffer, dataLength, stream);

            if ( !transportRegistery.InvokeTransport(key, dataBuffer) )
            {
                clientCubivox.GetLogger().Error($"An error has occured trying to invoke transport {key}, has it been registered?");
            }

            return true;
        }

        byte Packet.GetType()
        {
            return 0x0A;
        }
    }
}
