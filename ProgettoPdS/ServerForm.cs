using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Collections;



namespace ProgettoPdS
{
    public partial class ServerForm : Form
    {
       //public delegate void Del();
        bool isAboutLoaded = false;
        private SynchronousSocketClient client;
        private int CurrentSocketId;
        private SynchronousSocketListener listener;
        private ControlForm ctrl;
        private IPAddress addr;
        
        //getters
        public int getCurrentSocketId() { return CurrentSocketId; }

        //setters
        public void setListener(SynchronousSocketListener listener) { this.listener = listener; }

        public ServerForm()
        {
            InitializeComponent();
            
            // IP address
            addr = null;

            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName()); //get all addresses of the host
            foreach (IPAddress ip in localIPs)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    this.comboBox1.Items.Add(ip.ToString());
            }
            IEnumerator en = comboBox1.Items.GetEnumerator();
            en.MoveNext();
            this.comboBox1.Text = en.Current.ToString();

            // TCP port
            portBox.Text = MyProtocol.DEFAULT_PORT.ToString();

            //pi = new ProcessIcon();

            //var me = this;
            //pi.setFrm(ref me);
           

        }



        // To minimaze inside the tray area

        private void TrayMinimizerForm_Resize(object sender, EventArgs e)
        {
            //notifyIcon1.BalloonTipTitle = "Minimize to Tray App";
            //notifyIcon1.BalloonTipText = "You have successfully minimized your form.";

            if (FormWindowState.Minimized == this.WindowState)
            {
                //notifyIcon1.Visible = true;
                //notifyIcon1.ShowBalloonTip(1000);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
                //this.Show();
            }

        }


        private void portBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.comboBox1.Enabled = false;
            this.portBox.Enabled = false;
            this.pwd2.Enabled = false;

            this.button2.Enabled = false;
            this.button3.Enabled = true;

            try
            {
                listener = new SynchronousSocketListener(
                    this.addr,
                    Convert.ToInt32(portBox.Text),
                    MyProtocol.Encrypt(pwd2.Text));

                Thread t = new Thread(listener.startListening);

                t.Start();

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void Start_Form_Load(object sender, EventArgs e)
        {

        }

        private void Start_Form_Load_1(object sender, EventArgs e)
        {

        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            addr = IPAddress.Parse(comboBox1.Text);

            Console.WriteLine("IP Address: " + addr);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("@ale: non ho ancora implementato questa funzione.");

            /*
            this.comboBox1.Enabled = true;
            this.portBox.Enabled = true;
            this.pwd2.Enabled = true;
            this.button3.Enabled = false;
            this.button2.Enabled = true;
             * */
        }
        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Explorer_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void About_Click(object sender, EventArgs e)
        {
            if (!isAboutLoaded)
            {
                isAboutLoaded = true;
                new AboutBox().ShowDialog();
                isAboutLoaded = false;
            }
        }

        /// <summary>
        /// Processes a menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Exit_Click(object sender, EventArgs e)
        {
            // Quit without further ado.
            Application.Exit();
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {

        }

    }
}
