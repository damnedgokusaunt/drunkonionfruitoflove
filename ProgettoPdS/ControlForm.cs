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
        #region Attributes
        private Point CurrentPosition;
        private UdpClient mouseChannel;
        private IPEndPoint mouseRemoteEP;
        private Socket keybdChannel;
        private IPEndPoint keybdRemoteEP;
        public Socket CurrentSocket;
        private IPEndPoint udpRemoteEndPoint, tcpRemoteEndPoint;

        public Int32 x, y;
        public byte[] point;
        private byte[] keybd;

        GlobalKeyboardHook gkh;

        #endregion
        #region Constructor
        public ControlForm(IPAddress ipAddress, int port)
        {
            InitializeComponent();

            mouseRemoteEP = new IPEndPoint(ipAddress, port + 1);
            mouseChannel = new UdpClient();

            keybdRemoteEP = new IPEndPoint(ipAddress, port + 2);
            keybdChannel = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            keybdChannel.Connect(keybdRemoteEP);

            gkh = new GlobalKeyboardHook();
            point = new byte[sizeof(Int32) * 2];
            keybd = new byte[2];
        }
        #endregion

        private void ControlForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            gkh.HookedKeys.Add(Keys.A);
            gkh.HookedKeys.Add(Keys.B);
            gkh.KeyDown += new KeyEventHandler(ControlForm_KeyDown);
            gkh.KeyUp += new KeyEventHandler(ControlForm_KeyUp);
        }

        #region Mouse methods
        private void ControlForm_MouseMove(object sender, MouseEventArgs e)
        {
            // Concatena due byte[] (uno per la X, l'altro per la Y)
            Buffer.BlockCopy(BitConverter.GetBytes(Cursor.Position.X), 0, point, 0, sizeof(Int32));
            Buffer.BlockCopy(BitConverter.GetBytes(Cursor.Position.Y), 0, point, sizeof(Int32), sizeof(Int32));

            try
            {
                //Console.WriteLine("devo inviare mousemove");
                // Invia l'array risultante
                mouseChannel.Send(point, sizeof(Int32) * 2, mouseRemoteEP);

                //Console.WriteLine("Inviate coordinate: " + point[0] + "x" + point[1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ControlForm_MouseDown(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("entrato nella mousedown\n");

            if (e.Button == MouseButtons.Right)
            {
                //Console.WriteLine("devo inviare mousedowndx");
                string msg = "CR<EOF>";
                mouseChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, mouseRemoteEP);
                //Console.WriteLine("Ho inviato: " + msg);

            }

            if (e.Button == MouseButtons.Left)
            {
                //Console.WriteLine("devo inviare mousedownsx");
                string msg = "CLD<EOF>";
                mouseChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, mouseRemoteEP);
                //Console.WriteLine("Ho inviato: " + msg);
                //Console.WriteLine("gne gne");
            }
        }
        private void ControlForm_MouseUp(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("entrato nella mouseup\n");

            if (e.Button == MouseButtons.Left)
            {
                //Console.WriteLine("devo inviare mouseup");

                string msg = "CLU<EOF>";
                mouseChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, mouseRemoteEP);
            }     

        }
        private void ControlForm_MouseWheel(object sender, MouseEventArgs e)
        {
            int d = e.Delta;

            //Console.WriteLine("devo inviare mousewheel");
            string msg = "CS" + d + "<EOF>";

            mouseChannel.Send(Encoding.ASCII.GetBytes(msg), msg.Length, mouseRemoteEP);
        }
        #endregion

        #region Keyboard methods
        private void ControlForm_KeyDown(object sender, KeyEventArgs e)
        {
            
                keybd[0] = (byte)'D';
                keybd[1] = (byte)e.KeyData;

                keybdChannel.Send(keybd);

                e.Handled = true;
        
        }
        private void ControlForm_KeyUp(object sender, KeyEventArgs e)
        {
            
                keybd[0] = (byte)'U';
                keybd[1] = (byte)e.KeyData;

                keybdChannel.Send(keybd);

                e.Handled = true;
        }
        #endregion
       
    }
}
