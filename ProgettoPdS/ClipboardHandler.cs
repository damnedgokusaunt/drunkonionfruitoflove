using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;


namespace ProgettoPdS
{
    class ClipboardHandler
    {
        [DllImport("User32.dll")]
		protected static extern int SetClipboardViewer(int hWndNewViewer);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

		//private System.Windows.Forms.RichTextBox richTextBox1;

		IntPtr nextClipboardViewer;
        private IPEndPoint ep;
		private System.ComponentModel.Container components = null;



        public void setLocalEndPoint(IPEndPoint ep)
        {
            this.ep = ep;
        }


        public void Run()
        {
            //connessione TCP
            //byte[] data = new byte[sizeof(char)* 10];
            Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ep);
            listener.Listen(1000);
            Socket tcpChannel = listener.Accept();
            listener.Close();

             
            //parte operativa

            while (true)
            {

                int bytesRec;
                byte[] bytes = new byte[1024];
                string recvbuf = null;
                do
                {
                    bytesRec = tcpChannel.Receive(bytes);
                    recvbuf += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    //Console.WriteLine("recvbuf: " + recvbuf);
                }
                while (recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE) == -1);

                Console.WriteLine("Ricevuto: " + recvbuf);

                string command = recvbuf.Substring(0, 4); 
                
                switch (command)
                {
                    case MyProtocol.COPY:
                        string content;
                        int len = recvbuf.Length - MyProtocol.END_OF_MESSAGE.Length - MyProtocol.COPY.Length; 
                        content = recvbuf.Substring(MyProtocol.COPY.Length,len);
                        Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                        Clipboard.SetData(DataFormats.Text, content);
                        break;
                    default:
                        MessageBox.Show("Comando da tastiera non riconosciuto");
                        break;
                }            
            }
        }
    }

}
