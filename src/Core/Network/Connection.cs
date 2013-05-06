using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using SharpIRC;

namespace SharpIRC.Core.Network {
    public class Connection {

        private TcpClient IRCConnection;
        private NetworkStream ns;
        private StreamReader sr;
        private StreamWriter sw;

        private readonly IRCConfig config;

        public bool connected { get; private set; }

        public Connection(IRCConfig config) {
            this.config = config;
        }

        ~Connection() {
            close();
        }

        public void connect() {
            close();

            IRCConnection = new TcpClient(config.server, config.port);
            ns = IRCConnection.GetStream();

            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
            connected = true;
        }

        public string read() {
            if (sr == null)
                return "";
            return sr.ReadLine();
        }

        public void sendData(string cmd) {
            if (sw == null)
                return;
            sw.Write(cmd);
            sw.Flush();
        }

        public void close() {
            try {
                if (sw != null)
                    sw.Close();
                if (sr != null)
                    sr.Close();
                if (ns != null)
                    ns.Close();
                if (IRCConnection != null)
                    IRCConnection.Close();
            } catch (Exception e) {
                Console.WriteLine("Error closing sockets: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }

            sr = null;
            sw = null;
            ns = null;
            IRCConnection = null;
            connected = false;
        }
    }

}

