using System;
using System.Collections.Generic;

namespace CubivoxClient.Protocol.DataFormats
{
    [Serializable]
    public class ConnectionResponseData
    {
        public string ServerName;
        public Dictionary<string, string> VoxelMap;
        public PlayerData[] Players;

        [Serializable]
        public struct PlayerData
        {
            public string Username;
            public string Uuid;
            public double X;
            public double Y;
            public double Z;
        }
    }
}
