using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SharpIRC.Core.Event {

    public class EventBus {

        private ArrayList registry = new ArrayList();

        public void register<T>(Action<T> method) {
            if (!registry.Contains(method))
                registry.Add(method);
        }

        public void unregister<T>(Action<T> method) {
            if (registry.Contains(method))
                registry.Remove(method);
        }

        public void post<T>(Event e) {
            post<T>(e, false);
        }

        public void post<T>(Event e, bool block) {
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

