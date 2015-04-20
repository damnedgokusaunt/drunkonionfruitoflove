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
using System.IO;
using System.Collections.Specialized;


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
        private TcpClient tcp;



        public void setLocalEndPoint(IPEndPoint ep)
        {
            this.ep = ep;
        }

        public static byte[] ReceiveData(ref Socket sock, int size)
        {
            int bytesRead = 0;
            int offset = 0;
            int count = size;
            byte[] message = new byte[size];
            
            do
            {
                try
                {
                    bytesRead = sock.Receive(message, offset, count, SocketFlags.None);
                    //Console.WriteLine("I read: " + bytesRead + " byte");
                }
                catch (Exception e)
                {
                    throw e;
                }

                if (bytesRead == 0) 
                    throw new IOException();

                count -= bytesRead;
                offset += bytesRead;
            } while (count != 0);

            return message;
        }

        private void receiveFile(ref Socket sock, string fileName, Int64 fileSize)
        {
            //clipFile cd;
            Int32 chunks, chunkSize, resto;

            //FILE TRANSFER           
            FileStream output = File.Create(fileName);

            Console.WriteLine("creato file: " + fileName);

            BinaryWriter binWriter = new BinaryWriter(output);

            // read the file in chunks of 1MB max
            byte[] buffer;

            chunks = Convert.ToInt32(fileSize / MyProtocol.CHUNK_SIZE);
            chunkSize = 0;
            Console.Write("start to transfer a file\nFile name: " + fileName + "\n File size: " + fileSize);
            do
            {
                if (chunks > 0)
                {
                    chunkSize = MyProtocol.CHUNK_SIZE;
                }
                else
                {
                    resto = Convert.ToInt32(fileSize % MyProtocol.CHUNK_SIZE);
                    if (resto > 0)
                    {
                        chunkSize = resto;
                    }
                    else
                    {
                        break;
                    }
                }

                chunks--;

                try
                {
                    buffer = ReceiveData(ref sock, chunkSize);
                }
                catch (Exception e)
                {
                    output.Close();
                    throw e;
                }

                //Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, chunkSize));
                Console.WriteLine("start to write " + chunkSize); 
                try
                {
                    binWriter.Write(buffer, 0, chunkSize);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception!");
                    output.Close();
                    throw e;
                }

                Console.WriteLine("written a chunck. remain "+chunks+ " to write"); 
            } while (chunks >= 0);

            Console.WriteLine("Done!");
            output.Close();
        }

        private void handleFileDrop(ref Socket sock)
        {
            string fileName = null;

            StringCollection FileCollection = new StringCollection();

            byte[] buffer = null;

            // Ricezione della dimensione del file
            Console.WriteLine("Aspetto di ricevere dim del file...");
            try
            {
                buffer = ReceiveData(ref sock, sizeof(Int64));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            Int64 fileSize = BitConverter.ToInt64(buffer, 0);

            // Ricezione del nome del file
            
            Console.WriteLine("Aspetto di ricevere nome del file...");
            try
            {
                string recvbuf = null;
                int bytesRec;
                byte[] bytes = new byte[1];

                do
                {
                    bytesRec = sock.Receive(bytes);
                    recvbuf += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                }
                while (recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE) == -1);

                Console.WriteLine("(da nome file) ho ricevuto: " + recvbuf);
                fileName = recvbuf.Substring(0, recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE));
                Console.WriteLine("Nome del file: " + fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            Console.WriteLine("Inizio trasferimento file: " + fileName);

            receiveFile(ref sock, fileName, fileSize);
            Console.WriteLine("Fine trasferimento file: " + fileName);

            #region prova clipboard
            //prova clipboard
            List<string> paths = new List<string>();

            //Application.StartupPath + slash + fileName;
            string path_completo = Path.GetFullPath(fileName);


            if (File.Exists(path_completo))
            {
                MessageBox.Show("Esiste!");

                paths.Add(path_completo);

                Clipboard.Clear();
                Clipboard.SetData(DataFormats.FileDrop, paths.ToArray());
            }
            else
                MessageBox.Show("Non esiste!");
            #endregion


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
                    case MyProtocol.RTF:
                        int lun = recvbuf.Length - MyProtocol.END_OF_MESSAGE.Length - MyProtocol.RTF.Length; 
                        content = recvbuf.Substring(MyProtocol.RTF.Length,lun);
                        //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                        Clipboard.SetData(DataFormats.Rtf, content);
                        break;
                    case MyProtocol.FILE_SEND:
                        Console.WriteLine("Ricevuta richiesta: " + MyProtocol.FILE_SEND);
                        tcpChannel.Send(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK));

                        int lung = recvbuf.Length - MyProtocol.END_OF_MESSAGE.Length - MyProtocol.FILE_SEND.Length; 
                        content = recvbuf.Substring(MyProtocol.FILE_SEND.Length, lung);
                        handleFileDrop(ref tcpChannel);
                        //Clipboard.SetData(DataFormats.FileDrop, content);
                        break;
           
                    default:
                        MessageBox.Show("Comando da tastiera non riconosciuto");
                        break;
                }            
            }
        }
    }

}
