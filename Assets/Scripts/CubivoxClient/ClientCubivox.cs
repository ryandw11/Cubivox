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
using System.IO;

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

        public ClientCubivox(CubivoxScene currentScene)
        {
            instance = this;
            itemRegistry = new ClientItemRegistry();
            textureAtlas = new ClientTextureAtlas(new List<ClientAtlasTexture>(), "/tmp");
            players = new List<ClientPlayer>();
            packetList = new Dictionary<byte, ClientBoundPacket>();

            CurrentState = currentScene == CubivoxScene.TitleScene ? GameState.TITLE_SCREEN : GameState.DISCONNECTED;

            // Register Packets
            RegisterClientBoundPacket(new ConnectionResponsePacket());
            RegisterClientBoundPacket(new PlayerConnectPacket());
            RegisterClientBoundPacket(new PlayerPositionUpdatePacket());
            RegisterClientBoundPacket(new PlayerDisconnectPacket());
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

        public static bool HasInstance()
        {
            return instance != null;
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
            if (!client.Connected)
            {
                throw new Exception("The client is not connected to a server!");
            }

            client.GetStream().WriteByte(packet.GetType());
            packet.WritePacket(client.GetStream());
            client.GetStream().Flush();
        }

        public void ConnectToServer(string ip, int port, string username)
        {
            client = new TcpClient(ip, port);
            CurrentState = GameState.CONNECTING;
            SendPacketToServer(new ConnectPacket(username, Guid.NewGuid()));
        }

        public void RegisterClientBoundPacket(ClientBoundPacket clientBoundPacket)
        {
            packetList.Add(clientBoundPacket.GetType(), clientBoundPacket);
        }

        internal void HandleUserDisconnect(ClientPlayer clientPlayer)
        {
            if(clientPlayer.IsLocalPlayer)
            {
                Debug.LogError("An attempt was made to disconnect a the local player!");
            }
            players.Remove(clientPlayer);
        }

        public async void Update()
        {
            if (client != null && IsInNetworkingGameState())
            {
                if (handlePacketsTask == null)
                {
                    handlePacketsTask = ReadPackets();
                }

                if (handlePacketsTask.IsCompleted)
                {
                    try
                    {
                        await handlePacketsTask;
                        handlePacketsTask = ReadPackets();
                    } catch(IOException)
                    {
                        Debug.Log("[Networking] Disconnected from the game!");
                        client.Close();
                        handlePacketsTask = null;
                        CurrentState = GameState.DISCONNECTED;
                        client = null;
                    }
                }
            }

            if(CurrentState == GameState.CONNECTED_LOADING)
            {
                if(WorldManager.GetInstance().GetCurrentWorld().GetLoadedChunks().Count == 6400)
                {
                    CurrentState = GameState.PLAYING;
                    GameObject localPlayer = GameObject.Find("LocalPlayerCapsule");
                    if(localPlayer == null)
                    {
                        Debug.LogError("Cannot find local player game object!");
                    }
                    else
                    {
                        localPlayer.GetComponent<Rigidbody>().useGravity = true;
                    }
                }
            }
        }

        public bool IsInNetworkingGameState()
        {
            return CurrentState == GameState.CONNECTING || CurrentState == GameState.CONNECTED_LOADING || CurrentState == GameState.PLAYING;
        }

        private async Task ReadPackets()
        {
            NetworkStream stream = client.GetStream();
            byte[] id = new byte[1];
            if(await stream.ReadAsync(id, 0, 1) != 1)
            {
                Debug.LogWarning("[Networking] Error: Could not read byte from NetworkStream!");
                return;
            }
            try
            {
                if(!packetList[id[0]].ProcessPacket(this, stream))
                    Debug.LogWarning($"[Networking] Error: Invalid packet recived from Server! Packet Id: {(int)id[0]}");
            } catch(KeyNotFoundException)
            {
                Debug.LogWarning($"[Networking] Error: No packet with id {(int)id[0]} exists!");
            }
        }
    }
}
