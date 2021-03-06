﻿using System;
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
    public class ClientConnectionHandler : ConnectionHandler
    {
        public delegate void RemoveTargetDelegate();
        public RemoveTargetDelegate remove_target;

        private IPEndPoint clipboardRemoteEP;

        public ClientConnectionHandler(Form mainForm, IPAddress ipAddress, Int32 port, string password) : base(mainForm, ipAddress, port, password) { }

        public override bool Open()
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

                // Set keepalive values
                Functions.SetKeepAlive(handler, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);

                // Connect the socket to the remote endpoint. Catch any errors.
                handler.Connect(remoteEP);

                msg = MyProtocol.message(MyProtocol.CONNECTION + this.password);
                bytes = Encoding.ASCII.GetBytes(msg);

                //MessageBox.Show("Devo inviare: " + msg);
                // Send connection request
                Functions.SendData(handler, bytes, 0, bytes.Length);


                // Receive the response from the remote device.
                len = MyProtocol.message(MyProtocol.POSITIVE_ACK).Length;
                bytes = Functions.ReceiveData(handler, len);
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

                    Functions.SendData(handler, resolution, 0, sizeof(Int32) * 2);

                    // Receive the UDP port of the server
                    bytes = Functions.ReceiveData(handler, sizeof(Int32));
                    Int32 udpRemotePort = BitConverter.ToInt32(bytes, 0);
                    Int32 udpLocalPort = Functions.FindFreePort();
                    IPAddress localIP = (handler.LocalEndPoint as IPEndPoint).Address;
                    IPEndPoint udpLocalEP = new IPEndPoint(localIP, udpLocalPort);
                    udp_channel = new UdpClient(udpLocalEP);
                    udp_channel.Connect(new IPEndPoint(remoteEP.Address, udpRemotePort));

                    // Send the UDP port to the server
                    bytes = BitConverter.GetBytes(udpLocalPort);
                    Functions.SendData(handler, bytes, 0, sizeof(Int32));

                    // Receive the TCP port of the server 
                    bytes = Functions.ReceiveData(handler, sizeof(Int32));
                    Int32 tcpRemotePort = BitConverter.ToInt32(bytes, 0);
                    clipboardRemoteEP = new IPEndPoint(remoteEP.Address, tcpRemotePort);
                    clipbd_channel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    Functions.SetKeepAlive(clipbd_channel, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);

                    clipbd_channel.Connect(clipboardRemoteEP);

                    Functions.SendClipboard = this.SendClipboard;
                    Functions.ReceiveClipboard = this.ReceiveClipboard;

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return false;
        }

        public override bool Close()
        {
            byte[] bytes;
            string msg;
     
            bytes = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.QUIT));
            Functions.SendData(handler, bytes, 0, bytes.Length);    // Client sends to server a quit request

            bytes = Functions.ReceiveData(handler, MyProtocol.message(MyProtocol.POSITIVE_ACK).Length); // Client receive a response from server
            msg = Encoding.ASCII.GetString(bytes);

            if (msg == MyProtocol.message(MyProtocol.POSITIVE_ACK))
            {
      
                this.handler.Close();
                this.clipbd_channel.Close();
                this.udp_channel.Close();

                return true;
            }

            return false;
        }

        public void SendUDP(byte[] bytes)
        {
            this.udp_channel.Send(bytes, bytes.Length);
        }

        public void SendTCP(Socket sock, byte[] bytes)
        {
            do
            {
                try
                {
                    Functions.SendData(this.handler, bytes, 0, bytes.Length);             
                    break;
                }
                catch (Exception)
                {
                    if (!RetryConnection())
                        break;
                }

            } while (true);
        }

        public byte[] ReceiveTCP(Socket sock, int size)
        {
            byte[] bytes = null;

            try
            {
                bytes = Functions.ReceiveData(this.handler, size);
            }
            catch (Exception)
            {
                this.RetryConnection();
            }

            return bytes;
        }

        public override void RetryPrimaryConnection()
        {
            try
            {
                handler.Close();
                handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Functions.SetKeepAlive(handler, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);

                handler.Connect(new IPEndPoint(ipAddress, port));
            }
            catch (SocketException)
            {
                throw;
            }
        }

        public override void RetryClipboardConnection()
        {
            try
            {
                clipbd_channel.Close();
                clipbd_channel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Functions.SetKeepAlive(clipbd_channel, MyProtocol.KEEPALIVE_TIME, MyProtocol.KEEPALIVE_INTERVAL);
                clipbd_channel.Connect(this.clipboardRemoteEP);
            }
            catch (SocketException)
            {
                throw;
            }
        }
 
        public bool RetryConnection()
        {
            bool success = false;

            ((MainForm)this.form).hook.Stop();

            for (int i = 0; i < MyProtocol.MAX_ATTEMPTS && !success; i++)
            {
                try
                {
                    this.RetryPrimaryConnection();

                    Functions.ReceiveData(this.handler, MyProtocol.POSITIVE_ACK.Length);

                    this.RetryClipboardConnection();

                    success = true;
                }
                catch
                {
                    MessageBox.Show("Impossibile comunicare con l'host remoto. Tentativo " + i + "/2");
                }
            }

            if (!success)
                remove_target();
            else
                ((MainForm)this.form).hook.Start();

            return success;
        }

    }
}