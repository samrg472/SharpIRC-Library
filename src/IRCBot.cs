using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using SharpIRC.Core.Network;
using SharpIRC.Core.Event;
using SharpIRC.Core.Util;

namespace SharpIRC {

    public class IRCBot {

        private readonly RawHandler rawHandler;

        internal readonly Connection connection;
        internal Dictionary<string, Wrapper> cached = new Dictionary<string, Wrapper>(); // TODO: Cache the user

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
            if (cached.ContainsKey(channel))
                return;
            cached.Add(channel, new Wrapper(new Channel(this, channel)));
            sendRaw("JOIN " + channel);
            //eventBus.post<JoinChannelEvent>(new JoinChannelEvent(cached[channel].channel, new User(this, config.nick), true));
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
            if (!cached.ContainsKey(channel))
                return;
            cached.Remove(channel);
            if (reason == null) {
                sendRaw("PART " + channel);
                return;
            }
            sendRaw("PART " + channel + " :" + reason);
        }

        public void sendNotice(string target, string message) {
            sendRaw("NOTICE " + target + " :" + message);
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
            ArrayList channels = new ArrayList();
            foreach (Wrapper s in cached.Values) {
                if (s.isChannelWrapper)
                    channels.Add(s.channel);
            }
            eventBus.post<DisconnectEvent>(new DisconnectEvent(config.server, (Channel[])channels.ToArray(typeof(Channel))), true);
            if (!connected)
                return;
            sendRaw("QUIT" + (message != null ? (" :" + message) : ""));
            rawHandler.run(false);
            connection.close();
        }

    }
}

