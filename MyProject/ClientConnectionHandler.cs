using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//TODO: sistemare id (per ora è messo a 0) nelle entrystatusupdate

namespace MyProject
{
    public class ClientConnectionHandler
    {
        internal delegate void UpdateProgressDelegate(int ProgressPercentage);
        internal event UpdateProgressDelegate UpdateProgress;

        private IPAddress ipAddress;
        private Int32 port;
        private string password;
        private Socket handler, clipboard_channel;

        private UdpClient udp_channel;

        public ClientConnectionHandler(IPAddress ipAddress, Int32 port, string password)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.password = password;
        }

        // Questo metodo viene invocato per connettere l'oggetto con un server
        public bool Open()
        {
 
            int len;
            string msg;
            
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to  remote device.
            try
            {
                
                // Create a TCP/IP  socket.
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                handler.Connect(remoteEP);

                
                msg = MyProtocol.message(MyProtocol.CONNECTION + this.password);
                bytes = Encoding.ASCII.GetBytes(msg);

                //MessageBox.Show("Devo inviare: " + msg);
                // Send connection request
                Functions.SendData(ref handler, bytes, 0, bytes.Length);
                
                Console.WriteLine("Client sent connection request + encrypted password.");
                
                // Receive the response from the remote device.
                len = MyProtocol.message(MyProtocol.POSITIVE_ACK).Length;
                bytes = Functions.ReceiveData(ref handler, len);
                msg = Encoding.ASCII.GetString(bytes, 0, len);

                if (msg == MyProtocol.message(MyProtocol.POSITIVE_ACK))
                {
                    Int32 width = Screen.PrimaryScreen.Bounds.Width;
                    Int32 height = Screen.PrimaryScreen.Bounds.Height;

                    //MessageBox.Show("Coordinate: " + width + "," + height);
                    byte[] resolution = new byte[sizeof(Int32) * 2];

                    // Concatena due byte[] (uno per la X, l'altro per la Y)
                    System.Buffer.BlockCopy(BitConverter.GetBytes(width), 0, resolution, 0, sizeof(Int32));
                    System.Buffer.BlockCopy(BitConverter.GetBytes(height), 0, resolution, sizeof(Int32), sizeof(Int32));

                    Functions.SendData(ref handler, resolution, 0, sizeof(Int32) * 2);

                    // Receive the UDP port of the server
                    bytes = Functions.ReceiveData(ref handler, sizeof(Int32));
                    Int32 udpRemotePort = BitConverter.ToInt32(bytes, 0);
                    IPEndPoint udpLocalEP = new IPEndPoint(IPAddress.Any, port+1);
                    Int32 udpLocalPort = udpLocalEP.Port;
                    udp_channel = new UdpClient(udpLocalEP);
                    udp_channel.Connect(new IPEndPoint(remoteEP.Address, udpRemotePort));

                    // Send the UDP port to the server
                    bytes = BitConverter.GetBytes(udpLocalPort);
                    Functions.SendData(ref handler, bytes, 0, sizeof(Int32));

                    return true;
                }          
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return false;
        }

        public bool Close()
        {
            int len;
            byte[] bytes;
            string msg;

            bytes = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.QUIT));
            // Client sends to server a quit request
            Functions.SendData(ref handler, bytes, 0, bytes.Length);

            len = MyProtocol.message(MyProtocol.POSITIVE_ACK).Length;
            // Client receive a response from server
            bytes = Functions.ReceiveData(ref handler, len);
            msg = Encoding.ASCII.GetString(bytes);

            if (msg == MyProtocol.message(MyProtocol.POSITIVE_ACK))
            {
                handler.Close();
                udp_channel.Close();

                MessageBox.Show("Il client ha chiuso la connessione.");

                return true;
            }

            return false;
        }

        public void Send(byte[] bytes)
        {
            Functions.SendData(ref handler, bytes, 0, bytes.Length);
        }

        public byte[] Receive(int size)
        {
            return Functions.ReceiveData(ref handler, size);
        }

        public void SendUDP(byte[] bytes)
        {
            udp_channel.Send(bytes, bytes.Length);
        }

    }
}