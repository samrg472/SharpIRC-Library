using System;
using SharpIRC.Core.Util;

namespace SharpIRC.Core.Event {

    public class JoinChannelEvent : Event {

        public readonly Channel channel;

        public JoinChannelEvent(Channel channel) {
            this.channel = channel;
        }
    }

    public class ChannelForwardEvent : Event {

        public readonly string previousChannel;
        public readonly Channel channel;

        public ChannelForwardEvent(string previousChannel, Channel channel) {
            this.previousChannel = previousChannel;
            this.channel = channel;
        }

    }

}

