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
        [DllImport("user32.dll")]
        static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);

        private Dictionary<Int32, ClientConnectionHandler> connections;
        private ClientConnectionHandler current, target;
        private HotkeysHandler hotkeys_handler;
        private SetupHotkeysForm hotkeys_form;
        private Task clipboard_importer, clipboard_exporter, server_selecter, server_suspender;

        public GlobalHook hook;

        private object clipboardLock;

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
            if(m.Msg == 0x0112)
            {
                int command = m.WParam.ToInt32() & 0xfff0;
                if (command == 0xf010)
                    return;
            }

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
                        server_selecter = new Task(() => SelectServer());
                        server_selecter.Start();
                        break;
                    case 2:
                        server_suspender = new Task(() => SuspendServer());
                        server_suspender.Start();
                        break;
                    case 4:
                        clipboard_importer = new Task(() => ImportClipboard());
                        clipboard_importer.Start();
                        break;
                    case 6:
                        clipboard_exporter = new Task(() => ExportClipboard());
                        clipboard_exporter.Start();
                        break;
                    default:
                        throw new Exception("Hotkey non riconosciuta");
                }  
            }

            base.WndProc(ref m);
        }
        
        #endregion

        #region Keyboard methods
        private void OnKeyDown(object sender, byte vkCode, byte scanCode)
        {
            byte[] bytes = new byte[MyProtocol.KEYDOWN.Length + 2];
            Array.Copy(Encoding.ASCII.GetBytes(MyProtocol.KEYDOWN), bytes, MyProtocol.KEYDOWN.Length);
            Array.Copy(new byte[] { vkCode, scanCode }, 0, bytes, MyProtocol.KEYDOWN.Length, 2);

            if (target != null)
            {
                target.SendTCP(target.handler, bytes);
            }
        }
        private void OnKeyUp(object sender, byte vkCode, byte scanCode)
        {
            byte[] bytes = new byte[MyProtocol.KEYUP.Length + 2];
            Array.Copy(Encoding.ASCII.GetBytes(MyProtocol.KEYUP), bytes, MyProtocol.KEYUP.Length);
            Array.Copy(new byte[] { vkCode, scanCode }, 0, bytes, MyProtocol.KEYUP.Length, 2);

            if (target != null)
            {
                target.SendTCP(target.handler, bytes);
            }
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

        #region Clipboard methods

        private void HandleFileSend()
        {
            Functions.handleFileDrop(target.clipbd_channel, null);
            Functions.UpdateClipboard();
        }

        private void doImport()
        {
            bool exit = false;

            byte[] bytes = null;

            try
            {
                bytes = Encoding.ASCII.GetBytes(MyProtocol.CLIPBOARD_IMPORT + MyProtocol.END_OF_MESSAGE);
                Functions.SendClipboard(bytes, bytes.Length);

                while (!exit)
                {
                
                    string recvbuf = Functions.ReceiveTillTerminator(target.clipbd_channel);

                    string command = recvbuf.Substring(0, 4);
                    Console.WriteLine(this.GetType().Name + " - ricevuto comando: " + recvbuf);

                    switch (command)
                    {
                        case MyProtocol.CLEAN:
                            // Now, I'm starting to clean my clipboard folder as you told me.
                            Functions.CleanClipboardDir(Path.GetFullPath(MyProtocol.CLIPBOARD_DIR));
                            Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);
                            break;

                        case MyProtocol.COPY:
                            Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);

                            byte[] text_len_array = Functions.ReceiveClipboard(sizeof(Int32));
                            Int32 text_len = BitConverter.ToInt32(text_len_array, 0);

                            Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);

                            byte[] text_array = Functions.ReceiveClipboard(text_len);
                            string content = Encoding.Unicode.GetString(text_array);

                            //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                            Functions.clipboardSetData(DataFormats.Text, content);

                            MessageBox.Show("Ricevuto un Testo dalla clipboard del client.");

                            Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);
                            break;

                        case MyProtocol.FILE_SEND:
                            Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);
                            
                            HandleFileSend();
                            
                            //Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);
                            
                            break;

                        case MyProtocol.DIRE_SEND:
                            Functions.ReceiveDirectory(target.clipbd_channel);
                            Functions.UpdateClipboard();
                            //Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);
                            
                            break;

                        case MyProtocol.IMG:
                            bytes = Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK);
                            Functions.SendClipboard(bytes, bytes.Length);
                            byte[] length = Functions.ReceiveClipboard(sizeof(Int32));
                            Int32 length_int32 = BitConverter.ToInt32(length, 0);

                            bytes = Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK);
                            Functions.SendClipboard(bytes, bytes.Length);

                            byte[] imageSource = Functions.ReceiveData(target.clipbd_channel, length_int32);
                            Image image = Functions.ConvertByteArrayToBitmap(imageSource);
                            Functions.clipboardSetImage(image);
                            MessageBox.Show("Ricevuta Immagine dalla clipboard del client.");
                            Functions.SendClipboard(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), MyProtocol.POSITIVE_ACK.Length);
                           
                            break;

                        case MyProtocol.NEGATIVE_ACK:
                            exit = true;
                            break;

                        default:
                            MessageBox.Show("Comando clipboard non riconosciuto");
                            break;
                    }
                }
            }
            catch (SocketException)
            {
                throw;
            }
        }
  
        private void ImportClipboard()
        {
            lock (this)
            {
                if (target != null)
                {
                    try
                    {
                        doImport();
                    }
                    catch (SocketException)
                    {
                        keybd_event(0, 0, 0, 0);
                        keybd_event(0, 0, 2, 0);
                    }
                }
                else
                    MessageBox.Show("Nessun target impostato.");
            }       
        }

        private void ExportClipboard()
        {
            lock (this)
            {
                if (target != null)
                {
                    try
                    {
                        Functions.handleClipboardData();
                    }
                    catch (SocketException)
                    {
                        keybd_event(0, 0, 0, 0);
                        keybd_event(0, 0, 2, 0);
                    }
                } 
                else
                    MessageBox.Show("Nessun target impostato.");
            }
        }
        
        #endregion

        #region Progress handling methods
        public void UpdateProgressBar(UInt32 v)
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.BeginInvoke(new Action(() => this.progressBar1.Value = (int)v));
            }
        }

        public void UpdateLabel(string m)
        {
            if (this.info_label.InvokeRequired)
            {
                this.info_label.BeginInvoke(new Action(() => this.info_label.Text = m));
            }
        }
        #endregion

        public MainForm()
        {
            InitializeComponent();

            //this.progressBar1.Size.Width = (int) (this.Width * 0.7);

            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;

            Functions.getDataObject = delegate 
            {
                IDataObject idata = null;

                this.Invoke(new Action(() => idata = Clipboard.GetDataObject()));

                return idata;
            };

            Functions.clipboardContains = delegate(string format)
            {
                bool result = false;

                this.Invoke(new Action(() => result = Clipboard.ContainsData(format)));

                return result;
            };

            Functions.clipboardSetData = delegate(string format, object data)
            {
                this.BeginInvoke(new Action(() => Clipboard.SetData(format, data)));
            };

            Functions.clipboardGetImage = delegate
            {
                Image i = null;

                this.Invoke(new Action(() => i = Clipboard.GetImage()));

                return i;
            };

            Functions.clipboardSetImage = delegate(Image i)
            {
                this.BeginInvoke(new Action(() => Clipboard.SetImage(i)));
            };
                
           
            this.clipboardLock = new object();

            Application.ApplicationExit += this.HandleClientExit;

            //this.tableLayoutPanel1.Width = this.Width;
            //this.tableLayoutPanel1.Height = this.Height / 2;
            this.hotkeys_form = new SetupHotkeysForm(this.Handle);

            this.connections = new Dictionary<Int32, ClientConnectionHandler>();
            this.current = null;
            this.target = null;

            this.hotkeys_handler = new HotkeysHandler(this.Handle);

            this.hook = new GlobalHook();
            this.hook.Stop();

            // keyboard events
            this.hook.KeyDown += this.OnKeyDown;
            this.hook.KeyUp += this.OnKeyUp;

            // mouse events
            this.hook.MouseDown += this.OnMouseDown;
            this.hook.MouseUp += this.OnMouseUp;
            this.hook.MouseWheel += this.OnMouseWheel;
            this.hook.MouseMove += this.OnMouseMove;

        }

        private void HandleClientExit(object sender, EventArgs e)
        {
            // Controllo se i task sono terminati
            if (clipboard_importer != null)
            {
                if (!clipboard_importer.IsCompleted)
                    clipboard_importer.Wait();
            }

            if (clipboard_exporter != null)
            {
                if (!clipboard_exporter.IsCompleted)
                    clipboard_exporter.Wait();
            }

            if (server_selecter != null)
            {
                if (!server_selecter.IsCompleted)
                    server_selecter.Wait();
            }

            if (server_suspender != null)
            {
                if (!server_suspender.IsCompleted)
                    server_suspender.Wait();
            }

            if(connections.Count > 0)
            {
                foreach (KeyValuePair<Int32, ClientConnectionHandler> pair in connections)
                    pair.Value.Close();

                connections.Clear();
            }
        }

        public void NewConnection(string ip_address, string port, string password)
        {
            try
            {
                // Istanzia oggetto client
                ClientConnectionHandler new_connection = new ClientConnectionHandler(this, IPAddress.Parse(ip_address), Convert.ToInt32(port), Functions.Encrypt(password));

                new_connection.remove_target = this.removeTarget;

                Functions.update_label = this.UpdateLabel;
                Functions.update_progressbar = this.UpdateProgressBar;

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
            string msg;
            byte[] bytes;
            Int32 lookup_key = -1;

            lock (this)
            {
                // Retrieve the lookup key from listview
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        lookup_key = Convert.ToInt32(listView.FocusedItem.SubItems[0].Text);
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Non è stato selezionato alcun server dalla lista.");
                        return;
                    }
                }));
                if (lookup_key == -1)
                    return;

                // If I am controlling one server, I MUST pause it.
                if (target != null)
                {
                    target.SendTCP(target.handler, Encoding.ASCII.GetBytes(MyProtocol.PAUSE));

                    bytes = target.ReceiveTCP(target.handler, MyProtocol.POSITIVE_ACK.Length);
                    if (bytes == null)
                        return;

                    msg = Encoding.ASCII.GetString(bytes);
                    if (msg == MyProtocol.POSITIVE_ACK)
                    {
                        this.Invoke(new Action(() =>
                        {
                            hook.Stop();
                            this.target = null;
                        }));
                    }
                }

                List<int> keys = connections.Keys.ToList();
                foreach (int key in keys)
                {
                    this.Invoke(new Action(() => listView.Items[key].SubItems[3].Text = "Pause."));
                }

                connections.TryGetValue(lookup_key, out current);


                // The client sends a TARGET request to the server
                current.SendTCP(current.handler, Encoding.ASCII.GetBytes(MyProtocol.TARGET));

                bytes = current.ReceiveTCP(current.handler, MyProtocol.POSITIVE_ACK.Length);
                if (bytes == null)
                    return;

                msg = Encoding.ASCII.GetString(bytes);
                if (msg == MyProtocol.POSITIVE_ACK)
                {
                    this.Invoke(new Action(() => listView.Items[lookup_key].SubItems[3].Text = "Target"));
                    target = current;
                    Functions.SendClipboard = target.SendClipboard;
                    Functions.ReceiveClipboard = target.ReceiveClipboard;
                    this.Invoke(new Action(() => hook.Start()));
                }
                //MessageBox.Show("Thread Select terminato");
            }
        }

        private void SuspendServer()
        {
            byte[] bytes;
            string msg;

            lock (this)
            {
                if (target == null)
                {
                    MessageBox.Show("Non hai nessuno da mettere in pausa.");
                    return;
                }

                // The client sends a PAUSE request to the server
                target.SendTCP(target.handler, Encoding.ASCII.GetBytes(MyProtocol.PAUSE));

                bytes = current.ReceiveTCP(current.handler, MyProtocol.POSITIVE_ACK.Length);
                msg = Encoding.ASCII.GetString(bytes);
                if (msg == MyProtocol.POSITIVE_ACK)
                {
                    this.Invoke(new Action(() =>
                    {
                        foreach (int key in connections.Keys.ToList())
                            listView.Items[key].SubItems[3].Text = "Pausa.";

                        hook.Stop();
                        target = null;
                    }));
                }
            }
            //MessageBox.Show("Thread Suspend terminato");
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

        private void nuovaConnessioneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateConnectionForm f = new CreateConnectionForm();
            f.new_connection += this.NewConnection;
            f.ShowDialog();
        }

        private void importaAppuntiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            server_selecter = new Task(() => SelectServer());
            server_selecter.Start();
        }

        public void removeTarget()
        {
            Int32 key = -1;

            if (connections.ContainsValue(target))
            {
                foreach (KeyValuePair<Int32, ClientConnectionHandler> pair in connections)
                {
                    if (pair.Value.Equals(target))
                    {
                        key = pair.Key;
                        break;
                    }
                }
            }

            if (key != -1)
            {
                connections.Remove(key); // Remove from Dictionary
                listView.Items[key].Remove();    // Remove from ListView
                target = null;
            }
        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void importaAppuntiToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            clipboard_importer = new Task(() => ImportClipboard());
            clipboard_importer.Start();
        }

        private void esportaAppuntiVersoIlServerRemotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clipboard_exporter = new Task(() => ExportClipboard());
            clipboard_exporter.Start();
        }

        private void esportaAppuntiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            server_suspender = new Task(() => SuspendServer());
            server_suspender.Start();
        }

        private void hotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.hotkeys_form.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(MyProtocol.CLIPBOARD_DIR))
                Directory.CreateDirectory(MyProtocol.CLIPBOARD_DIR);
        }

        private void comandiToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }

}
