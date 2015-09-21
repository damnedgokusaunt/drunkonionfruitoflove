using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;

namespace MyProject
{
    public abstract class ConnectionHandler
    {
        protected IPAddress ipAddress;
        protected Int32 port;
        protected string password;
        protected Socket handler;
        protected UdpClient udp_channel;
        public Socket clipbd_channel { get; set; }
        public Form form;

        public abstract bool Open();
        public abstract bool Close();

        public ConnectionHandler(Form form, IPAddress ipAddress, Int32 port, string password)
        {
            this.form = form;
            this.ipAddress = ipAddress;
            this.port = port;
            this.password = password;
        }
    }
}
