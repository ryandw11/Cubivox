using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System;

/**
 * <summary>This class acts as a ThreadPool to allow the game to run operations Async without spinning up new temporary threads.
 * <para>Two threads are kept alive at all time to accept requests.</para>
 * <para>Get this class via <see cref="GetInstance"/></para>
 * </summary>
 */
public class ThreadManager : MonoBehaviour
{
    private Thread[] threads = new Thread[5];

    private static ThreadManager instance;

    private Queue<Action> threadQueue = new Queue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        // Thread 1
        threads[0] = new Thread(() => 
        {
            while (true)
            {
                lock (threadQueue)
                {
                    if (threadQueue.Count > 0)
                    {
                        threadQueue.Dequeue().Invoke();
                    }
                }
            }
        });
        threads[0].Start();

        // Thread 2
        threads[1] = new Thread(() =>
        {
            while (true)
            {
                lock (threadQueue)
                {
                    if (threadQueue.Count > 0)
                    {
                        threadQueue.Dequeue().Invoke();
                    }
                }
            }
        });
        threads[1].Start();
    }

    /**
     * <summary>Add an action to the queue to be executed on one of the two threads.</summary>
     * <param name="act">The action to be executed async.</param>
     */
    public void AddAction(Action act)
    {
        lock (threadQueue)
        {
            this.threadQueue.Enqueue(act);
        }
    }

    /**
     * <summary>Get the instance of the thread manager.</summary>
     * <returns>The instance of the thread manager.</returns>
     */
    public static ThreadManager GetInstance()
    {
        return instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // Stop the threads when the thread manager object is destroyed.
        foreach(Thread thr in threads)
        {
            if(thr != null)
                thr.Abort();
        }
    }
}
