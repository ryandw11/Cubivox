using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using CubivoxCore.Worlds;

using CubivoxClient.Worlds;

/// <summary>
/// This keeps track of the <see cref="World"/> that the client is currently in.
/// <br/>
/// Obtain the instance of this class through <see cref="GetInstance"/>.
/// </summary>
public class WorldManager : MonoBehaviour
{
    private ClientWorld currentWorld;
    private static WorldManager instance;

    private Queue<Action> mainThreadQueue = new Queue<Action>();

    private Vector3 mainCameraPos;
    int RenderDistance = 16;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        currentWorld = new ClientWorld();
    }

    public void AddAction(Action action)
    {
        lock (mainThreadQueue)
        {
            mainThreadQueue.Enqueue(action);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mainCameraPos = Camera.main.transform.position;

        lock(mainThreadQueue)
        {
            if (mainThreadQueue.Count > 0)
            {
                mainThreadQueue.Dequeue().Invoke();
            }
            if (mainThreadQueue.Count > 0)
            {
                mainThreadQueue.Dequeue().Invoke();
            }
            if (mainThreadQueue.Count > 0)
            {
                mainThreadQueue.Dequeue().Invoke();
            }
        }
    }

    public ClientWorld GetCurrentWorld()
    {
        return currentWorld;
    }

    public static WorldManager GetInstance()
    {
        return instance;
    }
}
