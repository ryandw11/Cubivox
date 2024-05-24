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
using System.Threading;
using CubivoxClient.Loggers;
using CubivoxCore.BaseGame;
using CubivoxCore.Worlds.Generation;

namespace CubivoxClient
{
    public class ClientCubivox : Cubivox
    {
        public static readonly short PROTOCOL_VERSION = 0x0;

        internal string DisconnectionReason { get; set; } = "";

        private bool enabled = false;
        private List<ClientPlayer> players;
        private Dictionary<byte, ClientBoundPacket> packetList;

        private ClientLogger logger;

        private TcpClient? client;
        private Thread handlePacketsThread;
        private Thread mainThread;

        public GameState CurrentState { get; internal set; }

        public ClientCubivox(CubivoxScene currentScene)
        {
            instance = this;
            itemRegistry = new ClientItemRegistry();
            textureAtlas = new ClientTextureAtlas(new List<ClientAtlasTexture>(), "/tmp");
            players = new List<ClientPlayer>();
            packetList = new Dictionary<byte, ClientBoundPacket>();

            logger = new ClientLogger("Cubivox");

            CurrentState = currentScene == CubivoxScene.TitleScene ? GameState.TITLE_SCREEN : GameState.DISCONNECTED;

            // Register Packets
            RegisterClientBoundPacket(new ConnectionResponsePacket());
            RegisterClientBoundPacket(new PlayerConnectPacket());
            RegisterClientBoundPacket(new PlayerPositionUpdatePacket());
            RegisterClientBoundPacket(new PlayerDisconnectPacket());
            RegisterClientBoundPacket(new DisconnectPacket());
            RegisterClientBoundPacket(new CBBreakVoxelPacket());
            RegisterClientBoundPacket(new CBPlaceVoxelPacket());
            RegisterClientBoundPacket(new CBLoadChunkPacket());
            RegisterClientBoundPacket(new CBChatMessagePacket());

            mainThread = Thread.CurrentThread;
        }

        public override EnvType GetEnvType()
        {
            return EnvType.CLIENT;
        }

        public override CubivoxCore.Console.Logger GetLogger()
        {
            return logger;
        }

        public override void LoadItemsStage(ItemRegistry itemRegistry)
        {
            itemRegistry.RegisterItem(new AirVoxel());

            // TODO: Modify RegisterItem to also register the texture.
            TestVoxel testVoxel = new TestVoxel(this);
            itemRegistry.RegisterItem(new TestVoxel(this));
            textureAtlas.RegisterTexture(testVoxel.GetAtlasTexture(), true);
        }

        public override void LoadGeneratorsStage(GeneratorRegistry generatorRegistry)
        {
            // The generator stage only ever triggers on the server.
        }

        public override void OnEnable()
        {
            if (enabled)
            {
                throw new Exception("Client Cubivox cannot be enabled twice.");
            }
            enabled = true;

            LoadItemsStage(itemRegistry);
        }

        public static ClientCubivox GetClientInstance()
        {
            return (ClientCubivox) GetInstance();
        }

        public static bool HasInstance()
        {
            return instance != null;
        }

        public ClientItemRegistry GetClientItemRegistry()
        {
            return (ClientItemRegistry) itemRegistry;
        }

        public List<ClientPlayer> GetPlayers()
        {
            return players;
        }

        /// <summary>
        /// Disconnect the client from the server with a specific reason. This will close the TCP connection to the server.
        /// This is a thread-safe action and can be called from any thread. (It may not trigger immediatley if not called on the main thread).
        /// </summary>
        /// <param name="reason">The reason fo the disconnect.</param>
        public void DisconnectClient(string reason)
        {
            if (Thread.CurrentThread != mainThread)
            {
                CubivoxController.RunOnMainThread(() =>
                {
                    DisconnectClient(reason);
                });
                return;
            }
            DisconnectionReason = reason;

            handlePacketsThread.Abort();
            if (client != null)
                client.Close();
            client = null;

            CurrentState = GameState.DISCONNECTED;
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
            try
            {
                client = new TcpClient(ip, port);
                CurrentState = GameState.CONNECTING;
                SendPacketToServer(new ConnectPacket(username, Guid.NewGuid()));
            } 
            catch (SocketException)
            {
                if(client != null) client.Close();
                DisconnectionReason = $"Could not connect to host server {ip}:{port}.";
                CurrentState = GameState.DISCONNECTED;
            }

            // Handle The Async Reading of Packets:
            StartListeningForPackets();
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

        internal void StartListeningForPackets()
        {
            if (client != null && IsInNetworkingGameState())
            {
                if (handlePacketsThread == null)
                {
                    handlePacketsThread = new Thread(() =>
                    {
                        while (true)
                        {
                            if ( client.GetStream().DataAvailable )
                            {
                                try
                                {
                                    ReadPackets();
                                }
                                catch (IOException)
                                {
                                    Debug.Log("[Networking] Disconnected from the game!");
                                    DisconnectClient("Lost connection to host server.");
                                }
                            }
                        }
                    });
                    handlePacketsThread.Start();
                }
            }
        }

        public async void Update()
        {

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

        public void OnApplicationQuit()
        {
            handlePacketsThread.Abort();
            if (client != null)
                client.Close();
            client = null;
        }

        public bool IsInNetworkingGameState()
        {
            return CurrentState == GameState.CONNECTING || CurrentState == GameState.CONNECTED_LOADING || CurrentState == GameState.PLAYING;
        }

        private void ReadPackets()
        {
            NetworkStream stream = client.GetStream();
            byte[] id = new byte[1];
            if(stream.Read(id, 0, 1) != 1)
            {
                Debug.LogWarning("[Networking] Error: Could not read byte from NetworkStream!");
                return;
            }
            try
            {
                //Debug.Log("Reading Packet: " + packetList[id[0]]);
                if (!packetList[id[0]].ProcessPacket(this, stream))
                {
                    Debug.LogWarning($"[Networking] Error: Invalid packet recived from Server! Packet Id: {(int)id[0]}");
                    Debug.LogWarning("[Networking] Clearing out the network stream.");
                    // Clear out the network stream.
                    byte[] buffer = new byte[4096];
                    while(stream.DataAvailable)
                    {
                        stream.Read(buffer, 0, buffer.Length);
                    }
                }
                //Debug.Log("Done Reading Packet");
            } 
            catch(KeyNotFoundException ex)
            {
                Debug.LogWarning($"[Networking] Error: No packet with id {(int)id[0]} exists!");
                Debug.LogWarning("[Networking] Clearing out the network stream.");
                // Clear out the network stream.
                byte[] buffer = new byte[4096];
                while (stream.DataAvailable)
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                Debug.LogError("[Networking | ERROR] Unable to process packets from server, disconnecting...");
                DisconnectClient("Invalid response recieved from server.");
            }
        }

        public override void AssertServer()
        {
            throw new Exception();
        }

        public override void AssertClient()
        {
        }
    }
}
