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


namespace MyProject
{
    public partial class ServerForm : Form
    {
        bool isAboutLoaded = false;
        private ServerConnectionHandler listener;
        private IPAddress addr;
        private Thread consumer_tcp, consumer_udp, clipboard_worker;
        private TargetForm frm;
  
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

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addr = IPAddress.Parse(comboBox.Text);

            Console.WriteLine("IP Address: " + addr);
        }

        public ServerForm()
        {
            InitializeComponent();

            PopulateIPAddressList();

            portBox.Text = Convert.ToString(Functions.FindFreePort());
            
            this.frm= new TargetForm();
            
        }

      //  private NotifyIcon notifier;


        public void show_target_form() {

            if (this.InvokeRequired)
            {
               this.BeginInvoke(new Action(() => this.frm.Show()));
            }
        }

        public void notify_me(int a, string b, string c, ToolTipIcon cletta)
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.notifyIcon1.ShowBalloonTip(a,b,c,cletta)));
            }
        }


        public void hide_target_form()
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.frm.Hide()));
            }
        }


        // To minimaze inside the tray area
        private void TrayMinimizerForm_Resize(object sender, EventArgs e)
        {
            
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                this.Hide();
                    
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
                
            }

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // Check that all the fields are valid
            if (this.passwordBox.Text == string.Empty || this.comboBox.Text == string.Empty || this.portBox.Text == string.Empty)
            {
               this.notify_me(20000, "Attenzione", "Alcuni campi risultano vuoti!", ToolTipIcon.Info);
                
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
                //to associate delegates to methods
                listener = new ServerConnectionHandler(this, this.addr, Convert.ToInt32(portBox.Text), Functions.Encrypt(passwordBox.Text));
                //delegates for target
                listener.show = this.show_target_form;
                listener.hide = this.hide_target_form;
                listener.notify = this.notify_me;
                listener.wakeup = this.wakeup_threads;
                listener.suspend = this.suspend_threads;

                Task<bool> t = Task.Factory.StartNew(() => listener.Open());

                t.Wait();

                if (t.Result)
                {
                    listener.Connected = true;


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
            throw new NotImplementedException();
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
            this.notifyIcon1.ShowBalloonTip(20000, "Attenzione", "Affinchè l'applicazione funzioni correttamente è necessario che il PC sia connesso ad una rete LAN!", ToolTipIcon.Info);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            portBox.Text = Convert.ToString(Functions.FindFreePort());
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        public void suspend_threads()
        {
            if(this.consumer_udp.ThreadState != ThreadState.Suspended)
            {
                this.consumer_udp.Suspend();
            }
            if (this.clipboard_worker.ThreadState != ThreadState.Suspended)
            {
                this.clipboard_worker.Suspend();
            }
        }

        public void wakeup_threads() 
        {
            try
            {
                this.consumer_udp.Resume();

            }
            catch { }

            try
            {
                this.clipboard_worker.Resume();

            }
            catch { }

        }


        private void mostraFinestraPrincipaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

    }
}
