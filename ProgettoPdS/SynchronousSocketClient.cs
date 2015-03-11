using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgettoPdS
{
    public class SynchronousSocketClient
    {

        private IPAddress ipAddress;

        private int port, id;
        private string pwd;
        private LinkedList<Socket> SocketList;
       
        private UdpClient udpChannel;
        private Socket tcpChannel;
        private Socket CurrentSocket;

        private Socket sender;

        private Form1 form;

        //setters
        public void setIpAddress(IPAddress ipAddress) { this.ipAddress = ipAddress; }
        public void setPort(int port) { this.port = port; }
        public void setPwd(string pwd) { this.pwd = pwd; }
        public void setCurrentSocket(int id) { CurrentSocket = SocketList.ElementAt(id); }
        public void setForm(Form1 form) { this.form = form; }
        //getters
        public int getId() { return id; }
        public Socket getCurrentSocket() { return CurrentSocket; }

        //costruttore
        public SynchronousSocketClient()
        {
            SocketList = new LinkedList<Socket>();
        }

        //metodi vari
        public void ShowControlForm()
        {
            ControlForm f = new ControlForm();
            udpChannel = new UdpClient();

            // Create a TCP/IP  socket.
            IPEndPoint tcpRemoteEndPoint = new IPEndPoint(ipAddress, port + 2);

            tcpChannel = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

            tcpChannel.Connect(ipAddress, port + 2);

            f.setUdpChannel(ref udpChannel);
            f.setTcpChannel(ref tcpChannel);

            f.CurrentSocket = CurrentSocket;

            f.setUdpRemoteEndPoint(new IPEndPoint(ipAddress, port+1));

            Application.Run(f);
        }

        public void StartClient()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {

                // Create a TCP/IP  socket.
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

                if (sender == null) MessageBox.Show("sender è null");

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(pwd + "<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);
                    Console.WriteLine("Client: inviato " + pwd + "<EOF>");

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Client: ricevuto " + Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    string mess1 = string.Empty, mess2 = string.Empty, resp = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    switch (resp)
                    {
                        case MyProtocol.POSITIVE_ACK:
                            mess1 = "Connessione accettata.";
                            mess2 = "Disponibile.";
                            id = SocketList.Count();
                            SocketList.AddLast(sender);
                            break;

                        case MyProtocol.NEGATIVE_ACK:
                            mess1 = "Connessione rifiutata.";
                            mess2 = "Non disponibile.";
                            break;

                        default:
                            MessageBox.Show("STRONZO!");
                            break;
                    }

                    MessageBox.Show(mess1);

                    form.StatusUpdate(mess2, id);

                    Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
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

        public void StartControl()
        {
            byte[] bytes = new byte[1024];

            int bytesSent = CurrentSocket.Send(Encoding.ASCII.GetBytes(MyProtocol.CONTROL_REQUEST));

            Console.WriteLine("Client: inviato " + MyProtocol.CONTROL_REQUEST);

            int bytesRec = CurrentSocket.Receive(bytes);
            
            string resp = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            Console.WriteLine("Client: ricevuto " + resp);

            if (resp == MyProtocol.POSITIVE_ACK)
            {
                Int32 width = Screen.PrimaryScreen.Bounds.Width;
                Int32 height = Screen.PrimaryScreen.Bounds.Height;

                byte[] resolution = new byte[sizeof(Int32) * 2];

                // Concatena due byte[] (uno per la X, l'altro per la Y)
                System.Buffer.BlockCopy(BitConverter.GetBytes(width), 0, resolution, 0, sizeof(Int32));
                System.Buffer.BlockCopy(BitConverter.GetBytes(height), 0, resolution, sizeof(Int32), sizeof(Int32));

                bytesSent = CurrentSocket.Send(resolution);

                form.StatusUpdate("Attivo", form.getCurrentSocketId());

                Thread t = new Thread(ShowControlForm);
                t.Start();
            }
                      
        }

    }
}