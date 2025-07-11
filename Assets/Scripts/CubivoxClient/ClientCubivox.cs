﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using CubivoxCore;
using CubivoxCore.BaseGame.VoxelDefs;
using CubivoxCore.Items;
using CubivoxCore.Mods;
using CubivoxCore.Players;
using CubivoxCore.Worlds;
using CubivoxCore.Worlds.Generation;

using CubivoxClient.Events;
using CubivoxClient.Loggers;
using CubivoxClient.Players;
using CubivoxClient.Protocol;
using CubivoxClient.Protocol.ServerBound;
using CubivoxClient.Protocol.ClientBound;
using CubivoxClient.Texturing;

using UnityEngine;
using CubivoxClient.Mods;
using CubivoxCore.Scheduler;
using CubivoxClient.Networking;

namespace CubivoxClient
{

    // When it comes to thread saftey the following points are assumed:
    // 1) Most items will not be modified after this class is constructed.
    // 2) Items that are modified and are required to be thread safe by the core are guareded by mLock.
    // 3) Everything else is not thread safe.
    public class ClientCubivox : Cubivox
    {
        public static readonly short PROTOCOL_VERSION = 0x0;

        internal string DisconnectionReason { get; set; } = "";

        private bool enabled = false;
        private List<ClientPlayer> players;
        private Dictionary<byte, ClientBoundPacket> packetList;
        private ClientLogger logger;

        private List<Mod> mods;

        private TcpClient? client;
        private Thread handlePacketsThread;
        private Thread mainThread;
        private volatile bool shouldTerminate = false;

        public GameState CurrentState { get; internal set; }

        private Guid localPlayerUuid;

        private ClientPlayer _localPlayer;
        public ClientPlayer LocalPlayer
        {
            get { return _localPlayer; }
            internal set
            {
                if (_localPlayer != null && value != null)
                {
                    throw new Exception("Cannot set the local place twice!");
                }

                _localPlayer = value;
                if (value != null)
                {
                    _localPlayer.Uuid = localPlayerUuid;
                }
            }
        }


        // General purpose lock to guard some of Cubivox's internal lists.
        // The following items are guarded:
        // 1) Online Player List
        private object mLock = new object();

        public ClientCubivox(CubivoxScene currentScene)
        {
            instance = this;
            textureAtlas = new ClientTextureAtlas(new List<ClientAtlasTexture>());
            itemRegistry = new ClientItemRegistry((ClientTextureAtlas) textureAtlas);
            eventManager = new ClientEventManager();
            transportRegistry = new ClientTransportRegistry();

            players = new List<ClientPlayer>();
            packetList = new Dictionary<byte, ClientBoundPacket>();

            logger = new ClientLogger("Cubivox");
            mods = new List<Mod>();

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
            RegisterClientBoundPacket(new ClientTransportPacket());

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

        public override ReadOnlyCollection<Player> GetOnlinePlayersImpl()
        {
            lock( mLock )
            {
                return players.Cast<Player>().ToList().AsReadOnly();
            }
        }

        public override ReadOnlyCollection<World> GetWorldsImpl()
        {
            throw new NotImplementedException("Worlds have not yet been implemented in the client!");
        }

        public override void LoadItemsStage(ItemRegistry itemRegistry)
        {
            RegisterBaseGameVoxels();
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

            LoadMods();
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

        /// <summary>
        /// Add a player to the list of online players.
        /// 
        /// <para>This method is thread safe.</para>
        /// </summary>
        /// <param name="player">The player to add.</param>
        internal void AddPlayer(ClientPlayer player)
        {
            lock (mLock)
            {
                players.Add(player);
            }
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
                CubivoxScheduler.RunOnMainThread(() =>
                {
                    DisconnectClient(reason);
                });
                return;
            }
            DisconnectionReason = reason;

            shouldTerminate = true;
            handlePacketsThread.Abort();
            if (client != null)
                client.Close();
            client = null;

            CurrentState = GameState.DISCONNECTED;
            LocalPlayer = null;
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

                Guid guid = Guid.NewGuid();
                SendPacketToServer(new ConnectPacket(username, guid));
                // TODO: Come up with a better system here for handeling the UUID
                localPlayerUuid = guid;
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

        /// <summary>
        /// Handle a user disconnecting form the game.
        /// 
        /// <para>This method is thread safe.</para>
        /// </summary>
        /// <param name="clientPlayer">The player to disconnect,</param>
        internal void HandleUserDisconnect(ClientPlayer clientPlayer)
        {
            if (clientPlayer.IsLocalPlayer)
            {
                GetLogger().Error("An attempt was made to disconnect a the local player!");
                return;
            }

            lock (mLock)
            {
                players.Remove(clientPlayer);
            }
        }

        internal void StartListeningForPackets()
        {
            if (client != null && IsInNetworkingGameState())
            {
                if (handlePacketsThread == null)
                {
                    handlePacketsThread = new Thread(() =>
                    {
                        while (!shouldTerminate)
                        {
                            try
                            {
                                if (client.GetStream().DataAvailable)
                                {
                                    ReadPackets();
                                }
                            }
                            catch (SocketException)
                            {
                                GetLogger().Info("[Networking] Disconnected from the game!");
                                DisconnectClient("Lost connection to host server.");
                                return;
                            }
                            catch (Exception ex)
                            {
                                if( !client.Connected )
                                {
                                    // Assume client was disconnected.
                                    DisconnectClient("Lost connection to host server.");
                                }
                                else
                                {
                                    DisconnectClient("An internal error has occured when processing packets!");
                                }
                                GetLogger().Error(ex.Message);
                                GetLogger().Error(ex.StackTrace);
                                return;
                            }

                        }
                    });
                    handlePacketsThread.Start();
                }
            }
        }

        /// <summary>
        /// Called on every Start (whether on title screen or plaing the game).
        /// </summary>
        public void Start()
        {
            // This will be null if on the title screen.
            hud = ClientHud.GetInstance();
        }

        public void Update()
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
                GetLogger().Warn("[Networking] Error: Could not read byte from NetworkStream!");
                return;
            }
            try
            {
                if (!packetList[id[0]].ProcessPacket(this, stream))
                {
                    GetLogger().Error($"[Networking] Error: Invalid packet recived from Server! Packet Id: {(int)id[0]}");
                    GetLogger().Error("[Networking] Clearing out the network stream.");
                    DisconnectClient("Invalid packet recieved from the server.");
                }
            } 
            catch(KeyNotFoundException ex)
            {
                GetLogger().Error($"[Networking] Error: No packet with id {(int)id[0]} exists!");
                GetLogger().Error("[Networking] Clearing out the network stream.");
                DisconnectClient("Invalid packet recieved from the server.");
            }
            catch (Exception ex)
            {
                GetLogger().Error("[Networking | ERROR] Unable to process packets from server, disconnecting...");
                GetLogger().Error(ex.Message);
                GetLogger().Error(ex.StackTrace);
                if(ex.InnerException != null)
                {
                    GetLogger().Error(ex.InnerException.Message);
                    GetLogger().Error(ex.InnerException.StackTrace);
                }
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


        /// <summary>
        /// Basic Mod Loader
        /// 
        /// Reads, loads, and setup the mods.
        /// </summary>
        private void LoadMods()
        {
            // TODO: This is a pretty hacky mod loader. This should be replaced with a better system later.
            DirectoryInfo modDirectory = new DirectoryInfo(CubivoxController.scModsFolder);
            foreach (FileInfo file in modDirectory.GetFiles())
            {
                var dll = Assembly.LoadFile(file.FullName);
                var resourceName = dll.GetName().Name + ".mod.json";

                string resource = null;
                using (Stream stream = dll.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        resource = reader.ReadToEnd();
                    }
                }

                if (resource == null)
                {
                    logger.Error($"Failed to load mod {file.Name}! Is the mod.json file in the right namespace?");
                    continue;
                }

                logger.Info(resource);

                ModDescriptionFile descriptionFile = ModDescriptionUtils.ClientToCoreDescriptionFile(JsonUtility.FromJson<ClientModDescriptionFile>(resource) );

                var mainModClass = dll.GetType(descriptionFile.MainClass);

                if (mainModClass == null)
                {
                    logger.Error($"Failed to load mod {file.Name}! Cannot find the mod's main class {descriptionFile.MainClass}!");
                    continue;
                }

                ClientLogger modLogger = new ClientLogger(descriptionFile.ModName);

                CubivoxMod mod = (CubivoxMod)Activator.CreateInstance(mainModClass, descriptionFile, modLogger);

                if (mod == null)
                {
                    logger.Error($"Failed to load mod {file.Name}! Unable to construct main mod class instance.");
                    continue;
                }

                mods.Add(mod);
                logger.Info($"Found and loaded mod {descriptionFile.ModName}.");
            }

            foreach (Mod mod in mods)
            {
                try
                {
                    mod.LoadItemsStage(itemRegistry);
                }
                catch (Exception ex)
                {
                    logger.Error($"An internal error has occur for {mod.GetName()}:");
                    logger.Error(ex.ToString());
                }
            }

            // Recacluate the texture atlas after loading all mods
            textureAtlas.RecalculateTextureAtlas();

            foreach (Mod mod in mods)
            {
                try
                {
                    mod.OnEnable();
                }
                catch (Exception ex)
                {
                    logger.Error($"An internal error has occur for {mod.GetName()}:");
                    logger.Error(ex.ToString());
                }
            }
        }
    }
}
