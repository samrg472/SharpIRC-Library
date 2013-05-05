using System;
using System.Threading;
using System.Collections;
using SharpIRC.Core.Network;
using SharpIRC.Core.Event;

namespace SharpIRC {

    public class IRCBot {

        private readonly RawHandler rawHandler;

        internal readonly Connection connection;
        internal ArrayList channels = new ArrayList();

        public readonly IRCConfig config;
        public readonly EventBus eventBus = new EventBus();

        /// <summary>
        /// Gets a value indicating whether this <see cref="IRC_Bot.IRCBot"/> is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool connected {
            get { return connection.connected; }
            private set {}
        }

        /// <summary>
        /// Initializes a new bot used to connect to servers
        /// </summary>
        /// <param name='config'>
        /// IRC configuration object
        /// </param>
        /// <param name='connect'>
        /// Whether to auto connect to the IRC server or not
        /// </param>
        public IRCBot(IRCConfig config, bool connect) {
            this.config = config;
            this.rawHandler = new RawHandler(this);
            this.connection = new Connection(config);
            if (connect)
                this.connect();
        }

        ~IRCBot() {
            disconnect();
        }

        /// <summary>
        /// Sends a raw IRC message
        /// </summary>
        /// <param name='message'>
        /// Sends the specified raw message to the IRC server
        /// </param>
        public void sendRaw(string message) {
            if (connected)
                connection.sendData(message + "\r\n");
        }

        /// <summary>
        /// Joins the specified channel
        /// </summary>
        /// <param name='channel'>
        /// Channel name
        /// </param>
        public void joinChannel(string channel) {
            sendRaw("JOIN " + channel);
            if (!channels.Contains(channel))
                channels.Add(channel);
            eventBus.post<JoinChannelEvent>(new JoinChannelEvent(channel));
        }

        /// <summary>
        /// Parts the specified channel with no reason given
        /// </summary>
        /// <param name='channel'>
        /// Channel name
        /// </param>
        public void partChannel(string channel) {
            partChannel(channel, null);
        }

        /// <summary>
        /// Parts the specified channel with a reason
        /// </summary>
        /// <param name='channel'>
        /// Channel name
        /// </param>
        /// <param name='reason'>
        /// Reason message
        /// </param>
        public void partChannel(string channel, string reason) {
            if (channels.Contains(channel))
                channels.Remove(channel);
            if (reason == null) {
                sendRaw("PART " + channel);
                return;
            }
            sendRaw("PART " + channel + " :" + reason);
        }

        public void sendMessage(string target, string message) {
            sendRaw("PRIVMSG " + target + " :" + message);
        }

        /// <summary>
        /// Connects to the IRC server if not already
        /// </summary>
        public void connect() {
            if (connected)
                return;
            connection.connect();
            rawHandler.run(true);
            sendRaw("NICK " + config.nick);
            sendRaw("USER " + config.username + " 8 * :" + config.realName);
        }

        /// <summary>
        /// Disconnects from the IRC server if not already
        /// </summary>
        public void disconnect() {
            disconnect(null);
        }

        /// <summary>
        /// Disconnects from the IRC server if not already with the specified quit message
        /// </summary>
        /// <param name='message'>
        /// Quit message
        /// </param>
        public void disconnect(string message) {
            eventBus.post<DisconnectEvent>(new DisconnectEvent(config.server, (string[])channels.ToArray(typeof(string))), true);
            if (!connected)
                return;
            sendRaw("QUIT" + (message != null ? (" :" + message) : ""));
            rawHandler.run(false);
            connection.close();
        }

    }
}

