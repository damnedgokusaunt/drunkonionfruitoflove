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
        public bool Connected
        {
            get { return this.connected; }
            set { this.connected = value; }
        }

        // Thread signal.
        public ManualResetEvent mouse_event = new ManualResetEvent(false);
        public ManualResetEvent clipboard_event = new ManualResetEvent(false);
        public ManualResetEvent handler_event = new ManualResetEvent(false);

        // to using beginreceive
        private class StateObject<T>
        {
            public T workSocket;
            public int BufferSize;
            public byte[] buffer;
            public StringBuilder sb;

            public StateObject(int BufferSize)
            {
                this.workSocket = default(T);
                this.sb = new StringBuilder();
                this.BufferSize = BufferSize;
                this.buffer = new byte[BufferSize];
            }
        }

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

        public delegate void SuspendThreadsDelegate();
        public SuspendThreadsDelegate suspend;

        private IPEndPoint clipboardLocalEndPoint;

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

            Functions.SetKeepAlive(handler, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);
         
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
                Functions.SetKeepAlive(clipbd_channel, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);        
        

                Functions.SendClipboard = this.SendClipboard;
                Functions.ReceiveClipboard = this.ReceiveClipboard;

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
            //Thread.Sleep(1000);
            Functions.SendData(handler, bytes, 0, bytes.Length);
            
            return true;
        }

        public void CloseAllSockets()
        {
            if (udp_channel != null)
                udp_channel.Close();

            if (clipbd_channel != null)
                clipbd_channel.Close();

            if (handler != null)
                handler.Close();
        }

        public void ListenUDPChannel()
        {
            while (connected)
            {
                try
                {
                    mouse_event.Reset();

                    StateObject<UdpClient> state = new StateObject<UdpClient>(8);
                    state.workSocket = udp_channel;

                    udp_channel.BeginReceive(new AsyncCallback(processMouseData), state);

                    mouse_event.WaitOne();
                }
                catch (SocketException)
                {
                    //addormenta i thread secondari in caso di eccezione sul socket
                    Thread.CurrentThread.Suspend();
                }
            }
        }

        private void processMouseData(IAsyncResult ar)
        {
            int del = 0;
            string msg = string.Empty, cmd = string.Empty;
            StateObject<UdpClient> state = (StateObject<UdpClient>)ar.AsyncState;
            UdpClient handler = state.workSocket;

            try
            {
                state.buffer = handler.EndReceive(ar, ref udp_remote_endpoint);
            }
            catch
            {
                return;
            }

            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, state.buffer.Length));
            msg = state.sb.ToString();
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
                    del = BitConverter.ToInt32(state.buffer, MyProtocol.MOUSE_WHEEL.Length);
                    Functions.doMouseWheel(del);
                    break;

                case "EX":
                    break;

                default:
                    Functions.doMouseMove(state.buffer, widthRatio, heightRatio);
                    break;
            }

            mouse_event.Set();
        }

        private void ReadCallback(IAsyncResult ar)
        {
            int bytesRead = 0;
            String content = String.Empty;

            StateObject<Socket> state = (StateObject<Socket>)ar.AsyncState;
            Socket handler = state.workSocket;

            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch
            {
                return;
            }

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf(MyProtocol.END_OF_MESSAGE) > -1)
                {
                    this.processClipboardData(content);
                    clipboard_event.Set();
                }
                else
                    handler.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        public void ListenClipboardChannel()
        {
            while (connected)
            {
                clipboard_event.Reset();

                StateObject<Socket> state = new StateObject<Socket>(1);
                state.workSocket = this.clipbd_channel;

                try
                {
                    clipbd_channel.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
                catch (SocketException)
                {
                    Thread.CurrentThread.Suspend();
                    Console.WriteLine("Weeeeeeeeeeeee!!");
                }
                finally
                {
                    clipboard_event.WaitOne();
                }
            }
        }

        private void processClipboardData(string recvbuf)
        {
            string command = recvbuf.Substring(0, 4);
            byte[] data;
            Console.WriteLine(this.GetType().Name + " - ricevuto comando: " + recvbuf);

            if (clipbd_channel == null)
            {

                MessageBox.Show("clipbd channel a NULL!");
            }
            switch (command)
            {
                case MyProtocol.CLEAN:
                    // Now, I'm starting to clean my clipboard folder as you told me.
                    Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK),MyProtocol.POSITIVE_ACK.Length);
                    
                    Functions.CleanClipboardDir(Path.GetFullPath(MyProtocol.CLIPBOARD_DIR));
                    break;
                case MyProtocol.COPY:
                    string content;
                    int len = recvbuf.Length - MyProtocol.COPY.Length - MyProtocol.END_OF_MESSAGE.Length;
                    content = recvbuf.Substring(MyProtocol.COPY.Length, len);
                    //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                    //Clipboard.SetData(DataFormats.Text, content);

                    Functions.clipboardSetData(DataFormats.Text, content);

                    
                    
                    this.notify(20000, "Aggiornamento clipboard", "Ricevuto un testo dalla clipboard del client!", ToolTipIcon.Info);
                    break;

                case MyProtocol.FILE_SEND:
                    // Simulo un po' di ritardo di rete
                    //Thread.Sleep(1000);
                    // Now send ack
                    Functions.SendData(clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);
                    Functions.handleFileDrop(clipbd_channel, null);
                    Functions.UpdateClipboard();
                    this.notify(20000, "Aggiornamento clipboard", "Ricevuto un file dalla clipboard del client!", ToolTipIcon.Info);
                    break;

                case MyProtocol.DIRE_SEND:
                    // Simulo un po' di ritardo di rete
                    //Thread.Sleep(1000);

                    Functions.ReceiveDirectory(clipbd_channel);
                    Functions.UpdateClipboard();
                    break;

                case MyProtocol.IMG:
                    Functions.SendData(clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);
                    byte[] length = Functions.ReceiveData(clipbd_channel, sizeof(Int32));
                    Int32 length_int32 = BitConverter.ToInt32(length, 0);
                    Functions.SendData(clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);
                    byte[] imageSource = Functions.ReceiveData(clipbd_channel, length_int32);
                    Image image = Functions.ConvertByteArrayToBitmap(imageSource);
                   // Clipboard.SetImage(image);


                   Functions.clipboardSetImage(image);
                    
                    this.notify(20000, "Aggiornamento clipboard", "Ricevuta un'immagine dalla clipboard del client!", ToolTipIcon.Info);
                    break;


                case MyProtocol.CLIPBOARD_IMPORT:

                    bool data_available = Functions.handleClipboardDataForImport();


                    if (!data_available)
                    {
                        data = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.NEGATIVE_ACK));

                        Functions.SendData(clipbd_channel, data, 0, data.Length);
                    }
                    break;

                default:
                    this.notify(20000, "Comando non riconosciuto", command, ToolTipIcon.Info);
                    break;
            }

        }

        public void ListenTCPChannel()
        {

            while (connected)
            {

                try
                {

                    handler_event.Reset();

                    StateObject<Socket> state = new StateObject<Socket>(4);
                    state.workSocket = handler;
                    handler.BeginReceive(state.buffer, 0, state.BufferSize, 0, new AsyncCallback(ReadCallbackTCP), state);
                    handler_event.WaitOne();

                }

                catch (SocketException)
                {

                    int idd = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Sono il thread principale:" + idd);
                    MessageBox.Show("La connessione è stata interrotta. Assicurati che la connessione venga ristabilita affinchè i servizi riprendano automaticamente");

                    //this.suspend();
                    RetryPrimaryConnection();
                    RetryClipboardConnection();
                    this.wakeup();
                }
            }
        }

        private void ReadCallbackTCP(IAsyncResult ar)
        {
            int bytesRead = 0;
            string msg = string.Empty;
            byte[] bytes = null;

            StateObject<Socket> state = (StateObject<Socket>)ar.AsyncState;
            Socket handler = state.workSocket;

            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch
            {
                return;
            }

            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, state.buffer.Length));
            msg = state.sb.ToString();

            switch (msg)
            {
                case MyProtocol.QUIT:
                    if (Close())
                        Application.Exit();
                    break;

                case MyProtocol.TARGET:
                    byte[] mess = System.Text.Encoding.Default.GetBytes(MyProtocol.POSITIVE_ACK);
                    Functions.SendData(handler, mess, 0, mess.Length);
                    this.show();
                    break;


                case MyProtocol.PAUSE:
                    byte[] mess1 = System.Text.Encoding.Default.GetBytes(MyProtocol.POSITIVE_ACK);
                    Functions.SendData(handler, mess1, 0, mess1.Length);
                    Functions.KeyUp(0x11,0);
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

            handler_event.Set();

        }

        public override void RetryPrimaryConnection() {

            handler.Close();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            handler = listener.Accept();
            listener.Close();
            Functions.SetKeepAlive(handler, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);        
        }

        public override void RetryClipboardConnection(){

            clipbd_channel.Close();
            Socket clipbd_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clipbd_listener.Bind(clipboardLocalEndPoint);
            clipbd_listener.Listen(10);
            clipbd_channel = clipbd_listener.Accept();
            clipbd_listener.Close();
            Functions.SetKeepAlive(clipbd_channel, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);        
        
        }

    }
}