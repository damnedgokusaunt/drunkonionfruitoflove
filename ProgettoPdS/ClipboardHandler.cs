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
using System.Threading;

namespace MyProject
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


        private string ReceiveTillTerminator(ref Socket sock)
        {
            int bytesRec;
            byte[] bytes = new byte[1];
            string recvbuf = null, cmd = null;
            do
            {
                bytesRec = sock.Receive(bytes);
                recvbuf += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                //Console.WriteLine("recvbuf: " + recvbuf);
            }
            while (recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE) == -1);
            
            cmd = recvbuf.Substring(0, recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE));
            
            return cmd;
        }
        public void setLocalEndPoint(IPEndPoint ep)
        {
            this.ep = ep;
        }

        #region Core methods

        public void SendData(ref Socket soc, byte[] buffer, int offset, int length)
        {
            NetworkStream clientStream;
            clientStream = new NetworkStream(soc);

            if (clientStream == null)
            {
                return;
            }

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

        public byte[] ReceiveData(ref Socket sock, int size)
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

            string path = fileName;

            //FILE TRANSFER           
            FileStream output = File.Create(path);

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
                //Console.WriteLine("start to write " + chunkSize); 
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

        private void UpdateClipboard()
        {
            List<string> paths = new List<string>();
            string dir = MyProtocol.CLIPBOARD_DIR;

            foreach (string f in Directory.GetFiles(dir))
            {
                paths.Add(Path.GetFullPath(f));
            }
            foreach (string d in Directory.GetDirectories(dir))
            {
                paths.Add(Path.GetFullPath(d));
            }

            Clipboard.SetData(DataFormats.FileDrop, paths.ToArray());
        }

        private void StartClipoardUpdaterThread()
        {
            Thread t = new Thread(UpdateClipboard);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private void CleanClipboardDir(string dir)
        {
            // Prima elimino tutti i file all'interno della directory
            foreach (string f in Directory.GetFiles(dir))
            {
                File.Delete(f);
            }
            
            foreach (string d in Directory.GetDirectories(dir))
            {
                // Chiamata ricorsiva: elimino tutti i file di tutte le sue sottocartelle
                CleanClipboardDir(d);
                // Elimino le directory in modo bottom-up
                Directory.Delete(d);
            }
        }

        private void handleFileDrop(ref Socket sock, string baseDir)
        {
            string fileName = null;
            StringCollection FileCollection = new StringCollection();
            byte[] buffer = null;


            Console.WriteLine("Inizio ricezione di un file.");
           
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

                if (baseDir != null)
                {
                    fileName = baseDir + Path.DirectorySeparatorChar + fileName;
                }
                else
                {
                    fileName = MyProtocol.CLIPBOARD_DIR + Path.DirectorySeparatorChar + fileName;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine("Inizio trasferimento file: " + fileName);

            receiveFile(ref sock, fileName, fileSize);
            Console.WriteLine("Fine trasferimento file: " + fileName);
        }

        private void ReceiveSubDirectory(ref Socket sock, string basedir)
        {
            //byte[] msg;
            string cmd = String.Empty;
            //Int32 dirNameSize;
            string dirName, newBaseDir, fileName;
            
            do
            {
                cmd = ReceiveTillTerminator(ref sock);

                switch(cmd)
                {
                    case MyProtocol.FILE_SEND:
                        try
                        {
                            sock.Send(Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return;
                        }
                        handleFileDrop(ref sock, basedir);
                        break;

                    case MyProtocol.DIRE_SEND:
                        // nome directory
                        dirName = ReceiveTillTerminator(ref sock);
                        dirName = System.IO.Path.GetInvalidFileNameChars().Aggregate(dirName, (current, c) => current.Replace(c.ToString(), string.Empty));
                        newBaseDir = System.IO.Path.Combine(basedir, dirName);

                        Directory.CreateDirectory(newBaseDir);

                        Console.WriteLine("Nuova cartella: " + newBaseDir);

                        ReceiveSubDirectory(ref sock, newBaseDir);
                        break;

                    case MyProtocol.END_OF_DIR:
                        Console.WriteLine("End of directory. up of a level");
                        break;

                    default:
                        Console.WriteLine("invalid message");
                        throw new Exception("Invalid message received from server while trying to transfer directory content.");
                }  
            } while (cmd != MyProtocol.END_OF_DIR);        
        }
                  
        public string ReceiveDirectory(ref Socket sock)
        {
            // Int32 dirNameSize;
            // byte[] msg = null;
            string basedir, myBaseDir;

            // Ricevo il nome della directory
            int bytesRec;
            byte[] bytes = new byte[1];
            string recvbuf = null;
            do
            {
                bytesRec = sock.Receive(bytes);
                recvbuf += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                //Console.WriteLine("recvbuf: " + recvbuf);
            }
            while (recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE) == -1);
            
            basedir = recvbuf.Substring(0, recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE));

            Console.WriteLine("Ricevuto nome dir: " + basedir);

            myBaseDir = Path.Combine(MyProtocol.CLIPBOARD_DIR, basedir);
            DirectoryInfo di = Directory.CreateDirectory(myBaseDir);
           

            ReceiveSubDirectory(ref sock, myBaseDir);

            return myBaseDir;
        } 
        
        #endregion

        #region Bitmap methods

        Bitmap ConvertByteArrayToBitmap(byte[] imageSource)
        {
            var imageConverter = new ImageConverter();
            var image = (Image)imageConverter.ConvertFrom(imageSource);

            return new Bitmap(image);
        }

        #endregion

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
                Console.WriteLine(this.GetType().Name + " - in attesa di comandi dal client...");

                string recvbuf = ReceiveTillTerminator(ref tcpChannel);
                string command = recvbuf.Substring(0, 4);
                Console.WriteLine(this.GetType().Name + " - ricevuto comando: " + recvbuf);

                switch (command)
                {
                    case MyProtocol.CLEAN:
                        // Now, I'm starting to clean my clipboard folder as you told me.
                        CleanClipboardDir(Path.GetFullPath(MyProtocol.CLIPBOARD_DIR));                    
                        
                        break;
                    case MyProtocol.COPY:
                        string content;
                        int len = recvbuf.Length - MyProtocol.END_OF_MESSAGE.Length - MyProtocol.COPY.Length; 
                        content = recvbuf.Substring(MyProtocol.COPY.Length,len);
                        //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                        Clipboard.SetData(DataFormats.Text, content);
                        break;
                    case MyProtocol.RTF:
                        int lun = recvbuf.Length - MyProtocol.END_OF_MESSAGE.Length - MyProtocol.RTF.Length; 
                        content = recvbuf.Substring(MyProtocol.RTF.Length,lun);
                        //Console.WriteLine("Tentativo di scrittura su clipboard: " + content);
                        Clipboard.SetData(DataFormats.Rtf, content);
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

                        handleFileDrop(ref tcpChannel, null);                       
                        StartClipoardUpdaterThread(); 
                        break;

                    case MyProtocol.DIRE_SEND: 
                        // Simulo un po' di ritardo di rete
                        Thread.Sleep(1000);
                        
                        ReceiveDirectory(ref tcpChannel);
                        StartClipoardUpdaterThread();                       
                        break;

                    case MyProtocol.IMG:

                        SendData(ref tcpChannel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                        byte[] length = ReceiveData(ref tcpChannel, sizeof(Int32));

                        /*
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(length);*/

                        Int32 length_int32 = BitConverter.ToInt32(length, 0);

                        SendData(ref tcpChannel, Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK), 0, MyProtocol.POSITIVE_ACK.Length);

                        byte[] imageSource = ReceiveData(ref tcpChannel, length_int32);

                        Image image = ConvertByteArrayToBitmap(imageSource);
                        Clipboard.SetImage(image);

                        break;

                    default:
                        MessageBox.Show("Comando da tastiera non riconosciuto");
                        break;
                }            
            }
        }
    }

}
