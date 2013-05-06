using System;
using System.Collections;

namespace SharpIRC.Core.Util {

    public class Channel {

        public readonly IRCBot bot;
        public readonly string channel;

        private readonly ArrayList ops = new ArrayList();
        private readonly ArrayList voices = new ArrayList();
        private readonly ArrayList users = new ArrayList(); // All users in channel

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
            return (string[]) ops.ToArray(typeof(string));
        }

        public string[] getVoices() {
            return (string[]) voices.ToArray(typeof(string));
        }

        public string[] getUsers() {
            return (string[]) users.ToArray(typeof(string));
        }

        public string[] getAllUsers() {
            ArrayList newArray = new ArrayList();
            newArray.AddRange(ops);
            newArray.AddRange(voices);
            newArray.AddRange(users);
            return (string[]) newArray.ToArray(typeof(string));
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
                if (!c.ops.Contains(user))
                    c.ops.Add(user);
            }
            
            public void removeOp(string user) {
                if (c.ops.Contains(user))
                    c.ops.Remove(user);
            }
            
            public void addVoice(string user) {
                if (!c.voices.Contains(user))
                    c.voices.Add(user);
            }
            
            public void removeVoice(string user) {
                if (c.voices.Contains(user))
                    c.voices.Remove(user);
            }
            
            public void addUser(string user) {
                if (!c.users.Contains(user))
                    c.users.Add(user);
            }
            
            public void removeUser(string user) {
                if (c.users.Contains(user))
                    c.users.Remove(user);
            }

            public void removeUserFromAll(string user) {
                removeOp(user);
                removeVoice(user);
                removeUser(user);
            }
        }
    }

}

