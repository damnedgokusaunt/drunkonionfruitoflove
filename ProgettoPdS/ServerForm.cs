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



namespace ProgettoPdS
{
    public partial class ServerForm : Form
    {

        private delegate void InvokeDelegate(string text, int id);

        private SynchronousSocketClient client;
        private int CurrentSocketId;
        private SynchronousSocketListener listener;
        private ControlForm ctrl;
        //getters
        public int getCurrentSocketId() { return CurrentSocketId; }

        //setters
        public void setListener(SynchronousSocketListener listener) { this.listener = listener; }

        public ServerForm()
        {
            InitializeComponent();
            
            ProcessIcon pi = new ProcessIcon();

            var me = this;
            pi.setFrm(ref me);
            pi.Display();
            
            
        }
        

       
        // To minimaze inside the tray area

        private void TrayMinimizerForm_Resize(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipTitle = "Minimize to Tray App";
            notifyIcon1.BalloonTipText = "You have successfully minimized your form.";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
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
            try
            {
                listener = new SynchronousSocketListener(
                    IPAddress.Any,
                    Convert.ToInt32(portBox.Text),
                    PasswordEncrypterMD5.Encrypt(pwd2.Text));

                Thread t = new Thread(listener.startListening);

                t.Start();
            }
            catch (FormatException ex)
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
    }
}
