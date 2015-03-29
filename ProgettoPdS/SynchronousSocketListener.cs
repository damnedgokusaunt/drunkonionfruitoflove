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
        private string expectedControlRequest;
        private string expectedQuitRequest;

        private Socket handler;
        
        //setters
        public void setIpAddress(IPAddress ipAddress) { this.ipAddress = ipAddress; }
        public void setPort(int port) { this.port = port; }
        public void setExpectedConnectionRequest(string expectedConnectionRequest) { this.expectedConnectionRequest = expectedConnectionRequest; }

        public SynchronousSocketListener(IPAddress ipAddress, int port, string pwd)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.expectedConnectionRequest = MyProtocol.message(MyProtocol.CONNECTION, pwd);
            this.expectedControlRequest = MyProtocol.message(MyProtocol.CONTROL, pwd);
            this.expectedQuitRequest = MyProtocol.message(MyProtocol.QUIT, pwd);
        }

        private void recvTillTheEnd(ref string recvbuf, string end)
        {
            int bytesRec;
            byte[] bytes = new byte[1024];

            do
            {
                bytesRec = handler.Receive(bytes);
                recvbuf += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            }
            while (recvbuf.IndexOf(end) == -1);
        }

        public void startListening() 
        {
            //thread mouse handler e keyboard handler
            Thread mh_t, kh_t;
            Thread ch_t;

            
            int bytesRec = 0;
            string resp = string.Empty;
            byte[] bytes = new Byte[1024];
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream, 
                ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);

            MessageBox.Show("Server avviato con successo.");

            while (true)
            {
                // Start listening for connections.
                handler = listener.Accept();

                while (true)
                {
                    // An incoming connection needs to be processed
                    data = null;

                    recvTillTheEnd(ref data, MyProtocol.END_OF_MESSAGE);

                    if (data == expectedConnectionRequest)
                    {
                        MessageBox.Show("Richiesta di connessione accettata.");
                        handler.Send(Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.POSITIVE_ACK)));
                    }

                    else if (data == expectedControlRequest)
                    {
                        MessageBox.Show("Richiesta di controllo accettata.");
                        handler.Send(Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.POSITIVE_ACK)));

                        byte[] point = new byte[sizeof(Int32) * 2];
                        byte[] buffer = new byte[expectedQuitRequest.Length];

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


                        // Istanzia gestore clipboard
                        ClipboardHandler ch= new ClipboardHandler();
                            ch.setLocalEndPoint(new IPEndPoint(ipAddress, port + 3));
                            ch_t = new Thread(ch.Run);
                            ch_t.SetApartmentState(ApartmentState.STA);
                  
                        // Istanzia i relativi thread
                        mh_t = new Thread(mh.Run);
                        kh_t = new Thread(kh.Run);
                        
                        // Lancia i thread
                        ch_t.Start();  
                        mh_t.Start();
                        kh_t.Start();
                        }

                    else
                    {
                        MessageBox.Show("Richiesta non riconosciuta.");
                        handler.Send(Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.NEGATIVE_ACK)));
                    }


                }

            }
            }
        
        }
        }
