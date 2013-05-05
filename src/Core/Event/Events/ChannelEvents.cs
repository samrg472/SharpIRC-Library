using System;

namespace SharpIRC.Core.Event {

    public class JoinChannelEvent : Event {

        public readonly string channel;

        public JoinChannelEvent(string channel) {
            this.channel = channel;
        }
    }

    public class ChannelForwardEvent : Event {

        public readonly string previousChannel;
        public readonly string channel;

        public ChannelForwardEvent(string previousChannel, string channel) {
            this.previousChannel = previousChannel;
            this.channel = channel;
        }

    }

}

