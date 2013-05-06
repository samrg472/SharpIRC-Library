using System;
using System.Threading;
using System.IO;
using SharpIRC.Core.Event;
using SharpIRC.Core.Util;

namespace SharpIRC {
    public class RawHandler {

        private readonly IRCBot bot;
        private Thread t;

        public RawHandler(IRCBot bot) {
            this.bot = bot;
        }

        ~RawHandler() {
            run(false);
        }

        public void run(bool run) {
            try {
                if (run) {
                    t = new Thread(new ThreadStart(handler));
                    t.Start();
                } else {
                    t.Abort();
                }
            } catch {
            }
        }

        private void handler() {
            string line;
            string[] data;
            bool postMOTD = false; // Handle cases after first motd to ever be displayed while connected
            try {
                while (bot.connected) {
                    line = bot.connection.read();
                    if (line == null) continue;

                    data = line.Split(' ');

                    if (data[0] == "PING") {
                        bot.sendRaw("PONG " + data[1]);
                        continue;
                    }

                    bot.eventBus.post<RawEvent>(new RawEvent(line));
                    switch (data[1]) {
                        case "433": // ERR_NICKNAMEINUSE
                            Console.WriteLine("Nickname in use, disconnecting...");
                            bot.disconnect();
                            break;
                        case "470": // Handle forwarding
                            if (bot.cached.ContainsKey(data[3]))
                                bot.cached.Remove(data[3]);
                            bot.cached.Add(data[4], new Wrapper(new Channel(bot, data[4])));
                            bot.eventBus.post<ChannelForwardEvent>(new ChannelForwardEvent(data[3], bot.cached[data[4]].channel));
                            break;
                        case "376": // RPL_ENDOFMOTD
                            if (!postMOTD) {
                                postMOTD = true;
                                if (bot.config.nickServPassword != null)
                                    bot.sendRaw("PRIVMSG NickServ :identify " + bot.config.nickServPassword);
                                foreach (string channel in bot.config.channels)
                                    bot.joinChannel(channel);
                            }
                            break;
                        case "353": // NAMES
                            for (int i = 5; i < data.Length; i++)
                                handleUser(data[4], data[i]);
                            break;
                        case "JOIN":
                            string user = data[0].Substring(1, data[0].IndexOf('!') - 1);
                            string chan = data[2];
                            handleUser(chan, user);
                            break;
                        case "PART":
                            user = data[0].Substring(1, data[0].IndexOf('!') - 1);
                            chan = data[2];
                            bot.cached[chan].channel.manager.removeUserFromAll(user);
                            break;
                        case "PRIVMSG": // TODO: Handle CTCP
                            user = data[0].Substring(1, data[0].IndexOf('!') - 1);
                            string message = "";
                            for (int i = 3; i < data.Length; i++)
                                message += (message.Length == 0 ? "" : " ") + data[i];
                            if (data[2].StartsWith("#"))
                                bot.eventBus.post<MessageEvent>(new MessageEvent(message.Substring(1), bot.cached[data[2]].channel, new User(bot, user)));
                            else {
                                bot.eventBus.post<PrivateMessageEvent>(new PrivateMessageEvent(message.Substring(1), new User(bot, user)));
                            }
                            break;
                    }

                }
            } catch (IOException) {} // catches a ThreadAbortException wrapped in an IOException
        }

        private void handleUser(string chan, string user) {
            user = user.StartsWith(":") ? user.Substring(1) : user;
            switch (user.Substring(0, 1)) {
                case "@": // op
                    bot.cached[chan].channel.manager.addOp(user.Substring(1));
                    break;
                case "+": // voice
                    bot.cached[chan].channel.manager.addVoice(user.Substring(1));
                    break;
                default: // normal
                    bot.cached[chan].channel.manager.addUser(user);
                    break;
            }
        }

    }
}

