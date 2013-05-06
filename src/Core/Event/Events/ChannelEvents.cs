using System;
using SharpIRC.Core.Util;

namespace SharpIRC.Core.Event {

    public class JoinChannelEvent : Event {

        public readonly Channel channel;
        public readonly User user;
        public readonly bool isItself;

        public JoinChannelEvent(Channel channel, User user, bool isItself) {
            this.channel = channel;
            this.user = user;
            this.isItself = isItself;
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

