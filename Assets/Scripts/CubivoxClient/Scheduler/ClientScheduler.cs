using CubivoxCore.Scheduler;

using System;
using System.Collections.Concurrent;
using System.Threading;

using UnityEngine;

namespace CubivoxClient.Scheduler
{
    public class ClientScheduler : MonoBehaviour
    {
        private class InternalScheduler : CubivoxScheduler
        {
            public ConcurrentQueue<Action> mActionQueue;
            private Thread mMainThread;

            public InternalScheduler(Thread mainThread)
            {
                mInstance = this;
                mActionQueue = new ConcurrentQueue<Action>();
                mMainThread = mainThread;
            }

            public override void RunOnMainThreadImpl(Action action)
            {
                mActionQueue.Enqueue(action);
            }

            public override void RunOnMainThreadSynchronizedImpl(Action action)
            {
                if(Thread.CurrentThread == mMainThread)
                {
                    action();
                    return;
                }

                mActionQueue.Enqueue(action);
            }
        }

        private InternalScheduler mScheduler;

        void Awake()
        {
            mScheduler = new InternalScheduler(Thread.CurrentThread);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            Action action;
            while (mScheduler.mActionQueue.TryDequeue(out action))
            {
                action();
            }
        }
    }
}
