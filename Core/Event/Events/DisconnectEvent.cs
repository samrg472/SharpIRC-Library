using System;

namespace SharpIRC.Core.Event {

    public class DisconnectEvent : Event {

        public readonly string network;
        public readonly string[] channels;

        public DisconnectEvent(string network, string[] channels) {
            this.network = network;
            this.channels = channels;
        }
    }

}

