using CubivoxCore;
using CubivoxCore.Exceptions;
using CubivoxCore.Networking;
using CubivoxCore.Players;
using System;

namespace CubivoxClient.Networking
{
    internal interface InvocableTransport
    {
        internal void InternalInvoke(params object[] parameters);
    }
}
