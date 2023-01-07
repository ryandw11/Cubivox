using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CubivoxClient;
using CubivoxCore;

/// <summary>
/// This is the controller of cubivox. It contains <see cref="ClientCubivox"/>, which is the
/// client implementation of the Cubivox class.
/// <br/>
/// Get the Cubivox instance through <see cref="Cubivox.GetInstance"/>.
/// </summary>
public class CubivoxController : MonoBehaviour
{

    public GameObject playerPrefab;

    private ClientCubivox clientCubivox;

    private static CubivoxController instance;
    private Queue<Action> mainThreadQueue = new Queue<Action>();
    private volatile bool hasItemsInQueue = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Debug.Log("test");
        this.clientCubivox = new ClientCubivox();
        this.clientCubivox.OnEnable();

        Debug.Log("Connecting to server!");
        this.clientCubivox.ConnectToServer("localhost", 5555);
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

    public ClientCubivox GetCubivox()
    {
        return clientCubivox;
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
