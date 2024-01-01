using CubivoxClient.Players;
using System;
using System.Net.Sockets;

using UnityEngine;

namespace CubivoxClient.Protocol.ClientBound
{
    public class PlayerDisconnectPacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] rawUuid = new byte[16];
            NetworkingUtils.FillBufferFromNetwork(rawUuid, stream);

            Guid uuid = new Guid(rawUuid);

            CubivoxController.RunOnMainThread(() =>
            {
                GameObject user = GameObject.Find(uuid.ToString());
                if(user == null)
                {
                    Debug.LogWarning($"[Networking] Recieved user disconnect packet from server, but could not find the requested player. ({uuid})");
                    return;
                }
                ClientPlayer clientPlayer = user.GetComponent<ClientPlayer>();
                clientCubivox.HandleUserDisconnect(clientPlayer);

                GameObject.Destroy(user);

                Debug.Log($"[Networking] {uuid} has left the game!");
            });

            return true;
        }

        byte Packet.GetType()
        {
            return 0x04;
        }
    }
}
