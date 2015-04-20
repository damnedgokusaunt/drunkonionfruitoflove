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
using System.IO;

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
        private MainForm  window;

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
        

        //ascia

        public void SendData(ref Socket soc,byte[] buffer, int offset, int length)
        {
            NetworkStream clientStream;
            clientStream = new NetworkStream(soc);
           
            if (clientStream == null)
            {
                return;
            }

          //  clientStream = clipbdChannel.GetStream();

            try
            {
                clientStream.Write(buffer, offset, length);

            }
            catch (Exception e)
            {
                Console.WriteLine("write exception " + e.Message);
                clientStream.Close();
                throw;
            }

            clientStream.Flush();

        }

        public void SendFile(FileStream fs, Int64 fileSize)
        {
            Int32 chunks, chunkSize;
            BinaryReader reader = new BinaryReader(fs);

            chunks = Convert.ToInt32(fileSize / MyProtocol.CHUNK_SIZE);
            chunkSize = 0;
            byte[] bufferFile = new byte[MyProtocol.CHUNK_SIZE];

            do
            {
                Console.WriteLine("Start!");
                if (chunks > 0)
                {
                    chunkSize = MyProtocol.CHUNK_SIZE;
                }
                else
                {
                    chunkSize = Convert.ToInt32(fileSize % MyProtocol.CHUNK_SIZE);

                    //if (chunkSize <= 0) break;
                }

                chunks--;

                reader.Read(bufferFile, 0, chunkSize);
                try
                {
                    SendData(ref clipbdChannel,bufferFile, 0, chunkSize);
                }
                catch
                {
                    fs.Close();
                    throw;
                }
            } while (chunks >= 0);

            Console.WriteLine("The end!");
        }

        private void ClipboardSendFile(string name)
        {
            FileInfo f;
            FileStream fs;
            byte[] buffer;
            int bufferSize;
            Int64 fileSize;

            string fileName;
            

            if (!File.Exists(name)) throw new Exception("File doesn't exist!");

            f = new FileInfo(name);
            fileSize = f.Length;
            fileName = f.Name;

            fs = File.Open(name, FileMode.Open);

            bufferSize = (13 + ASCIIEncoding.ASCII.GetBytes(fileName).Length);
            buffer = new byte[bufferSize];


            //string mess= MyProtocol.FILE_SEND;

            //message to start file transfer, (Message[1]=SEND_FILE, filesize[8], fileNameLength[4], filename[fileNameLength])
            Buffer.BlockCopy(BitConverter.GetBytes(fileSize), 0, buffer, 1, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(ASCIIEncoding.ASCII.GetBytes(fileName).Length), 0, buffer, 9, 4);
            Buffer.BlockCopy(ASCIIEncoding.ASCII.GetBytes(fileName), 0, buffer, 13, ASCIIEncoding.ASCII.GetBytes(fileName).Length);

            //sending protocol message

            SendData(ref clipbdChannel, BitConverter.GetBytes(fileSize), 0, BitConverter.GetBytes(fileSize).Length);
            SendData(ref clipbdChannel, ASCIIEncoding.ASCII.GetBytes(fileName + MyProtocol.END_OF_MESSAGE), 0, ASCIIEncoding.ASCII.GetBytes(fileName + MyProtocol.END_OF_MESSAGE).Length);
            Console.WriteLine("Sto inviando il file: " + fileName);
            

            Console.WriteLine("start file trasfering: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            
            SendFile(fs, fileSize);
            Console.WriteLine("end file trasfering: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

            fs.Close();

            Console.WriteLine("done.");
        }



        void handleClipboardData()
        {
            try
            {
                string clipbdMsg;
                
                
                //4_file
                object clipContent;
                string[] path;
                int n;
                byte[] buffer;
                string mess = MyProtocol.FILE_SEND;

                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();


                if (Clipboard.ContainsData(DataFormats.Text))
                {
                    clipbdMsg = MyProtocol.COPY + (string)iData.GetData(DataFormats.Text) + MyProtocol.END_OF_MESSAGE;
                    clipbdChannel.Send(Encoding.ASCII.GetBytes(clipbdMsg));
                }

                else if (Clipboard.ContainsData(DataFormats.Rtf))
                {
                    clipbdMsg = MyProtocol.RTF + (string)iData.GetData(DataFormats.Rtf) + MyProtocol.END_OF_MESSAGE;
                    clipbdChannel.Send(Encoding.ASCII.GetBytes(clipbdMsg));
                }
                else if (Clipboard.ContainsData(DataFormats.FileDrop))
                {
                    
                            //operazioni invio file
                    buffer = new byte[1];
                    try
                    {
                        Console.WriteLine("preparazione invio file...");
                        SendData(ref clipbdChannel, Encoding.ASCII.GetBytes(mess + MyProtocol.END_OF_MESSAGE), 0, Encoding.ASCII.GetBytes(mess + MyProtocol.END_OF_MESSAGE).Length);
                        Console.WriteLine("SENDATA eseguita!");
                     
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    byte[] bytes = new byte[MyProtocol.POSITIVE_ACK.Length];
                    clipbdChannel.Receive(bytes);

                    Console.WriteLine("Il client è pronto a ricevere il file.");

                    IDataObject data = Clipboard.GetDataObject();    
                    clipContent = data.GetData(DataFormats.FileDrop);
                    path = (string[])clipContent;
                    n = path.Length;

                    for (int i = 0; i < n; i++)
                    {
                        if (File.Exists(path[i]))
                        {
                            try
                            {
                                ClipboardSendFile(path[i]);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }
                        }
                        else if (Directory.Exists(path[i]))
                        {
                            try
                            {
                                Console.WriteLine("start directory trasfering: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                              //  ClipboardRecursiveDirectorySend(path[i]);
                                Console.WriteLine("end file trasfering: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                                Console.WriteLine("done.");
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Isn't a File or a Directory!!!");
                        }
                    }

                    string mess1 = MyProtocol.END_OF_MESSAGE;
  
                    try
                    {
                        SendData(ref clipbdChannel, Encoding.ASCII.GetBytes(mess1), 0, Encoding.ASCII.GetBytes(mess1).Length);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }

                    Console.WriteLine("done.");

                    //Close();            
                } 
            
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

    }
}
