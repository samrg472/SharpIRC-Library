using System;

namespace SharpIRC.Core.Event {

    public class JoinChannelEvent : Event {

        public readonly string channel;

        public JoinChannelEvent(string channel) {
            this.channel = channel;
        }
    }

}

