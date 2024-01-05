using CubivoxClient.Worlds;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace CubivoxClient.Protocol.ClientBound
{
    public class CBChatMessagePacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] typeBuffer = new byte[1];
            NetworkingUtils.FillBufferFromNetwork(typeBuffer, stream);

            if (typeBuffer[0] == 0x0)
            {
                byte[] messageLengthBuffer = new byte[2];
                NetworkingUtils.FillBufferFromNetwork(messageLengthBuffer, stream);
                short messageLength = BitConverter.ToInt16(messageLengthBuffer);
                byte[] messageBuffer = new byte[messageLength];
                NetworkingUtils.FillBufferFromNetwork(messageBuffer, stream);
                string message = Encoding.ASCII.GetString(messageBuffer, 0, messageLength);

                Debug.Log(messageLength);
                Debug.Log(string.Join(" ,", messageBuffer.ToList()));

                Debug.Log("Got Chat Message: " + message);

                CubivoxController.RunOnMainThread(() => {
                    ChatUI.Instance.SendChatMessage(message);
                });
            }
            else if (typeBuffer[0] == 0x1)
            {
                Debug.Log("Player chat message not yet implemented!");
            }

            return true;
        }

        byte Packet.GetType()
        {
            return 0x09;
        }
    }
}
