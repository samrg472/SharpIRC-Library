using System;

namespace SharpIRC.Core.Util {
    public class User {
        
        public readonly IRCBot bot;
        public readonly string user;
        
        public User(IRCBot bot, string user) {
            this.bot = bot;
            this.user = user;
        }
        
        public void sendMessage(string message) {
            bot.sendMessage(user, message);
        }
        
        public void sendNotice(string message) {
            bot.sendNotice(user, message);
        }

        public override string ToString() {
            return string.Format("[User(user={0})]", user);
        }
    }
}

