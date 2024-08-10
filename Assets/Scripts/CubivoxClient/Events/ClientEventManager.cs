using System;
using System.Collections.Generic;
using System.Reflection;

using CubivoxCore;
using CubivoxCore.Attributes;
using CubivoxCore.Events;

using CubivoxClient.Utils;

namespace CubivoxClient.Events
{
    public class ClientEventManager : EventManager
    {
        private Dictionary<Type, List<Action<Event>>> events;

        public ClientEventManager()
        {
            events = new Dictionary<Type, List<Action<Event>>>();
        }

        public void RegisterEvent<T>(Action<T> evt) where T : Event
        {
            Type parameterType = evt.GetMethodInfo().GetParameters()[0].ParameterType;

            if (evt.Method.GetCustomAttributes(typeof(ClientOnly), true).Length > 0)
            {
                if (Cubivox.GetEnvironment() != EnvType.CLIENT)
                {
                    return;
                }
            }
            else if (evt.Method.GetCustomAttributes(typeof(ServerOnly), true).Length > 0)
            {
                if (Cubivox.GetEnvironment() != EnvType.SERVER)
                {
                    return;
                }
            }

            if (!events.ContainsKey(parameterType))
            {
                events[parameterType] = new List<Action<Event>>() { new Action<Event>(o => evt((T)o)) };
            }
            else
            {
                events[parameterType].Add(new Action<Event>(o => evt((T)o)));
            }
        }

        public bool TriggerEvent(Event evt)
        {
            if (!events.ContainsKey(evt.GetType()))
            {
                return true;
            }

            if (evt is CancellableEvent cancellableEvent)
            {
                var delegates = events[cancellableEvent.GetType()];
                foreach (var delegator in delegates)
                {
                    Isolator.Isolate(() => delegator.Invoke(cancellableEvent));

                    if (cancellableEvent.IsCanceled())
                    {
                        return false;
                    }
                }
            }
            else
            {
                var delegates = events[evt.GetType()];
                foreach (var delegator in delegates)
                {
                    Isolator.Isolate(() => delegator.Invoke(evt));
                }
            }

            return true;
        }
    }
}