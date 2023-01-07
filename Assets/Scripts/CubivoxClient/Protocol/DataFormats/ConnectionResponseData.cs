using System;

namespace CubivoxClient.Protocol.DataFormats
{
    [Serializable]
    public class ConnectionResponseData
    {
        public string ServerName;
        public string[] Voxels;
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
