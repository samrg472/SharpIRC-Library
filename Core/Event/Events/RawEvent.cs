using System;

namespace SharpIRC.Core.Event {

    public class RawEvent : Event {

        public readonly string message;

        public RawEvent(string message) {
            this.message = message;
        }

    }

}

