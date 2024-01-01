using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class NetworkingUtils
{
    public static void FillBufferFromNetwork(byte[] buffer, NetworkStream stream)
    {
        int buffIndex = 0;
        while (buffIndex < buffer.Length)
        {
            buffIndex += stream.Read(buffer, buffIndex, buffer.Length - buffIndex);
        }
    }
}
