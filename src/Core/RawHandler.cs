using System;
using System.Threading;
using System.IO;
using SharpIRC.Core.Event;

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
                            if (bot.channels.Contains(data[2]))
                                bot.channels.Remove(data[2]);
                            bot.channels.Add(data[2]);
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
                        case "PRIVMSG":
                            string user = data[0].Substring(1, data[0].IndexOf('!') - 1);
                            string message = "";
                            for (int i = 3; i < data.Length; i++)
                                message += (message.Length == 0 ? "" : " ") + data[i];
                            if (data[2].StartsWith("#"))
                                bot.eventBus.post<MessageEvent>(new MessageEvent(message.Substring(1), data[2], user));
                            else {
                                bot.eventBus.post<PrivateMessageEvent>(new PrivateMessageEvent(message.Substring(1), user));
                            }
                            break;
                    }

                }
            } catch (IOException) {} // catches a ThreadAbortException wrapped in an IOException
        }

    }
}

