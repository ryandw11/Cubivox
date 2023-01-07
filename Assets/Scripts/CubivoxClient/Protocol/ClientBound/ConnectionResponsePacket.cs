using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CubivoxClient.Players;
using CubivoxClient.Protocol.DataFormats;

namespace CubivoxClient.Protocol.ClientBound
{
    public class ConnectionResponsePacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] rawDataSize = new byte[4];
            if (stream.Read(rawDataSize, 0, 4) != 4) return false;
            int dataSize = BitConverter.ToInt32(rawDataSize);

            byte[] data = new byte[dataSize];
            if (stream.Read(data, 0, dataSize) != dataSize) return false;
            string jsonString = Encoding.ASCII.GetString(data);
            var jsonObj = JsonUtility.FromJson<ConnectionResponseData>(jsonString);

            Debug.Log(jsonString);
            CubivoxController.RunOnMainThread(() =>
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
                if(objs.Length == 0)
                {
                    Debug.LogError("Controller not instantiated!");
                    return;
                }
                CubivoxController controller = objs[0].GetComponent<CubivoxController>();

                foreach (var player in jsonObj.Players)
                {
                    // Instantiate the player object.
                    var obj = GameObject.Instantiate(controller.playerPrefab, new Vector3((float)player.X, (float)player.Y, (float)player.Z), controller.playerPrefab.transform.rotation);
                    obj.name = player.Uuid;
                    // This is added to the client player list on object start.
                    ClientPlayer clientPlayer = obj.GetComponent<ClientPlayer>();
                    clientPlayer.Username = player.Username;
                    // TODO:: Error handle this.
                    clientPlayer.Uuid = Guid.Parse(player.Uuid);
                }

                controller.GetCubivox().CurrentState = GameState.PLAYING;
            });
            return true;
        }

        byte Packet.GetType()
        {
            return 0x01;
        }
    }
}
