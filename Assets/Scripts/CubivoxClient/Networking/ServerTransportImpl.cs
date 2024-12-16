using CubivoxClient.Protocol.ServerBound;
using CubivoxCore;
using CubivoxCore.Networking;
using CubivoxCore.Networking.DataFormat;
using System;

namespace CubivoxClient.Networking
{
    // Client -> Server
    public sealed class ServerTransportImpl<T> : ServerTransport<T> where T : Delegate
    {

        public ServerTransportImpl(ControllerKey key) : base(key)
        {
        }

        public override void Invoke(params object[] parameters)
        {
            TransportFormat.AssertTypeMatch(GetParameterTypes(), parameters, true);

            ServerTransportPacket transportPacket = new ServerTransportPacket(mKey, parameters);
            ClientCubivox.GetClientInstance().SendPacketToServer(transportPacket);
        }

        public override void Register(T method)
        {
            // Server Only
        }
    }
}
