using System;
using System.Net;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;

namespace ProgettoPdS
{
    public partial class ControlForm : Form
    {
        private Point CurrentPosition;

        private UdpClient udpChannel;
        private Socket tcpChannel;

        public Socket CurrentSocket;

        private IPEndPoint udpRemoteEndPoint, tcpRemoteEndPoint;

        public Int32 x, y;
        public byte[] point;
        private byte[] keybd;


        public void setUdpChannel(ref UdpClient udpChannel)
        {
            this.udpChannel = udpChannel;
        }

        public void setTcpChannel(ref Socket tcpChannel)
        {
            this.tcpChannel = tcpChannel;
        }

        public Point getCurrentPosition()
        {
            return CurrentPosition;
        }

        public void setUdpRemoteEndPoint(IPEndPoint udpRemoteEndPoint)
        {
            this.udpRemoteEndPoint = udpRemoteEndPoint;
        }

        public void setTcpRemoteEndPoint(IPEndPoint tcpRemoteEndPoint)
        {
            this.tcpRemoteEndPoint = tcpRemoteEndPoint;
        }

        public ControlForm()
        {

            InitializeComponent();
            
            point = new byte[sizeof(Int32) * 2];
            keybd = new byte[2];
            
            /*
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset);

            Thread t = new Thread(new ParameterizedThreadStart(vai));
            t.Start();
             * */           
        }

        private void ControlForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        private void MouseMoveIntercepter(object sender, MouseEventArgs e)
        {
            // Concatena due byte[] (uno per la X, l'altro per la Y)
            Buffer.BlockCopy(BitConverter.GetBytes(Cursor.Position.X), 0, point, 0, sizeof(Int32));
            Buffer.BlockCopy(BitConverter.GetBytes(Cursor.Position.Y), 0, point, sizeof(Int32), sizeof(Int32));

            try
            {
                // Invia l'array risultante
                udpChannel.Send(point, sizeof(Int32) * 2, udpRemoteEndPoint);
                //Console.WriteLine("Inviate coordinate: " + point[0] + "x" + point[1]);
            }
            catch (SocketException)
            {
                MessageBox.Show("Impossibile comunicare col ser");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Esci(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ControlForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                string msg = "CR<EOF>";
                udpChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, udpRemoteEndPoint);
                //Console.WriteLine("Ho inviato: " + msg);

            }

            if (e.Button == MouseButtons.Left)
            {
                string msg = "CLD<EOF>";
                udpChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, udpRemoteEndPoint);
                //Console.WriteLine("Ho inviato: " + msg);
                //Console.WriteLine("gne gne");
            }
        }

        private void ControlForm_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                string msg = "CLU<EOF>";
                udpChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, udpRemoteEndPoint);
                //Console.WriteLine("Ho inviato: " + msg);
            }     

        }

        // Metodo per MouseWheel
        private void ControlForm_MouseWheel(object sender, MouseEventArgs e)
        {
            int d = e.Delta;

            //Console.WriteLine("Inviato: " + d);
            string msg = "CS" + d + "<EOF>";
            
            udpChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, udpRemoteEndPoint);
        }

        private void ControlForm_KeyDown(object sender, KeyEventArgs e)
        {
            keybd[0] = (byte)'D';
            keybd[1] = (byte)e.KeyData;

            tcpChannel.Send(keybd);
        }

        private void ControlForm_KeyUp(object sender, KeyEventArgs e)
        {
            keybd[0] = (byte)'U';
            keybd[1] = (byte)e.KeyData;

            tcpChannel.Send(keybd);
        }



        
    }
}
