using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using CubivoxClient.Items.Voxels;
using CubivoxClient.Texturing;
using CubivoxCore;
using CubivoxCore.BaseGame.VoxelDefs;
using UnityEngine;

using CubivoxClient.Players;
using CubivoxClient.Protocol;
using CubivoxClient.Protocol.ServerBound;
using CubivoxClient.Protocol.ClientBound;

namespace CubivoxClient
{
    public class ClientCubivox : Cubivox
    {
        public static readonly short PROTOCOL_VERSION = 0x0;

        private List<ClientPlayer> players;
        private Dictionary<byte, ClientBoundPacket> packetList;

        private TcpClient? client;
        private Task handlePacketsTask;

        public GameState CurrentState { get; internal set; }

        public ClientCubivox()
        {
            instance = this;
            itemRegistry = new ClientItemRegistry();
            textureAtlas = new ClientTextureAtlas(new List<ClientAtlasTexture>(), "/tmp");
            players = new List<ClientPlayer>();
            packetList = new Dictionary<byte, ClientBoundPacket>();

            CurrentState = GameState.NOT_CONNECTED;

            // Register Packets
            RegisterClientBoundPacket(new ConnectionResponsePacket());
            RegisterClientBoundPacket(new PlayerConnectPacket());
            RegisterClientBoundPacket(new PlayerPositionUpdatePacket());
        }

        public override EnvType GetEnvType()
        {
            return EnvType.CLIENT;
        }

        public override void OnEnable()
        {
            itemRegistry.RegisterItem(new AirVoxel());
            TestVoxel testVoxel = new TestVoxel(this);
            itemRegistry.RegisterItem(testVoxel);
            textureAtlas.RegisterTexture(testVoxel.GetAtlasTexture(), true);
        }

        public static ClientCubivox GetClientInstance()
        {
            return (ClientCubivox) GetInstance();
        }

        public List<ClientPlayer> GetPlayers()
        {
            return players;
        }

        public void SendPacketToServer(ServerBoundPacket packet)
        {
            if(client == null)
            {
                throw new Exception("The client is not connected to a server!");
            }

            client.GetStream().WriteByte(packet.GetType());
            packet.WritePacket(client.GetStream());
            client.GetStream().Flush();
        }

        public void ConnectToServer(string ip, int port)
        {
            client = new TcpClient(ip, port);
            CurrentState = GameState.CONNECTING;
            System.Random rand = new System.Random();
            SendPacketToServer(new ConnectPacket($"Test{rand.Next(0, 100)}", Guid.NewGuid()));
        }

        public void RegisterClientBoundPacket(ClientBoundPacket clientBoundPacket)
        {
            packetList.Add(clientBoundPacket.GetType(), clientBoundPacket);
        }

        public async void Update()
        {
            if(client != null)
            {
                if (handlePacketsTask == null)
                {
                    handlePacketsTask = ReadPackets();
                }

                if (handlePacketsTask.IsCompleted)
                {
                    await handlePacketsTask;
                    handlePacketsTask = ReadPackets();
                }
            }
        }

        private async Task ReadPackets()
        {
            NetworkStream stream = client.GetStream();
            byte[] id = new byte[1];
            if(await stream.ReadAsync(id, 0, 1) != 1)
            {
                Console.WriteLine("Error: Invalid packet!");
                return;
            }
            try
            {
                packetList[id[0]].ProcessPacket(this, stream);
                Console.WriteLine("Read Pakcet!");
            } catch(KeyNotFoundException)
            {
                Console.WriteLine($"Error: Invalid Packet Detected for packet: {(int)id[0]}");
            }
        }
    }
}
