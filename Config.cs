using System;

namespace SharpIRC {

    public class IRCConfig {

        public string nick { get; private set; }
        public string server { get; private set; }
        public int port { get; private set; }

        private string _username;
        public string username { 
            get { return _username == null ? nick : _username; }
            set { if (_username == null) this._username = value; }
        }

        private string _realName;
        public string realName {
            get { return (_realName == null ? nick : _realName);}
            set { if (_realName == null) _realName = value; }
        }

        private string _serverPassword;
        public string serverPassword {
            get { return _serverPassword; }
            set { if (_serverPassword == null) _serverPassword = value; }
        }

        private string _nickServPassword;
        public string nickServPassword {
            get { return _nickServPassword; }
            set { if (_nickServPassword == null) _nickServPassword = value; }
        }

        private string[] _channels;
        public string[] channels {
            get { return _channels == null ? new string[0] : _channels; }
            set { if (_channels == null) _channels = value; }
        }

        public IRCConfig(string nick, string server, uint port) {
            this.nick = nick;
            this.server = server;
            this.port = (int) port;
        }

    }

}

