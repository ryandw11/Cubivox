using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CubivoxClient;
using CubivoxCore;
using System.IO;

/// <summary>
/// This is the controller of cubivox. It contains <see cref="ClientCubivox"/>, which is the
/// client implementation of the Cubivox class.
/// <br/>
/// Get the Cubivox instance through <see cref="Cubivox.GetInstance"/>.
/// </summary>
public class CubivoxController : MonoBehaviour
{
    /// <summary>
    /// The folder to store persistent user game data.
    /// 
    /// On Windows:
    /// C:\Users\[USERNAME]\AppData\Roaming\Cubivox
    /// </summary>
    public static readonly string scGameDataFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cubivox");
    public static readonly string scModsFolder = Path.Join(scGameDataFolder, "Mods");

    public GameObject playerPrefab;
    public CubivoxScene cubivoxScene;

    private ClientCubivox clientCubivox;

    private static CubivoxController instance;
    private Queue<Action> mainThreadQueue = new Queue<Action>();
    private volatile bool hasItemsInQueue = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Debug.Log("Changing instances of the CubivoxController"); 
        }

        instance = this;

        if (!ClientCubivox.HasInstance())
        {
            // The game has started for the first time.
            EnsureGameDataFolders();

            // Setup Cubivox Client
            Debug.Log("Initalizing Client Cubivox...");
            this.clientCubivox = new ClientCubivox(cubivoxScene);
            this.clientCubivox.OnEnable();

            // Connect to the server if starting off in the player scene.
            if(cubivoxScene == CubivoxScene.PlayScene)
            {
                Debug.Log("Detected Debug Player, Connecting to default server...");
                System.Random rand = new System.Random();
                clientCubivox.ConnectToServer("localhost", 5555, $"Test{rand.Next(0, 100)}");
            }
        }
        else
        {
            clientCubivox = ClientCubivox.GetClientInstance();
        }
    }

    // Update is called once per frame
    void Update()
    {
        clientCubivox.Update();

        if(hasItemsInQueue)
        {
            lock (mainThreadQueue)
            {
                while(mainThreadQueue.Count != 0)
                {
                    mainThreadQueue.Dequeue().Invoke();
                }
                hasItemsInQueue = false;
            }
        }
    }

    void OnApplicationQuit()
    {
        clientCubivox.OnApplicationQuit();
    }

    public ClientCubivox GetCubivox()
    {
        return clientCubivox;
    }

    /// <summary>
    /// Ensures that the game data folders habe been created and is valid.
    /// 
    /// Folders Currently Include:
    /// - Mod Folder
    /// </summary>
    public void EnsureGameDataFolders()
    {
        if (!Directory.Exists(scGameDataFolder))
        {
            Directory.CreateDirectory(scGameDataFolder);
        }
        if (!Directory.Exists(scModsFolder))
        {
            Directory.CreateDirectory(scModsFolder);
        }
    }

    public static void RunOnMainThread(Action action)
    {
        lock(instance.mainThreadQueue)
        {
            instance.mainThreadQueue.Enqueue(action);
        }
        instance.hasItemsInQueue = true;
    }
}
