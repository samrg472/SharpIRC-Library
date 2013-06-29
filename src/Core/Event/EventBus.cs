using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.ComponentModel;

namespace SharpIRC.Core.Event {

    public class EventBus {
        #region Channel events
        public event EventHandler<JoinChannelEvent> @JoinChannelEvent {
            add { _JoinChannelEvent += value; }
            remove { _JoinChannelEvent += value; }
        }
        private event EventHandler<JoinChannelEvent> _JoinChannelEvent;

        public event EventHandler<ChannelForwardEvent> @ChannelForwardEvent {
            add { _ChannelForwardEvent += value; }
            remove { _ChannelForwardEvent += value; }
        }
        private event EventHandler<ChannelForwardEvent> _ChannelForwardEvent;
        #endregion

        #region Message events
        public event EventHandler<MessageEvent> @MessageEvent {
            add { _MessageEvent += value; }
            remove { _MessageEvent -= value; }
        }
        private event EventHandler<MessageEvent> _MessageEvent;

        public event EventHandler<PrivateMessageEvent> @PrivateMessageEvent {
            add { _PrivateMessageEvent += value; }
            remove { _PrivateMessageEvent -= value; }
        }
        private event EventHandler<PrivateMessageEvent> _PrivateMessageEvent;
        #endregion

        public event EventHandler<DisconnectEvent> @DisconnectEvent {
            add { _DisconnectEvent += value; }
            remove { _DisconnectEvent -= value; }
        }
        private event EventHandler<DisconnectEvent> _DisconnectEvent;

        public event EventHandler<RawEvent> @RawEvent {
            add { _RawEvent += value; }
            remove { _RawEvent -= value; }
        }
        private event EventHandler<RawEvent> _RawEvent;

        public void post<T>(T e) where T : Event {
            post<T>(e, false);
        }

        public void post<T>(T e, bool block) where T : Event {
            string fieldName = "_" + typeof(T).Name;
            FieldInfo fi = typeof(EventBus).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null)
                throw new NullReferenceException("FieldInfo is null; attempted field name " + fieldName);

            EventHandler<T> eh = fi.GetValue(this) as EventHandler<T>;
            if (eh != null) {
                Thread t = new Thread(() => { eh(new object(), e); });
                t.IsBackground = true;
                t.Start();
                if (block)
                    t.Join();
            }
        }
    }
}

