using System;
using System.Collections.Generic;

namespace SharpIRC.Core.Util {

    public class Channel {

        public readonly IRCBot bot;
        public readonly string channel;

        private readonly List<string> ops = new List<string>();
        private readonly List<string> voices = new List<string>();
        private readonly List<string> users = new List<string>(); // All regular users in channel

        public UserManagement manager;

        public Channel(IRCBot bot, string channel) {
            this.bot = bot;
            this.channel = channel;
            this.manager = new UserManagement(this);
        }

        public void sendMessage(string message) {
            bot.sendMessage(channel, message);
        }

        public void sendNotice(string message) {
            bot.sendNotice(channel, message);
        }

        public void part() {
            bot.partChannel(channel);
        }

        public void part(string reason) {
            bot.partChannel(channel, reason);
        }

        public string[] getOps() {
            return ops.ToArray();
        }

        public string[] getVoices() {
            return voices.ToArray();
        }

        public string[] getUsers() {
            return users.ToArray();
        }

        public string[] getAllUsers() {
            List<string> newArray = new List<string>();
            newArray.AddRange(ops);
            newArray.AddRange(voices);
            newArray.AddRange(users);
            return newArray.ToArray();
        }

        public override string ToString() {
            return string.Format("[Channel(channel={0},ops={1},voices={2},regulars={3},totalUsers={4})]", 
                                 channel, ops.Count, voices.Count, users.Count, (ops.Count + voices.Count + users.Count));
        }

        public class UserManagement {

            private readonly Channel c;

            public UserManagement(Channel c) {
                this.c = c;
            }

            public void addOp(string user) {
                removeUserFromAll(user);
                c.ops.Add(user);
            }
            
            public void addVoice(string user) {
                removeUserFromAll(user);
                c.voices.Add(user);
            }
            
            public void addUser(string user) {
                removeUserFromAll(user);
                c.users.Add(user);
            }

            public void removeUserFromAll(string user) {
                if (c.ops.Contains(user))
                    c.ops.Remove(user);

                if (c.voices.Contains(user))
                    c.voices.Remove(user);

                if (c.users.Contains(user))
                    c.users.Remove(user);
            }

        }
    }

}

