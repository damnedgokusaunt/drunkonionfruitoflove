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
        public delegate void TargetHandlerShow();
        public delegate void TargetHandlerHide();
        public TargetHandlerShow show;
        public TargetHandlerHide hide;

        
        
        private IPEndPoint udp_remote_endpoint;
        private Int32 widthRatio, heightRatio;
        private byte[] endpoint_resolution;
     //   Thread worker_target;
      //  Thread sleeper_target;

        //Delegate for TargetForm
    /*   public delegate void open_target();
       public delegate void close_target();
       public open_target open;
       public close_target close;
 */




        private bool connected;
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
          
         //   worker_target = new Thread(() => Application.Run(frm));
          //  sleeper_target = new Thread(() => frm.Close());
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

            msg = MyProtocol.message(MyProtocol.CONNECTION + this.password);

            //MessageBox.Show("Mi aspetto di ricevere: " + msg);

            // Receive connection request
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
                ServerForm.Notifier.ShowBalloonTip(20000, "Info", "Connesso!", ToolTipIcon.Info);
               
               // MessageBox.Show("Connesso.");

                // Create UDP channel
                Int32 udpPort = Functions.FindFreePort();
                IPEndPoint udpLocalEndPoint = new IPEndPoint(ipAddress, udpPort);
                udp_channel = new UdpClient(udpLocalEndPoint);

                // Create TCP channel for clipboard operations (import/export)
                Int32 clipboardPort = Functions.FindFreePort();
                IPEndPoint clipboardLocalEndPoint = new IPEndPoint(ipAddress, clipboardPort);
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
                ServerForm.Notifier.ShowBalloonTip(20000, "Info", "Richiesta non riconosciuta!", ToolTipIcon.Info);
               
              //  MessageBox.Show("Richiesta non riconosciuta.");

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
            ServerForm.Notifier.ShowBalloonTip(20000, "Info", "Il server ha chiuso la connessione!", ToolTipIcon.Info);
               
            //MessageBox.Show("Il server ha chiuso la connessione.");

            return true;
        }

        public void ListenUDPChannel()
        {
            int del = 0;
            string msg = string.Empty, cmd = string.Empty;
            byte[] bytes = new byte[sizeof(Int32) * 2];
            

            while (connected)
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

        }

        public void ListenClipboardChannel()
        {
            byte[] data;

            while (connected)
            {
                Console.WriteLine(this.GetType().Name + " - in attesa di comandi dal client...");

                string recvbuf = Functions.ReceiveTillTerminator(clipbd_channel);
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
                        ServerForm.Notifier.ShowBalloonTip(20000, "Aggiornamento clipboard", "Ricevuto un testo dalla clipboard del client!", ToolTipIcon.Info);
               
                        //MessageBox.Show("Ricevuto un Testo dalla clipboard del client.");
                        break;
                    
                    case MyProtocol.FILE_SEND:
                        // Simulo un po' di ritardo di rete
                        Thread.Sleep(1000);
                        // Now send ack
                        try
                        {
                            clipbd_channel.Send(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return;
                        }

                        Functions.handleFileDrop(clipbd_channel, null);
                        Functions.StartClipoardUpdaterThread();
                        ServerForm.Notifier.ShowBalloonTip(20000, "Aggiornamento clipboard", "Ricevuto un file dalla clipboard del client!", ToolTipIcon.Info);
               
                        //MessageBox.Show("Ricevuto un File dalla clipboard del client.");
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

                        /*
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(length);*/

                        Int32 length_int32 = BitConverter.ToInt32(length, 0);

                        Functions.SendData(clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                        byte[] imageSource = Functions.ReceiveData(clipbd_channel, length_int32);

                        Image image = Functions.ConvertByteArrayToBitmap(imageSource);
                        Clipboard.SetImage(image);
                        ServerForm.Notifier.ShowBalloonTip(20000, "Aggiornamento clipboard", "Ricevuta un'immagine dalla clipboard del client!", ToolTipIcon.Info);
               
                       // MessageBox.Show("Ricevuta Immagine dalla clipboard del client.");

                        break;
                    
                    case MyProtocol.CLIPBOARD_IMPORT:
                        bool data_available = Functions.handleClipboardData(this);
                        
                        if(!data_available)
                        {
                            data = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.NEGATIVE_ACK));

                            Functions.SendData(clipbd_channel, data, 0, data.Length);
                        }
                        break;

                    default:
                        ServerForm.Notifier.ShowBalloonTip(20000, "Aggiornamento clipboard", "Comando da tastiera non riconosciuto!", ToolTipIcon.Info);
               
                       // MessageBox.Show("Comando da tastiera non riconosciuto");
                        break;
                }
            }
        }
    
        public void ListenTCPChannel()
        {
            byte[] bytes;
            string msg;
            Int32 commands_length = MyProtocol.STD_COMMAND_LENGTH;

            while (connected)
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
                        bytes = Functions.ReceiveData(handler, 1);
                        Functions.KeyDown(bytes[0]);
                       break;

                    case MyProtocol.KEYUP:
                        bytes = Functions.ReceiveData(handler, 1);
                        Functions.KeyUp(bytes[0]);
                        break;
                }
            }
        }
    }
}