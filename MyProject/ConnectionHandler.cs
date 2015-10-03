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
        public Socket handler;
        public UdpClient udp_channel;
        public Socket clipbd_channel { get; set; }
        public Form form;

        public abstract bool Open();
        public abstract bool Close();

        public abstract void RetryPrimaryConnection();
        public abstract void RetryClipboardConnection();

        public void SendClipboard(byte[] bytes)
        {
            try
            {
                Functions.SendData(this.clipbd_channel, bytes, 0, bytes.Length);
            }
            catch (SocketException)
            {
                
                throw;
            }
        }

        public byte[] ReceiveClipboard(int size)
        {
            byte[] bytes = null;

            try
            {
                bytes = Functions.ReceiveData(this.clipbd_channel, size);
            }
            catch (SocketException)
            {
                throw;
            }

            return bytes;
        }


        public ConnectionHandler(Form form, IPAddress ipAddress, Int32 port, string password)
        {
            this.form = form;
            this.ipAddress = ipAddress;
            this.port = port;
            this.password = password;
        }
    }
}
