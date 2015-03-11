using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Collections;

using System.Windows.Input;
using System.Runtime.InteropServices;

using System.Threading;

namespace ProgettoPdS
{
    public class SynchronousSocketListener
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        // Incoming data from the client.
        private string data = null;
        private IPAddress ipAddress;
        private int port;
        private string expectedConnectionRequest;

        private Socket handler;
        
        //setters
        public void setIpAddress(IPAddress ipAddress) { this.ipAddress = ipAddress; }
        public void setPort(int port) { this.port = port; }
        public void setExpectedConnectionRequest(string expectedConnectionRequest) { this.expectedConnectionRequest = expectedConnectionRequest; }

        public void StartListening() 
        {     
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(LocalAddressResolve());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp );

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            /*
            try
            {*/
            Console.WriteLine("IP: " + ipAddress + "\nPort: " + port);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                MessageBox.Show("Server avviato con successo.");

                while (true)
                {
                    // Start listening for connections.
                    handler = listener.Accept();

                    int bytesRec = 0;
                    string resp = string.Empty;

                    while (true)
                    {
                        //thread mouse handler e keyboard handler
                        Thread mh_t, kh_t;

                        // An incoming connection needs to be processe
                        data = null;

                        while (true)
                        {
                            bytes = new byte[1024];
                            bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            if (data.IndexOf("<EOF>") > -1)
                            {
                                break;
                            }
                        }

                        if (data == expectedConnectionRequest)
                        {
                            MessageBox.Show("Richiesta di connessione accettata.");
                            resp = "+OK<EOF>";

                            // Echo the data back to the client.
                            byte[] msg = Encoding.ASCII.GetBytes(resp);

                            handler.Send(msg);
                        }

                        else if (data == MyProtocol.CONTROL_REQUEST)
                        {
                            MessageBox.Show("Richiesta di controllo accettata.");

                            handler.Send(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK));

                            byte[] point = new byte[sizeof(Int32) * 2];
                            byte[] buffer = new byte["QUIT<EOF>".Length];

                            // Riceve risoluzione dello schermo
                            handler.Receive(point);

                            // Istanzia gestore mouse
                            MouseHandler mh = new MouseHandler(
                                new UdpClient(port + 1),
                                handler.RemoteEndPoint as IPEndPoint,
                                BitConverter.ToInt32(point, 0),
                                BitConverter.ToInt32(point, sizeof(Int32)));

                            // Istanzia gestore tastiera
                            KeyboardHandler kh = new KeyboardHandler();

                            kh.setLocalEndPoint(new IPEndPoint(ipAddress, port + 2));

                            // Istanzia i relativi thread
                            mh_t = new Thread(mh.Run);
                            kh_t = new Thread(kh.Run);
                            
                            // Lancia i thread
                            mh_t.Start();
                            kh_t.Start();
                        }

                        else
                        {
                            MessageBox.Show("Richiesta non riconosciuta.");

                            handler.Send(Encoding.ASCII.GetBytes("-ERR<EOF>"));
                        }


                    }

                }
            }
        
        }
        }
