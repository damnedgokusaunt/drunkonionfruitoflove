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
using System.Diagnostics;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace MyProject
{
   
    
    public class ServerConnectionHandler : ConnectionHandler
    {
       
        private IPEndPoint udp_remote_endpoint;
        private Int32 widthRatio, heightRatio;
        private byte[] endpoint_resolution;
        private bool connected;
        //Delegate
        public delegate void TargetHandlerShow();
        public delegate void TargetHandlerHide();
        public TargetHandlerShow show;
        public TargetHandlerHide hide;
        public delegate void NotifyDelegate(int a, string b,string c, ToolTipIcon cletta);
        public NotifyDelegate notify;
        //delegato per la slee_threads
        public delegate void AwakeThreadsDelegate();
        public AwakeThreadsDelegate wakeup;

        private IPEndPoint clipboardLocalEndPoint;
        public bool Connected
        {
            get 
            {
                return connected; 
            }
            set
            { 
                connected = value; 
            }
        }

        public ServerConnectionHandler(Form form, IPAddress ipAddress, int port, string password) : base(form, ipAddress, port, password)
        {
            this.connected = false;
            TargetForm frm = new TargetForm();
      
        }

        public override bool Open()
        {
            
            string msg;
            byte[] bytes;

            // Set local endpoint and create a TCP/IP socket.
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
           
            listener.Bind(localEndPoint);
            listener.Listen(10);

            handler = listener.Accept();
            listener.Close();

            Functions.SetKeepAlive(handler, MyProtocol.KEEPALIVE_TIMER, MyProtocol.KEEPALIVE_INTERVAL);
         
            msg = MyProtocol.message(MyProtocol.CONNECTION + this.password);

            bytes = Functions.ReceiveData(handler, msg.Length);

            string received_request = Encoding.ASCII.GetString(bytes);

            if (received_request == msg)
            {
                Console.WriteLine("Richiesta di connessione accettata.");

                msg = MyProtocol.message(MyProtocol.POSITIVE_ACK);
                bytes = Encoding.ASCII.GetBytes(msg);
                Functions.SendData(handler, bytes, 0, bytes.Length);

                // Receive display resolution
                endpoint_resolution = Functions.ReceiveData(handler, sizeof(Int32) * 2);
                widthRatio = Screen.PrimaryScreen.Bounds.Width / BitConverter.ToInt32(endpoint_resolution, 0);
                heightRatio = Screen.PrimaryScreen.Bounds.Height / BitConverter.ToInt32(endpoint_resolution, sizeof(Int32));
                this.notify(20000, "Info", "Connesso!", ToolTipIcon.Info);
               
                // Create UDP channel
                Int32 udpPort = Functions.FindFreePort();
                IPEndPoint udpLocalEndPoint = new IPEndPoint(ipAddress, udpPort);
                udp_channel = new UdpClient(udpLocalEndPoint);

                // Create TCP channel for clipboard operations (import/export)
                Int32 clipboardPort = Functions.FindFreePort();
                clipboardLocalEndPoint = new IPEndPoint(ipAddress, clipboardPort);
                Socket clipboard_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clipboard_listener.Bind(clipboardLocalEndPoint);

                //Notify the UDP port to the client
                bytes = BitConverter.GetBytes(udpPort);
                Functions.SendData(handler, bytes, 0, bytes.Length);

                // Receive the UDP port from the server
                bytes = Functions.ReceiveData(handler, sizeof(Int32));
                udp_remote_endpoint = handler.RemoteEndPoint as IPEndPoint;

                //Notify the TCP port for clipboard to the client
                bytes = BitConverter.GetBytes(clipboardPort);
                Functions.SendData(handler, bytes, 0, bytes.Length);

                clipboard_listener.Listen(10);

                this.clipbd_channel = clipboard_listener.Accept();
                clipboard_listener.Close();

                this.connected = true;

                return true;
            }
            else
            {
                this.notify(20000, "Info", "Richiesta non riconosciuta!", ToolTipIcon.Info);          
                msg = MyProtocol.message(MyProtocol.NEGATIVE_ACK);
                bytes = Encoding.ASCII.GetBytes(msg);
                Functions.SendData(handler, bytes, 0, bytes.Length);
                return false;
            }
        }

        public override bool Close()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.POSITIVE_ACK));
            // Send ack
            Functions.SendData(handler, bytes, 0, bytes.Length);
            connected = false;
            handler.Close();
            udp_channel.Close();
            this.notify(20000, "Info", "Il server ha chiuso la connessione!", ToolTipIcon.Info);
          
            return true;
        }

        public void ListenUDPChannel()
        {
            int del = 0;
            string msg = string.Empty, cmd = string.Empty;
            byte[] bytes = new byte[sizeof(Int32) * 2];


            while (connected)
            {
                try
                {

                    bytes = udp_channel.Receive(ref udp_remote_endpoint);
                    msg = Encoding.ASCII.GetString(bytes);

                    cmd = msg.Substring(0, 2);
                    switch (cmd)
                    {
                        case "DR":
                            Functions.doMouseRightClick();
                            break;

                        case "DL":
                            Functions.doMouseDragAndDrop();
                            break;

                        case "UL":
                            Functions.doMouseLeftClick();
                            break;

                        case "WH":
                            del = BitConverter.ToInt32(bytes, MyProtocol.MOUSE_WHEEL.Length);
                            Functions.doMouseWheel(del);
                            break;
                        default:
                            Functions.doMouseMove(bytes, widthRatio, heightRatio);
                            break;
                    }
                } 
                catch (SocketException)
                {
                    //addormenta i thread secondari in caso di eccezione sul socket
                    Thread.CurrentThread.Suspend();

                }
            }         
        }

        public void ListenClipboardChannel()
        {
            byte[] data;

            while (connected)
            {
                try
                {

                    Console.WriteLine(this.GetType().Name + " - in attesa di comandi dal client...");
                    string recvbuf = string.Empty;                   
                    recvbuf = Functions.ReceiveTillTerminator(clipbd_channel);
                    string command = recvbuf.Substring(0, 4);
                    Console.WriteLine(this.GetType().Name + " - ricevuto comando: " + recvbuf);

                    switch (command)
                    {
                        case MyProtocol.CLEAN:
                            // Now, I'm starting to clean my clipboard folder as you told me.
                            Functions.CleanClipboardDir(Path.GetFullPath(MyProtocol.CLIPBOARD_DIR));
                            break;
                        case MyProtocol.COPY:
                            string content;
                            int len = recvbuf.Length - MyProtocol.COPY.Length;
                            content = recvbuf.Substring(MyProtocol.COPY.Length, len);
                            //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                            Clipboard.SetData(DataFormats.Text, content);
                            this.notify(20000, "Aggiornamento clipboard", "Ricevuto un testo dalla clipboard del client!", ToolTipIcon.Info);
                            break;

                        case MyProtocol.FILE_SEND:
                            // Simulo un po' di ritardo di rete
                            Thread.Sleep(1000);
                            // Now send ack
                            Functions.SendData(clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length); 
                            Functions.handleFileDrop(clipbd_channel, null);
                            Functions.StartClipoardUpdaterThread();
                            this.notify(20000, "Aggiornamento clipboard", "Ricevuto un file dalla clipboard del client!", ToolTipIcon.Info);
                            break;

                        case MyProtocol.DIRE_SEND:
                            // Simulo un po' di ritardo di rete
                            Thread.Sleep(1000);

                            Functions.ReceiveDirectory(clipbd_channel);
                            Functions.StartClipoardUpdaterThread();
                            break;

                        case MyProtocol.IMG:
                            Functions.SendData(clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);
                            byte[] length = Functions.ReceiveData(clipbd_channel, sizeof(Int32));
                            Int32 length_int32 = BitConverter.ToInt32(length, 0);
                            Functions.SendData(clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);
                            byte[] imageSource = Functions.ReceiveData(clipbd_channel, length_int32);
                            Image image = Functions.ConvertByteArrayToBitmap(imageSource);
                            Clipboard.SetImage(image);
                            this.notify(20000, "Aggiornamento clipboard", "Ricevuta un'immagine dalla clipboard del client!", ToolTipIcon.Info);
                            break;

                        case MyProtocol.CLIPBOARD_IMPORT:
                            bool data_available = Functions.handleClipboardData(this);

                            if (!data_available)
                            {
                                data = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.NEGATIVE_ACK));

                                Functions.SendData(clipbd_channel, data, 0, data.Length);
                            }
                            break;

                        default:
                            this.notify(20000, "Aggiornamento clipboard", "Comando da tastiera non riconosciuto!", ToolTipIcon.Info);
                            break;
                    }
                }
                catch(SocketException) {

                    //Console.WriteLine("Ho sentitio l'eccezione e mi vo cucco!");
                    //clipboard_worker.Suspend();

                    Thread.CurrentThread.Suspend();
                    Console.WriteLine("Weeeeeeeeeeeee!!");
                }
            }
        }

        public override void RetryPrimaryConnection() {

            handler.Close();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            handler = listener.Accept();
            listener.Close();
            Functions.SetKeepAlive(handler, MyProtocol.KEEPALIVE_TIMER, MyProtocol.KEEPALIVE_INTERVAL);        
        }

    
        public override void RetryClipboardConnection(){

            clipbd_channel.Close();
            Socket clipbd_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clipbd_listener.Bind(clipboardLocalEndPoint);
            clipbd_listener.Listen(10);
            clipbd_channel = clipbd_listener.Accept();
            clipbd_listener.Close();
        }


        public void ListenTCPChannel()
        {
            byte[] bytes;
            string msg;
            Int32 commands_length = MyProtocol.STD_COMMAND_LENGTH;

            while (connected)
            {

                try
                {
                  
                    bytes = Functions.ReceiveData(handler, commands_length);
                    msg = Encoding.ASCII.GetString(bytes).Substring(0, MyProtocol.STD_COMMAND_LENGTH);

                    switch (msg)
                    {
                        case MyProtocol.QUIT:
                            Close();
                            break;

                        case MyProtocol.TARGET:
                            byte[] mess = System.Text.Encoding.Default.GetBytes(MyProtocol.POSITIVE_ACK);
                            Functions.SendData(handler, mess, 0, mess.Length);
                            this.show();
                            break;


                        case MyProtocol.PAUSE:
                            byte[] mess1 = System.Text.Encoding.Default.GetBytes(MyProtocol.POSITIVE_ACK);
                            Functions.SendData(handler, mess1, 0, mess1.Length);
                            this.hide();
                            break;

                        case MyProtocol.KEYDOWN:
                            bytes = Functions.ReceiveData(handler, 2);
                            Functions.KeyDown(bytes[0], bytes[1]);
                            break;

                        case MyProtocol.KEYUP:
                            bytes = Functions.ReceiveData(handler, 2);
                            Functions.KeyUp(bytes[0], bytes[1]);
                            break;
                    }
                }
                
                catch (SocketException)
                {

                    int idd = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Sono il thread principale:" + idd);                    
                    MessageBox.Show("Assicurati che la connessione venga ristabilita affinchè i servizi riprendano automaticamente");
                    RetryPrimaryConnection();
                    RetryClipboardConnection();
                    this.wakeup();
                  
                }
            }
        }
    }
}