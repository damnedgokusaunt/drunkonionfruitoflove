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

namespace ProgettoPdS
{
    public class SynchronousSocketClient
    {
        #region Attributes
        private IPAddress ipAddress;

        private int port, id;
        private string pwd;
        private LinkedList<Socket> SocketList;
       
        private Socket CurrentSocket;

        private Socket sender;

        private MainForm form;


        private string connectionRequest;
        private string controlRequest;
        private string quitRequest;

        #endregion
        
        #region Getters and setters
        //getters
        public int getId() { return id; }
        public IPAddress getIpAddress() { return ipAddress; }
        public int getPort() { return port; }
        public Socket getCurrentSocket() { return CurrentSocket; }
        //setters
        public void setIpAddress(IPAddress ipAddress) { this.ipAddress = ipAddress; }
        public void setPort(int port) { this.port = port; }
        public void setPwd(string pwd) { this.pwd = pwd; }
        public void setCurrentSocket(int id) { CurrentSocket = SocketList.ElementAt(id); }
        public void setForm(MainForm form) { this.form = form; }
        #endregion

        #region Constructor
        public SynchronousSocketClient(IPAddress ipAddress, int port, string pwd)
        {
            this.ipAddress = ipAddress;
            this.port = port;

            this.connectionRequest = MyProtocol.message(MyProtocol.CONNECTION, pwd);
            this.controlRequest = MyProtocol.message(MyProtocol.CONTROL, pwd);
            this.quitRequest = MyProtocol.message(MyProtocol.QUIT, pwd);

            SocketList = new LinkedList<Socket>();
        }
        #endregion

        #region Methods
        // Questo metodo viene invocato per connettere l'oggetto SynchronousSocketClient con un server
        public void initConnection()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {
                // Create a TCP/IP  socket.
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                sender = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);

                    // Send the data through the socket.
                    int bytesSent = sender.Send(Encoding.ASCII.GetBytes(connectionRequest));
                    Console.WriteLine("Client sent password.");

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Client: ricevuto " + Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    string resp = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    switch (resp)
                    {
                        case MyProtocol.POSITIVE_ACK + MyProtocol.END_OF_MESSAGE:
                            form.entryStatusUpdate("Disponibile.", 0);
                            //id = SocketList.Count();
                            SocketList.AddLast(sender);
                            break;

                        case MyProtocol.NEGATIVE_ACK + MyProtocol.END_OF_MESSAGE:
                            form.entryStatusUpdate("Non disponibile.", 0);
                            break;

                        default:
                            MessageBox.Show("STRONZO!");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
       
        public bool initControl()
        {
            byte[] bytes = new byte[1024];

            int bytesSent = sender.Send(Encoding.ASCII.GetBytes(controlRequest));
            Console.WriteLine("Client: inviato " + controlRequest);

            int bytesRec = sender.Receive(bytes);
            string resp = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            Console.WriteLine("Client: ricevuto " + resp);

            if (resp == MyProtocol.message(MyProtocol.POSITIVE_ACK))
            {
                Int32 width = Screen.PrimaryScreen.Bounds.Width;
                Int32 height = Screen.PrimaryScreen.Bounds.Height;

                byte[] resolution = new byte[sizeof(Int32) * 2];

                // Concatena due byte[] (uno per la X, l'altro per la Y)
                System.Buffer.BlockCopy(BitConverter.GetBytes(width), 0, resolution, 0, sizeof(Int32));
                System.Buffer.BlockCopy(BitConverter.GetBytes(height), 0, resolution, sizeof(Int32), sizeof(Int32));

                bytesSent = sender.Send(resolution);

                form.entryStatusUpdate("Attivo", 0);

                return true;     
            }

            return false;

        }
        

        #endregion
    }
}