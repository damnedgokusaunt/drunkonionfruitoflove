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

using System.Runtime.InteropServices;

namespace ProgettoPdS
{
    public partial class ControlForm : Form
    {
        #region Import methods for Clipboard

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private System.Windows.Forms.RichTextBox richTextBox1;

        IntPtr nextClipboardViewer;

        #endregion

        #region Attributes

        // Remote end points
        private IPEndPoint mouseRemoteEP, keybdRemoteEP, clipbdRemoteEP;

        // Sockets
        private UdpClient mouseChannel;
        private Socket keybdChannel, clipbdChannel;
        
        public Socket CurrentSocket;
        

        public Int32 x, y;
        public byte[] point;
        private byte[] keybd;
        private byte[] clipbd;

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


            clipbdRemoteEP = new IPEndPoint(ipAddress, port + 3);
            clipbdChannel = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            clipbdChannel.Connect(clipbdRemoteEP);

            
        }
        #endregion

        private void ControlForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            Clipboard.Clear();
            gkh.HookedKeys.Add(Keys.A);
            gkh.HookedKeys.Add(Keys.B);
            gkh.KeyDown += new KeyEventHandler(ControlForm_KeyDown);
            gkh.KeyUp += new KeyEventHandler(ControlForm_KeyUp);

            #region Some code for Clipboard
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            #endregion

            Clipboard.Clear();
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

        #region Clipboard methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        void handleClipboardData()
        {

            try
            {
                string clipbdMsg;

                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();

                if (Clipboard.ContainsData(DataFormats.Text))
                {
                    clipbdMsg = MyProtocol.COPY + (string)iData.GetData(DataFormats.Text) + MyProtocol.END_OF_MESSAGE;
                    clipbdChannel.Send(Encoding.ASCII.GetBytes(clipbdMsg));
                }
                                      
                /*
                if (iData.GetDataPresent(DataFormats.Rtf))
                    MessageBox.Show((string)iData.GetData(DataFormats.Rtf));
                //richTextBox1.Rtf = (string)iData.GetData(DataFormats.Rtf);
                else if (iData.GetDataPresent(DataFormats.Text))
                    MessageBox.Show((string)iData.GetData(DataFormats.Text));
                //richTextBox1.Text = (string)iData.GetData(DataFormats.Text);
                else
                    MessageBox.Show("[Clipboard data is not RTF or ASCII Text]");
                //richTextBox1.Text = "[Clipboard data is not RTF or ASCII Text]";
                 * */
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (clipbdChannel == null)
                return;

            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:                    
                    handleClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;
                /*
                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;
                */
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        
        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
