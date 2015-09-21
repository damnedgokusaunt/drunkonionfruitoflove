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
        public ClientConnectionHandler(Form mainForm, IPAddress ipAddress, Int32 port, string password) : base(mainForm, ipAddress, port, password) { }

        // Questo metodo viene invocato per connettere l'oggetto con un server
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

                // Connect the socket to the remote endpoint. Catch any errors.
                handler.Connect(remoteEP);
                
                msg = MyProtocol.message(MyProtocol.CONNECTION + this.password);
                bytes = Encoding.ASCII.GetBytes(msg);

                //MessageBox.Show("Devo inviare: " + msg);
                // Send connection request
                Functions.SendData(handler, bytes, 0, bytes.Length);

                ((MainForm)form).UpdateProgress(0.0, "Richiesta di connessione inviata.");

                // Receive the response from the remote device.
                len = MyProtocol.message(MyProtocol.POSITIVE_ACK).Length;
                bytes = Functions.ReceiveData(handler, len);
                msg = Encoding.ASCII.GetString(bytes, 0, len);

                if (msg == MyProtocol.message(MyProtocol.POSITIVE_ACK))
                {
                    Int32 width = Screen.PrimaryScreen.Bounds.Width;
                    Int32 height = Screen.PrimaryScreen.Bounds.Height;

                    Console.WriteLine("Thread secondario: " + Thread.CurrentThread.ManagedThreadId);
                    ((MainForm)form).UpdateProgress(0.5, "Connessione accettata dal sistema remoto.");

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
                    IPEndPoint clipboardRemoteEP = new IPEndPoint(remoteEP.Address, tcpRemotePort);
                    clipbd_channel = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                    clipbd_channel.Connect(clipboardRemoteEP);
                    
                    ((MainForm)form).UpdateProgress(1.0, "Connessione instaurata con successo.");
                    
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
            int len;
            byte[] bytes;
            string msg;

            bytes = Encoding.ASCII.GetBytes(MyProtocol.message(MyProtocol.QUIT));
            // Client sends to server a quit request
            Functions.SendData(handler, bytes, 0, bytes.Length);

            len = MyProtocol.message(MyProtocol.POSITIVE_ACK).Length;
            // Client receive a response from server
            bytes = Functions.ReceiveData(handler, len);
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
            Functions.SendData(handler, bytes, 0, bytes.Length);
        }

        public byte[] Receive(int size)
        {
            return Functions.ReceiveData(handler, size);
        }

        public void SendUDP(byte[] bytes)
        {
            udp_channel.Send(bytes, bytes.Length);
        }
    }
}