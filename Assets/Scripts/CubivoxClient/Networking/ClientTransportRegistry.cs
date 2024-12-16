using CubivoxClient.Protocol.DataFormats;
using CubivoxCore;
using CubivoxCore.Exceptions;
using CubivoxCore.Networking;
using CubivoxCore.Networking.DataFormat;
using CubivoxCore.Players;
using System;

namespace CubivoxClient.Networking
{
    public sealed class ClientTransportRegistry : TransportRegistry
    {
        // Server -> Client
        public override void Transport<T>(Player player, params object[] parameters)
        {
            throw new InvalidEnvironmentException("This method can only be called on the server!");
        }

        // Client -> Server
        public override void Transport<T>(params object[] parameters)
        {
            IServerTransport transport;
            if( mServerTransports.TryGetValue(typeof(T), out transport) )
            {
                transport.Invoke(parameters);
            }
        }

        protected override ClientTransport<T> CreateClientTransport<T>(ControllerKey key)
        {
            return new ClientTransportImpl<T>(key);
        }

        protected override ServerTransport<T> CreateServerTransport<T>(ControllerKey key)
        {
            return new ServerTransportImpl<T>(key);
        }

        internal bool InvokeTransport(ControllerKey key, byte[] data)
        {
            Type type;
            if( mTypeMap.TryGetValue(key, out type) )
            {
                IClientTransport transport = mClientTransports[type];
                var clientTransport = (InvocableTransport)transport;

                var objs = TransportFormat.ReadObjects(transport.GetParameterTypes(), data);
                if ( objs == null )
                {
                    return false;
                }

                clientTransport.InternalInvoke(objs.ToArray());
                return true;
            }

            return false;
        }
    }
}
