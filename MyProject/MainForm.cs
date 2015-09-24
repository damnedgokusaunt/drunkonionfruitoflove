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
using System.IO;
using System.Diagnostics;


namespace MyProject
{
    public partial class MainForm : Form
    {
        public delegate void ProgressHandler(double v, string message);
        public ProgressHandler UpdateProgress;

        private Dictionary<Int32, ClientConnectionHandler> connections;
        private ClientConnectionHandler current, target;
        private HotkeysHandler hotkeys_handler;
        private GlobalHook hook;

        private SetupHotkeysForm hotkeys_form;

        #region Hotkey

        private enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        protected override void WndProc(ref Message m)
        {         
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                switch (id)
                {
                    case 0:
                        SelectServer();
                        break;
                    case 2:
                        SuspendServer();
                        break;
                    case 4:
                        ImportClipboard();
                        break;
                    case 6:
                        ExportClipboard();
                        break;
                    default:
                        throw new Exception("Hotkey non riconosciuta");
                }
            }
        }
        
        #endregion

        #region Keyboard methods
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (target != null)
            {
               target.Send(Encoding.ASCII.GetBytes(MyProtocol.KEYDOWN));
               target.Send(new byte[]{(byte)e.KeyData});
            }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (target != null)
            {
               target.Send(Encoding.ASCII.GetBytes(MyProtocol.KEYUP));
               target.Send(new byte[]{(byte)e.KeyData});
            }
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
                  
        }
        #endregion

        #region Mouse methods
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            byte[] bytes = new byte[sizeof(int)*2];
            Array.Copy(BitConverter.GetBytes(e.X), bytes, sizeof(int));
            Array.Copy(BitConverter.GetBytes(e.Y), 0, bytes, sizeof(int), sizeof(int));

            // Vengono inviati 8 byte
            try
            {
                current.SendUDP(bytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            string cmd = string.Empty;

            if (e.Button == MouseButtons.Right)
            {
                cmd = MyProtocol.MOUSE_DOWN_RIGHT;
            }
            else
                cmd = MyProtocol.MOUSE_DOWN_LEFT;

            try
            {
                current.SendUDP(Encoding.ASCII.GetBytes(cmd));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    current.SendUDP(Encoding.ASCII.GetBytes(MyProtocol.MOUSE_UP_LEFT));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            byte[] bytes = new byte[MyProtocol.MOUSE_WHEEL.Length + sizeof(int)];
            Array.Copy(Encoding.ASCII.GetBytes(MyProtocol.MOUSE_WHEEL), bytes, MyProtocol.MOUSE_WHEEL.Length);
            Array.Copy(BitConverter.GetBytes(e.Delta), 0, bytes, MyProtocol.MOUSE_WHEEL.Length, sizeof(int));

            try
            {
                current.SendUDP(bytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        public void NewConnection(string ip_address, string port, string password)
        {
            try
            {
                // Istanzia oggetto client
                ClientConnectionHandler new_connection = new ClientConnectionHandler(this, IPAddress.Parse(ip_address), Convert.ToInt32(port), Functions.Encrypt(password));

                //connection.setForm(this);
                Task<bool> t = Task.Factory.StartNew(() => new_connection.Open());

                // Wait only for 3 minutes
                bool isCompletedInTime = t.Wait(3000 * 60);
                if (!isCompletedInTime)
                {
                    throw new NotImplementedException();
                }

                if (t.Result)
                {
                    Int32 key = connections.Count();
                    // Create three items and three sets of subitems for each item.
                    ListViewItem item = new ListViewItem(key.ToString(), key);

                    // Place a check mark next to the item.
                    item.Checked = true;
                    item.SubItems.Add(ip_address);
                    item.SubItems.Add(port);
                    item.SubItems.Add("Pausa.");

                    //Add the items to the ListView.
                    listView.Items.AddRange(new ListViewItem[] { item });

                    // Add connection
                    connections.Add(key, new_connection);
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
 
        public void Update(double v, string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                    {
                        // v from 0 to 1
                        //this.progress_bar.Value = (int) v * 100;
                        //Console.WriteLine("Thread id: " + Thread.CurrentThread.ManagedThreadId);
                        //this.debugBox.AppendText(message + "\n");
                    }));
            }
        }
        
        public MainForm()
        {
            InitializeComponent();

            //this.tableLayoutPanel1.Width = this.Width;
            //this.tableLayoutPanel1.Height = this.Height / 2;
            this.hotkeys_form = new SetupHotkeysForm(this.Handle);

            this.UpdateProgress += Update;

            this.connections = new Dictionary<Int32, ClientConnectionHandler>();
            this.current = null;
            this.target = null;

            this.hotkeys_handler = new HotkeysHandler(this.Handle);
            
            this.hook = new GlobalHook();
            this.hook.Stop();
            
            // keyboard events
            this.hook.KeyDown += this.OnKeyDown;
            this.hook.KeyUp += this.OnKeyUp;
            this.hook.KeyPress += this.OnKeyPress;

            // mouse events
            this.hook.MouseDown += this.OnMouseDown;
            this.hook.MouseUp += this.OnMouseUp;
            this.hook.MouseWheel += this.OnMouseWheel;
            this.hook.MouseMove += this.OnMouseMove;

        }

        private void chiudiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            serverAddrBox.Text = string.Empty;
            port.Text = string.Empty;
            pwd1.Text = string.Empty;
             * */
        }

        private void SelectServer()
        {
            byte[] bytes;
            Int32 lookup_key = -1;

            try
            {
                // Retrieve the lookup key from listview
                lookup_key = Convert.ToInt32(listView.FocusedItem.SubItems[0].Text);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Non è stato selezionato alcun server dalla lista.");
                
                return;
            }

            connections.TryGetValue(lookup_key, out current);
            
                // If I am controlling one server, I MUST pause it.
                if (target != null)
                {
                    MessageBox.Show("Invio richiesta di PAUSE.");
                    current.Send(Encoding.ASCII.GetBytes(MyProtocol.PAUSE));

                    bytes = current.Receive(MyProtocol.POSITIVE_ACK.Length);
                    string msg = Encoding.ASCII.GetString(bytes);
                    if (msg == MyProtocol.POSITIVE_ACK)
                    {
                        hook.Stop();

                        target = null;
                    }
                }

                List<int> keys = connections.Keys.ToList();
                foreach(int key in keys)
                {
                    listView.Items[key].SubItems[3].Text = "Pause.";
                }

                MessageBox.Show("Invio richiesta di target.");
                // The client sends a TARGET request to the server
                current.Send(Encoding.ASCII.GetBytes(MyProtocol.TARGET));

                bytes = current.Receive(MyProtocol.POSITIVE_ACK.Length);
                string mess = Encoding.ASCII.GetString(bytes);
                if(mess == MyProtocol.POSITIVE_ACK)
                {
                    listView.Items[lookup_key].SubItems[3].Text = "Target";
                    target = current;
                    hook.Start();
                }
        
            
        }


        private void SuspendServer()
        {
            if (target == null)
            {
                MessageBox.Show("Non hai nessuno da mettere in pausa.");
                return;
            }


                MessageBox.Show("Invio richiesta di PAUSE.");
                // The client sends a PAUSE request to the server
                target.Send(Encoding.ASCII.GetBytes(MyProtocol.PAUSE));

                byte[] bytes = current.Receive(MyProtocol.POSITIVE_ACK.Length);
                string msg = Encoding.ASCII.GetString(bytes);
                if (msg == MyProtocol.POSITIVE_ACK)
                {
                    foreach(int key in connections.Keys.ToList())
                    {
                        listView.Items[key].SubItems[3].Text = "Pausa.";
                    }
                    
                    hook.Stop();

                    target = null;
                }

            }
        

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            Int32 server_id = -1;

            current = null;

            try
            {
                // Retrieve the lookup key from listview
                server_id = Convert.ToInt32(listView.FocusedItem.SubItems[0].Text);
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Seleziona un server.");
                return;
            }

            if (server_id != -1)
            {
                ClientConnectionHandler client;
                
                if(connections.TryGetValue(server_id, out client))
                {
                    Task<bool> t = Task.Factory.StartNew(() => client.Close());

                    t.Wait();

                    if (t.Result)
                    {
                        connections.Remove(server_id);
                        listView.FocusedItem.Remove();
                    }
                }
                
            }
}


        public void Run_Client(Socket tcpChannel)
        {

            //Il client chiede al server se hai dati nella clipboard
            Functions.SendData(tcpChannel, Encoding.ASCII.GetBytes(MyProtocol.CLIENT + MyProtocol.END_OF_MESSAGE), 0, (MyProtocol.CLIENT + MyProtocol.END_OF_MESSAGE).Length);
            //In caso di risposta negativa, invia un negative ack al client: in questo caso la procedura termina.
            //Diversamente...
            //Ricevo il comando inviatomi dal server
            string recvbuf = Functions.ReceiveTillTerminator(tcpChannel);
            string command = recvbuf.Substring(0, 4);

            switch (command)
            {
                case MyProtocol.CLEAN:
                    Functions.CleanClipboardDir(Path.GetFullPath(MyProtocol.CLIPBOARD_DIR));
                    break;

                case MyProtocol.NEGATIVE_ACK:
                    MessageBox.Show("Nessun dato della Clipboard da inviare!");

                    break;

                case MyProtocol.COPY:
                    string content;
                    int len = recvbuf.Length - MyProtocol.END_OF_MESSAGE.Length - MyProtocol.COPY.Length;
                    content = recvbuf.Substring(MyProtocol.COPY.Length, len);
                    Clipboard.SetData(DataFormats.Text, content);
                    break;

                case MyProtocol.FILE_SEND:
                    // Simulo un po' di ritardo di rete
                    Thread.Sleep(1000);
                    // Now send ack
                    try
                    {
                        tcpChannel.Send(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }

                    Functions.handleFileDrop(tcpChannel, null);
                    Functions.StartClipoardUpdaterThread();
                    break;

                case MyProtocol.DIRE_SEND:
                    // Simulo un po' di ritardo di rete
                    Thread.Sleep(1000);

                    Functions.ReceiveDirectory(tcpChannel);
                    Functions.StartClipoardUpdaterThread();
                    break;

                case MyProtocol.IMG:

                    Functions.SendData(tcpChannel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                    byte[] length = Functions.ReceiveData(tcpChannel, sizeof(Int32));

                    Int32 length_int32 = BitConverter.ToInt32(length, 0);

                    Functions.SendData(tcpChannel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                    byte[] imageSource = Functions.ReceiveData(tcpChannel, length_int32);

                    Image image = Functions.ConvertByteArrayToBitmap(imageSource);
                    Clipboard.SetImage(image);

                    break;

                default:
                    MessageBox.Show("Comando da tastiera non riconosciuto");
                    break;
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            this.hook.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.hook.Stop();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            /*
            this.progress_bar.Width = (int)(this.Width * 0.7);
            this.progress_bar.Height = (int)(this.Height * 0.02);
            
            this.progress_bar.Dock = DockStyle.Bottom;*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            int id = 0;     // The id of the hotkey. 
            Keys key = (Keys)this.comboBox1.SelectedItem;

            if(hotkeys_handler.Register(id, (int)KeyModifier.Control, key.GetHashCode()))
                MessageBox.Show("Hotkey impostata!");
             * */
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*
            int id = 1;     // The id of the hotkey. 
            Keys key = (Keys)this.comboBox2.SelectedItem;

            if (hotkeys_handler.Register(id, (int)KeyModifier.Control, key.GetHashCode()))
                MessageBox.Show("Hotkey impostata!");
             * */
        }

        private void ImportClipboard()
        {
            if (target != null) {
           
                Functions.SendData(target.clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.CLIPBOARD_IMPORT + MyProtocol.END_OF_MESSAGE), 0, (MyProtocol.CLIPBOARD_IMPORT + MyProtocol.END_OF_MESSAGE).Length);
               
                string recvbuf = Functions.ReceiveTillTerminator(target.clipbd_channel);
                string command = recvbuf.Substring(0, 4);
                Console.WriteLine(this.GetType().Name + " - ricevuto comando: " + recvbuf);

                switch (command)
                {
                    case MyProtocol.CLEAN:
                        // Now, I'm starting to clean my clipboard folder as you told me.
                        Functions.CleanClipboardDir(Path.GetFullPath(MyProtocol.CLIPBOARD_DIR));

                        break;
                    
                    case MyProtocol.COPY:
                        string content;
                        int len = recvbuf.Length - MyProtocol.COPY.Length;
                        content = recvbuf.Substring(MyProtocol.COPY.Length, len);
                        //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                        Clipboard.SetData(DataFormats.Text, content);

                        MessageBox.Show("Ricevuto un Testo dalla clipboard del client.");
                        break;
                
                    case MyProtocol.FILE_SEND:
                        // Simulo un po' di ritardo di rete
                        Thread.Sleep(1000);
                        // Now send ack
                        try
                        {
                            target.clipbd_channel.Send(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return;
                        }

                        Functions.handleFileDrop(target.clipbd_channel, null);
                        Functions.StartClipoardUpdaterThread();

                        MessageBox.Show("Ricevuto un File dalla clipboard del client.");
                        break;

                    case MyProtocol.DIRE_SEND:
                        // Simulo un po' di ritardo di rete
                        Thread.Sleep(1000);

                        Functions.ReceiveDirectory(target.clipbd_channel);
                        Functions.StartClipoardUpdaterThread();
                        break;

                    case MyProtocol.IMG:
                        Functions.SendData(target.clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                        byte[] length = Functions.ReceiveData(target.clipbd_channel, sizeof(Int32));

                        Int32 length_int32 = BitConverter.ToInt32(length, 0);

                        Functions.SendData(target.clipbd_channel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                        byte[] imageSource = Functions.ReceiveData(target.clipbd_channel, length_int32);

                        Image image = Functions.ConvertByteArrayToBitmap(imageSource);
                        Clipboard.SetImage(image);

                        MessageBox.Show("Ricevuta Immagine dalla clipboard del client.");

                        break;

                    case MyProtocol.NEGATIVE_ACK:
                        //debugBox.AppendText("Nessun dato negli appunti del sistema remoto \n");
                        break;


                    default:
                        MessageBox.Show("Comando da tastiera non riconosciuto");
                        break;
                }
            }




        }

        private void ExportClipboard()
        {
            if (target != null)
            {
                Thread trd = new Thread(() => Functions.handleClipboardData(target));
                trd.SetApartmentState(ApartmentState.STA);
                trd.Start();
            }
        }


        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void hotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hotkeys_form.ShowDialog();
        }

        private void nuovaConnessioneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateConnectionForm f = new CreateConnectionForm();
            f.new_connection += this.NewConnection;
            f.ShowDialog();
        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }


    }

}
