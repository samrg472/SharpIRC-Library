using System;

namespace SharpIRC.Core.Event {

    public class MessageEvent : Event {

        public readonly string message;
        public readonly string channel;
        public readonly string user;

        public MessageEvent(string message, string channel, string user) {
            this.message = message;
            this.channel = channel;
            this.user = user;
        }

    }

    public class PrivateMessageEvent : Event {

        public readonly string message;
        public readonly string user;

        public PrivateMessageEvent(string message, string user) {
            this.message = message;
            this.user = user;
        }

    }
}

