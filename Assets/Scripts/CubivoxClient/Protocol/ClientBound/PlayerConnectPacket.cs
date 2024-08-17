using CubivoxClient.Players;
using CubivoxCore.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CubivoxClient.Protocol.ClientBound
{
    public class PlayerConnectPacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] rawUsername = new byte[25];
            NetworkingUtils.FillBufferFromNetwork(rawUsername, stream);
            string username = Encoding.ASCII.GetString(rawUsername);
            byte[] rawUuid = new byte[16];
            NetworkingUtils.FillBufferFromNetwork(rawUuid, stream);
            Guid uuid = new Guid(rawUuid);

            byte[] locBuffer = new byte[8];
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            double x = BitConverter.ToDouble(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            double y = BitConverter.ToDouble(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            double z = BitConverter.ToDouble(locBuffer);

            CubivoxScheduler.RunOnMainThread(() =>
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
                if (objs.Length == 0)
                {
                    Debug.LogError("Controller not instantiated!");
                    return;
                }
                CubivoxController controller = objs[0].GetComponent<CubivoxController>();

                // Instantiate the player object.
                var obj = GameObject.Instantiate(controller.playerPrefab, new Vector3((float)x, (float)y, (float)z), controller.playerPrefab.transform.rotation);
                obj.name = uuid.ToString();
                // This is added to the client player list on object start.
                ClientPlayer clientPlayer = obj.GetComponent<ClientPlayer>();
                clientPlayer.Username = username;
                clientPlayer.Uuid = uuid;

                Debug.Log($"[Networking] {username} has joined the game!");
            });

            return true;
        }

        byte Packet.GetType()
        {
            return 0x02;
        }
    }
}
