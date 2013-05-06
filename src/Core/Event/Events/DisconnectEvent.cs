using System;
using SharpIRC.Core.Util;

namespace SharpIRC.Core.Event {

    public class DisconnectEvent : Event {

        public readonly string network;
        public readonly Channel[] channels;

        public DisconnectEvent(string network, Channel[] channels) {
            this.network = network;
            this.channels = channels;
        }
    }

}

