using System;
using SharpIRC.Core.Util;

namespace SharpIRC.Core.Event {

    public class MessageEvent : Event {

        public readonly string message;
        public readonly Channel channel;
        public readonly User user;

        public MessageEvent(string message, Channel channel, User user) {
            this.message = message;
            this.channel = channel;
            this.user = user;
        }

    }

    public class PrivateMessageEvent : Event {

        public readonly string message;
        public readonly User user;

        public PrivateMessageEvent(string message, User user) {
            this.message = message;
            this.user = user;
        }

    }
}

