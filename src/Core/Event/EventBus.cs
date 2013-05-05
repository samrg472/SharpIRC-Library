using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SharpIRC.Core.Event {

    public class EventBus {

        private ArrayList list = new ArrayList();

        public void register<T>(Action<T> method) {
            if (!list.Contains(method))
                list.Add(method);
        }

        public void post<T>(Event e) {
            post<T>(e, false);
        }

        public void post<T>(Event e, bool block) {
            ThreadStart start = () => {
                for (int i = 0; i < list.Count; i++) {
                    Action<T> m = list[i] as Action<T>;
                    if (m != null)
                        m.DynamicInvoke(e);
                }
            };
            Thread t = new Thread(start);
            t.IsBackground = true;
            t.Start();
            if (block)
                t.Join(100 * list.Count);
        }
    }

}

