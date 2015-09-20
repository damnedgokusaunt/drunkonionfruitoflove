using System.Threading;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Diagnostics;


namespace MyProject
{
    static class Functions
    {

        [DllImport("user32.dll")]
        static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        
        private static Socket clipbdChannel;


        #region Connection
        public static string Encrypt(string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, source);
            }
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        #endregion

        #region Keyboard handling methods
        public static void KeyUp(byte key)
        {
            keybd_event(key, 0, 2, 0);
        }

        public static void KeyDown(byte key)
        {
            keybd_event(key, 0, 0, 0);
           // MessageBox.Show("Keydown: "+ Process.GetCurrentProcess().Id);
            
        }
        #endregion

        #region Mouse handling methods

        // Flags for mouse_event api
        [Flags]
        public enum MouseEventFlagsAPI
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x800
        }

        public static void doMouseMove(byte[] bytes, Int32 widthRatio, Int32 heightRatio)
        {
            Int32 x = BitConverter.ToInt32(bytes, 0) * widthRatio;
            Int32 y = BitConverter.ToInt32(bytes, sizeof(int)) * heightRatio;

            Cursor.Position = new Point(x, y);
        }
        public static void doMouseRightClick()
        {
            // Send click to system
            mouse_event((int)MouseEventFlagsAPI.RIGHTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.RIGHTUP, 0, 0, 0, 0);
        }
        public static void doMouseLeftClick()
        {
            // Send click to system
            // mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.LEFTUP, 0, 0, 0, 0);
        }
        public static void doMouseWheel(int delta)
        {
            //Console.WriteLine("Server esegue wheel");

            mouse_event((int)MouseEventFlagsAPI.WHEEL, 0, 0, delta, 0);
        }
        public static void doMouseDragAndDrop()
        {
            //mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
        }
        #endregion

        #region Clipboard methods
        public static void SendFile(FileStream fs, Int64 fileSize)
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

                }

                chunks--;

                reader.Read(bufferFile, 0, chunkSize);
                try
                {
                    SendData(ref clipbdChannel, bufferFile, 0, chunkSize);
                }
                catch
                {
                    fs.Close();
                    throw;
                }
            } while (chunks >= 0);

            Console.WriteLine("The end!");
        }

        public static void ClipboardSendFile(string name)
        {
            FileInfo f;
            FileStream fs;
            Int64 fileSize;

            string fileName;
            string mess = MyProtocol.FILE_SEND;


            if (!File.Exists(name)) throw new Exception("File doesn't exist!");

            f = new FileInfo(name);
            fileSize = f.Length;
            fileName = f.Name;

            fs = File.Open(name, FileMode.Open);

            SendData(ref clipbdChannel, Encoding.ASCII.GetBytes(mess + MyProtocol.END_OF_MESSAGE), 0, Encoding.ASCII.GetBytes(mess + MyProtocol.END_OF_MESSAGE).Length);
            Console.WriteLine("Comando invio_file inviato!");


            SendData(ref clipbdChannel, BitConverter.GetBytes(fileSize), 0, BitConverter.GetBytes(fileSize).Length);
            Console.WriteLine("FileSize inviata!");

            SendData(ref clipbdChannel, ASCIIEncoding.ASCII.GetBytes(fileName + MyProtocol.END_OF_MESSAGE), 0, ASCIIEncoding.ASCII.GetBytes(fileName + MyProtocol.END_OF_MESSAGE).Length);
            Console.WriteLine("Sto inviando il file: " + fileName);

            SendFile(fs, fileSize);
            fs.Close();

            Console.WriteLine("Done.");
        }

        public static void ClipboardRecursiveDirectorySend(string sDir)
        {
            DirectoryInfo dInfo;
            string dirName;
            byte[] buffer;
            int bufferSize;

            if (!Directory.Exists(sDir)) throw new Exception("Directory doesn't exists!");

            dInfo = new DirectoryInfo(sDir);
            dirName = dInfo.Name;

            bufferSize = (1 + 4 + ASCIIEncoding.ASCII.GetBytes(dirName).Length);
            buffer = new byte[bufferSize];

            string com = MyProtocol.DIRE_SEND;


            SendData(ref clipbdChannel, ASCIIEncoding.ASCII.GetBytes(com + MyProtocol.END_OF_MESSAGE), 0, ASCIIEncoding.ASCII.GetBytes(com + MyProtocol.END_OF_MESSAGE).Length);
            SendData(ref clipbdChannel, ASCIIEncoding.ASCII.GetBytes(dirName + MyProtocol.END_OF_MESSAGE), 0, ASCIIEncoding.ASCII.GetBytes(dirName + MyProtocol.END_OF_MESSAGE).Length);

            foreach (string f in Directory.GetFiles(sDir))
            {
                ClipboardSendFile(f);
            }

            foreach (string d in Directory.GetDirectories(sDir))
            {
                ClipboardRecursiveDirectorySend(d);
            }

            SendData(ref clipbdChannel, ASCIIEncoding.ASCII.GetBytes(MyProtocol.END_OF_DIR + MyProtocol.END_OF_MESSAGE), 0, ASCIIEncoding.ASCII.GetBytes(MyProtocol.END_OF_DIR + MyProtocol.END_OF_MESSAGE).Length);

        }

        public static void UpdateClipboard()
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

        public static void StartClipoardUpdaterThread()
        {
            Thread t = new Thread(UpdateClipboard);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        public static void CleanClipboardDir(string dir)
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

        //Funzione per la conversione da bitmap obj a byte array
        public static byte[] ConvertBitmapToByteArray(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static void handleClipboardData(ref Socket clipbdChannel)
        {
            try
            {
                string clipbdMsg;


                //Dichiarazioni utili per i file
                object clipContent;
                string[] path;
                int n;
                byte[] buffer;


                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();


                if (Clipboard.ContainsData(DataFormats.Text))
                {
                    clipbdMsg = MyProtocol.COPY + (string)iData.GetData(DataFormats.Text) + MyProtocol.END_OF_MESSAGE;
                    clipbdChannel.Send(Encoding.ASCII.GetBytes(clipbdMsg));
                }


                else if (Clipboard.ContainsData(DataFormats.Bitmap))
                {

                    //invio comando bitmap + terminatore
                    SendData(ref clipbdChannel, Encoding.ASCII.GetBytes(MyProtocol.IMG + MyProtocol.END_OF_MESSAGE), 0, Encoding.ASCII.GetBytes(MyProtocol.IMG + MyProtocol.END_OF_MESSAGE).Length);
                    //attendo ack di ricezione
                    ReceiveData(ref clipbdChannel, MyProtocol.POSITIVE_ACK.Length);
                    //preparazione invio dati bitmap
                    Image img = Clipboard.GetImage();
                    //converto da bitmap obj a byte array
                    byte[] res = ConvertBitmapToByteArray(img);
                    //invio la dimensione dei dati
                    Int32 dim = res.Length;
                    SendData(ref clipbdChannel, BitConverter.GetBytes(dim), 0, sizeof(Int32));
                    //attendo ack di ricezione
                    ReceiveData(ref clipbdChannel, MyProtocol.POSITIVE_ACK.Length);
                    //invio dati
                    clipbdChannel.Send(res);

                }

                else if (Clipboard.ContainsData(DataFormats.FileDrop))
                {
                    //Inizio operazioni di invio file

                    buffer = new byte[1];
                    try
                    {

                        SendData(ref clipbdChannel, Encoding.ASCII.GetBytes(MyProtocol.CLEAN + MyProtocol.END_OF_MESSAGE), 0, Encoding.ASCII.GetBytes(MyProtocol.CLEAN + MyProtocol.END_OF_MESSAGE).Length);
                        Console.WriteLine("Clean!");
                        Console.WriteLine("preparazione invio file...");

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }

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
                                ClipboardRecursiveDirectorySend(path[i]);
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

                    Console.WriteLine("done.");

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                SendData(ref clipbdChannel, Encoding.ASCII.GetBytes(MyProtocol.NEGATIVE_ACK + MyProtocol.END_OF_MESSAGE), 0, (MyProtocol.NEGATIVE_ACK + MyProtocol.END_OF_MESSAGE).Length);

            }
        }

        public static void receiveFile(ref Socket sock, string fileName, Int64 fileSize)
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

                Console.WriteLine("written a chunck. remain " + chunks + " to write");
            } while (chunks >= 0);

            Console.WriteLine("Done!");
            output.Close();
        }

        public static void handleFileDrop(ref Socket sock, string baseDir)
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

        public static void ReceiveSubDirectory(ref Socket sock, string basedir)
        {
            //byte[] msg;
            string cmd = String.Empty;
            //Int32 dirNameSize;
            string dirName, newBaseDir;

            do
            {
                cmd = ReceiveTillTerminator(ref sock);

                switch (cmd)
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

        public static string ReceiveDirectory(ref Socket sock)
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

        public static Bitmap ConvertByteArrayToBitmap(byte[] imageSource)
        {
            var imageConverter = new ImageConverter();
            var image = (Image)imageConverter.ConvertFrom(imageSource);

            return new Bitmap(image);
        }

        #endregion

        public static void SendData(ref Socket soc, byte[] buffer, int offset, int length)
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

        //PER CLIPBOARD HANDLER
        public static string ReceiveTillTerminator(ref Socket sock)
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
     
    }
}