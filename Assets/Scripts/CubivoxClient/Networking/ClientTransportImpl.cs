using CubivoxCore;
using CubivoxCore.Exceptions;
using CubivoxCore.Networking;
using CubivoxCore.Players;
using System;
using System.Reflection;

namespace CubivoxClient.Networking
{
    // Server -> Client
    public sealed class ClientTransportImpl<T> : ClientTransport<T>, InvocableTransport where T : Delegate
    {
        private Delegate mDelegate;

        public ClientTransportImpl(ControllerKey key) : base(key)
        {
        }

        public override void Invoke(Player player, params object[] parameters)
        {
            // Server Only
        }

        public override void Register(T method)
        {
            if ( mDelegate == null )
            {
                mDelegate = method;
            }
            else
            {
                mDelegate = Delegate.Combine(mDelegate, method);
            }
        }

        void InvocableTransport.InternalInvoke(params object[] parameters)
        {
            mDelegate?.DynamicInvoke(parameters);
        }
    }
}
