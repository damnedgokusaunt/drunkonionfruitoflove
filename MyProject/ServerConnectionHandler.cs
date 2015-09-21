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
    public class ServerConnectionHandler
    {
        private Socket tcp_channel, clipboard_channel;
        private IPAddress ipAddress;
        private Int32 port;
        private string password;
        private bool connected;
        private UdpClient udp_channel;
        private IPEndPoint udp_remote_endpoint;
        private Int32 widthRatio, heightRatio;



        public ServerConnectionHandler(IPAddress ipAddress, int port, string password)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.password = password;
            this.connected = false;
        }

        public byte[] Open()
        {
            string msg;
            byte[] bytes, endpoint_resolution;

            // Set local endpoint and create a TCP/IP socket.
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);

            tcp_channel = listener.Accept();
            listener.Close();

            msg = MyProtocol.message(MyProtocol.CONNECTION + this.password);

            //MessageBox.Show("Mi aspetto di ricevere: " + msg);

            // Receive connection request
            bytes = Functions.ReceiveData(ref tcp_channel, msg.Length);

            string received_request = Encoding.ASCII.GetString(bytes);

            if (received_request == msg)
            {
                Console.WriteLine("Richiesta di connessione accettata.");

                msg = MyProtocol.message(MyProtocol.POSITIVE_ACK);
                bytes = Encoding.ASCII.GetBytes(msg);
                Functions.SendData(ref tcp_channel, bytes, 0, bytes.Length);

                // Receive display resolution
                endpoint_resolution = Functions.ReceiveData(ref tcp_channel, sizeof(Int32) * 2);
                widthRatio = Screen.PrimaryScreen.Bounds.Width / BitConverter.ToInt32(endpoint_resolution, 0);
                heightRatio = Screen.PrimaryScreen.Bounds.Height / BitConverter.ToInt32(endpoint_resolution, sizeof(Int32));
                MessageBox.Show("Connesso.");

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
                Functions.SendData(ref tcp_channel, bytes, 0, bytes.Length);

                // Receive the UDP port from the server
                bytes = Functions.ReceiveData(ref tcp_channel, sizeof(Int32));
                udp_remote_endpoint = tcp_channel.RemoteEndPoint as IPEndPoint;

                //Notify the TCP port for clipboard to the client
                bytes = BitConverter.GetBytes(clipboardPort);
                Functions.SendData(ref tcp_channel, bytes, 0, bytes.Length);

                clipboard_listener.Listen(10);

                clipboard_channel = clipboard_listener.Accept();
                clipboard_listener.Close();

                this.connected = true;

                return endpoint_resolution;
            }
            else
            {
                MessageBox.Show("Richiesta non riconosciuta.");

                msg = MyProtocol.message(MyProtocol.NEGATIVE_ACK);
                bytes = Encoding.ASCII.GetBytes(msg);
                Functions.SendData(ref tcp_channel, bytes, 0, bytes.Length);

                return null;
            }
        }

        public void Close()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.POSITIVE_ACK));

            // Send ack
            Functions.SendData(ref tcp_channel, bytes, 0, bytes.Length);

            connected = false;

            tcp_channel.Close();
            udp_channel.Close();

            MessageBox.Show("Il server ha chiuso la connessione.");
        }

        public void setConnected()
        {
            this.connected = true;
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
            //parte operativa
            while (connected)
            {
                Console.WriteLine(this.GetType().Name + " - in attesa di comandi dal client...");

                string recvbuf = Functions.ReceiveTillTerminator(ref clipboard_channel);
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

                        MessageBox.Show("Ricevuto un Testo dalla clipboard del client.");
                        break;
                        /*
                    case MyProtocol.RTF:
                        int lun = recvbuf.Length - MyProtocol.END_OF_MESSAGE.Length - MyProtocol.RTF.Length;
                        content = recvbuf.Substring(MyProtocol.RTF.Length, lun);
                        //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                        Clipboard.SetData(DataFormats.Rtf, content);
                        break;*/
                    case MyProtocol.FILE_SEND:
                        // Simulo un po' di ritardo di rete
                        Thread.Sleep(1000);
                        // Now send ack
                        try
                        {
                            clipboard_channel.Send(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return;
                        }

                        Functions.handleFileDrop(ref clipboard_channel, null);
                        Functions.StartClipoardUpdaterThread();

                        MessageBox.Show("Ricevuto un File dalla clipboard del client.");
                        break;

                    case MyProtocol.DIRE_SEND:
                        // Simulo un po' di ritardo di rete
                        Thread.Sleep(1000);

                        Functions.ReceiveDirectory(ref clipboard_channel);
                        Functions.StartClipoardUpdaterThread();
                        break;

                    case MyProtocol.IMG:
                        Functions.SendData(ref clipboard_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                        byte[] length = Functions.ReceiveData(ref clipboard_channel, sizeof(Int32));

                        /*
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(length);*/

                        Int32 length_int32 = BitConverter.ToInt32(length, 0);

                        Functions.SendData(ref clipboard_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                        byte[] imageSource = Functions.ReceiveData(ref clipboard_channel, length_int32);

                        Image image = Functions.ConvertByteArrayToBitmap(imageSource);
                        Clipboard.SetImage(image);

                        MessageBox.Show("Ricevuta Immagine dalla clipboard del client.");

                        break;

                    default:
                        MessageBox.Show("Comando da tastiera non riconosciuto");
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
                bytes = Functions.ReceiveData(ref tcp_channel, commands_length);
                msg = Encoding.ASCII.GetString(bytes).Substring(0, MyProtocol.STD_COMMAND_LENGTH);

                switch (msg)
                {
                    case MyProtocol.QUIT:
                        Close();
                        break;

                    case MyProtocol.TARGET:

                        byte[] mess = System.Text.Encoding.Default.GetBytes(MyProtocol.POSITIVE_ACK);
                        Functions.SendData(ref tcp_channel, mess, 0, mess.Length);
                        MessageBox.Show("Sono il target numero: " + Process.GetCurrentProcess().Id);
                        break;

                    case MyProtocol.PAUSE:
                        
                        byte[] mess1 = System.Text.Encoding.Default.GetBytes(MyProtocol.POSITIVE_ACK);
                        Functions.SendData(ref tcp_channel, mess1, 0, mess1.Length);
                        MessageBox.Show("Sono il server messo in pausa numero: " + Process.GetCurrentProcess().Id);
                        break;

                    case MyProtocol.KEYDOWN:
                        bytes = Functions.ReceiveData(ref tcp_channel, 1);
                        Functions.KeyDown(bytes[0]);
                       break;

                    case MyProtocol.KEYUP:
                        bytes = Functions.ReceiveData(ref tcp_channel, 1);
                        Functions.KeyUp(bytes[0]);
                        break;
                }
            }
        }
    }
}