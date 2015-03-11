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

    public partial class Form1 : Form
    {

        private delegate void InvokeDelegate(string text, int id);

        private SynchronousSocketClient client { get; set; }
        private int CurrentSocketId { get; set; }
        private SynchronousSocketListener listener;

        //getters
        public int getCurrentSocketId() { return CurrentSocketId; }

        //setters
        public void setListener(SynchronousSocketListener listener) { this.listener = listener; }

        public void StatusUpdate(string text, int id)
        {
            if (InvokeRequired)
            {
                Invoke(new InvokeDelegate(StatusUpdate), text, id);
                return;
            }
            listView.Items[id].SubItems[2].Text = text;
        }


        public Form1()
        {

            InitializeComponent();

            // Set the view to show details.
            listView.View = View.Details;
            // Allow the user to edit item text.
            listView.LabelEdit = true;
            // Allow the user to rearrange columns.
            listView.AllowColumnReorder = true;
            // Display check boxes.
            //listView.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            listView.FullRowSelect = true;
            // Display grid lines.
            listView.GridLines = true;
            // Sort the items in the list in ascending order.
            listView.Sorting = SortOrder.Ascending;

            // Create columns for the items and subitems.
            // Width of -2 indicates auto-size.
            listView.Columns.Add("Indirizzo IP", 100, HorizontalAlignment.Left);
            listView.Columns.Add("Porta", 40, HorizontalAlignment.Left);
            listView.Columns.Add("Stato", -2, HorizontalAlignment.Left);

            CurrentSocketId = -1;

     

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                listener = new SynchronousSocketListener();
               
                listener.setIpAddress(IPAddress.Any);
                listener.setPort(Convert.ToInt32(portBox.Text));
                listener.setExpectedConnectionRequest("PASS" + PasswordEncrypterMD5.Encrypt(pwd2.Text) + "<EOF>");

                Thread t = new Thread(listener.StartListening);

                t.Start();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dsfdsdsdfToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void chiudiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void nuovoServerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void startClientButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Istanzia oggetto client
                client = new SynchronousSocketClient();

                client.setForm(this);

                client.setIpAddress(IPAddress.Parse(serverAddrBox.Text));
                client.setPort(Convert.ToInt32(serverPortBox.Text));              
                client.setPwd("PASS" + PasswordEncrypterMD5.Encrypt(Convert.ToString(pwd1.Text)));

                Thread t = new Thread(client.StartClient);

                t.Name = "connection_starter_t" + CurrentSocketId;

                t.Start();

                // Create three items and three sets of subitems for each item.
                ListViewItem item = new ListViewItem(serverAddrBox.Text, client.getId());

                // Place a check mark next to the item.
                item.Checked = true;

                item.SubItems.Add(serverPortBox.Text);
                item.SubItems.Add("tmp");

                //Add the items to the ListView.
                listView.Items.AddRange(new ListViewItem[] { item });

                // Add the ListView to the control collection.
                this.Controls.Add(listView);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverAddrBox.Text = string.Empty;
            serverPortBox.Text = string.Empty;
            pwd1.Text = string.Empty;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void item_DoubleClick(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                CurrentSocketId = listView.FocusedItem.Index;
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Seleziona un server.");
                return;
            }

            client.setCurrentSocket(CurrentSocketId);

            MessageBox.Show("Invio richiesta di controllo a: " + client.getCurrentSocket().RemoteEndPoint.ToString());

            Thread t = new Thread(client.StartControl);
            t.Start();

        }


        private void getMousePosition(object sender, MouseEventArgs e)
        {


        }

        private void button3_Click(object sender, EventArgs e)
        {
          
        }

        private void down(object sender, KeyEventArgs e)
        {

        }

       


    }

}
