using System;

namespace SharpIRC.Core.Util {
    public class Wrapper {

        public readonly Channel channel;
        public readonly User user;

        public bool isChannelWrapper {
            get { return channel != null; }
            private set {}
        }

        public bool isUserWrapper {
            get { return user != null; }
            private set {}
        }

        public Wrapper(Channel channel) {
            this.channel = channel;
            this.user = null;
        }

        public Wrapper(User user) {
            this.channel = null;
            this.user = user;
        }

    }
}

