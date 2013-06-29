using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SharpIRC.Core.Event {

    public class EventBus {

        private List<Delegate> registry = new List<Delegate>();

        public void register<T>(Action<T> method) where T : Event {
            if (!registry.Contains(method))
                registry.Add(method);
        }

        public void unregister<T>(Action<T> method) where T : Event {
            if (registry.Contains(method))
                registry.Remove(method);
        }

        public void post<T>(T e) where T : Event {
            post<T>(e, false);
        }

        public void post<T>(T e, bool block) where T : Event {
            ThreadStart start = () => {
                for (int i = 0; i < registry.Count; i++) {
                    Action<T> m = registry[i] as Action<T>;
                    if (m != null)
                        m.DynamicInvoke(e);
                }
            };
            Thread t = new Thread(start);
            t.IsBackground = true;
            t.Start();
            if (block)
                t.Join(100 * registry.Count);
        }
    }

}

