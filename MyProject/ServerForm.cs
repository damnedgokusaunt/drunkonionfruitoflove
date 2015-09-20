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
using System.Net.NetworkInformation;


namespace ProgettoPdS
{
    public partial class ServerForm : Form
    {
       //public delegate void Del();
        bool isAboutLoaded = false;
        private ServerConnectionHandler listener;
     //   private ControlForm ctrl;
        private IPAddress addr;

        private Thread consumer_tcp, consumer_udp, clipboard_worker;

        /// <summary>
        /// This method get all addresses of the host and insert them in the combobox
        /// </summary>
        private void PopulateIPAddressList()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in localIPs)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    this.comboBox.Items.Add(ip.ToString());
            }

            IEnumerator en = comboBox.Items.GetEnumerator();
            en.MoveNext();
            
            this.comboBox.Text = en.Current.ToString();
        }

        public ServerForm()
        {
            InitializeComponent();

            
            // Populate comboBox
            PopulateIPAddressList();
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

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addr = IPAddress.Parse(comboBox.Text);

            Console.WriteLine("IP Address: " + addr);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // Check that all the fields are valid
            if (this.passwordBox.Text == string.Empty || this.comboBox.Text == string.Empty || this.portBox.Text == string.Empty)
            {
                MessageBox.Show("Alcuni campi risultano vuoti.");
                return;
            }

            // Invalid all the dangerous fields
            this.comboBox.Enabled = false;
            this.portBox.Enabled = false;
            this.passwordBox.Enabled = false;
            this.startButton.Enabled = false;
            this.quitButton.Enabled = true;

            // Create a thread responsible for connection enstablishment
            // Wait for 1 minute
            // If the thread does not terminate, kill it and return

            try
            {
                listener = new ServerConnectionHandler(this.addr, Convert.ToInt32(portBox.Text), Functions.Encrypt(passwordBox.Text));

                Task<byte[]> t = Task.Factory.StartNew(() => listener.Open());

                t.Wait();

                if (t.Result != null)
                {
                    listener.setConnected();
                    //MessageBox.Show("Connesso.");

                    consumer_tcp = new Thread(listener.ListenTCPChannel);
                    consumer_tcp.IsBackground = true;

                    consumer_udp = new Thread(listener.ListenUDPChannel);
                    consumer_udp.IsBackground = true;

                    clipboard_worker = new Thread(listener.ListenClipboardChannel);
                    clipboard_worker.IsBackground = true;
                    clipboard_worker.SetApartmentState(ApartmentState.STA);
                    
                    
                    consumer_tcp.Start();
                    consumer_udp.Start();
                    clipboard_worker.Start();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return;
            }
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("@ale: non ho ancora implementato questa funzione.");

            /*
            this.comboBox1.Enabled = true;
            this.portBox.Enabled = true;
            this.pwd2.Enabled = true;
            this.button3.Enabled = false;
            this.startButton.Enabled = true;
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

        private void ServerForm_Load(object sender, EventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                MessageBox.Show("Affinchè l'applicazione funzioni correttamente è necessario che il PC sia connesso ad una rete LAN.");

        }

    }
}
