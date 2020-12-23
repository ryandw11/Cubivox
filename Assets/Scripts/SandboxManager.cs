﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>
 * This is the general manager for the game.
 * 
 * <para>Get the instance of this class using <see cref="GetInstance"/></para>
 * </summary>
 */
public class SandboxManager : MonoBehaviour
{
    private static SandboxManager instance;

    private Queue<Action> mainThreadQueue = new Queue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    /**
     * <summary>Get the instance of this class.</summary>
     * <returns>The instance of this class.</returns>
     */
    public static SandboxManager GetInstance()
    {
        return instance;
    }

    /**
     * <summary>This adds an action to be ran on the main thread.</summary>
     * <param name="act">The action to be added on the main thread.</param>
     */
    public void AddAction(Action act)
    {
        lock (mainThreadQueue)
        {
            mainThreadQueue.Enqueue(act);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Invokes three actions at a time.
        if (mainThreadQueue.Count > 0)
        {
            mainThreadQueue.Dequeue().Invoke();
        }
        /*
        if (mainThreadQueue.Count > 0)
        {
            mainThreadQueue.Dequeue().Invoke();
        }
        if (mainThreadQueue.Count > 0)
        {
            mainThreadQueue.Dequeue().Invoke();
        }
        */
    }
}