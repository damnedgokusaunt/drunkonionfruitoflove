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
using Microsoft.Win32;


namespace MyProject
{
    static class Functions
    {
        public delegate void ProgressBarUpdater(UInt32 v);
        public delegate void LabelUpdater(string m);

        public delegate void SendClipboardDelegate(byte[] bytes, int length);
        public delegate byte[] ReceiveClipboardDelegate(int size);
        
        public static ProgressBarUpdater update_progressbar;
        public static LabelUpdater update_label;
        public static SendClipboardDelegate SendClipboard;
        public static ReceiveClipboardDelegate ReceiveClipboard;

        public delegate IDataObject GetDataObjectDelegate();
        public static GetDataObjectDelegate getDataObject;

        public delegate bool ClipboardContainsDelegate(string format);
        public static ClipboardContainsDelegate clipboardContains;

        public delegate void ClipboardSetDataDelegate(string format, object data);
        public static ClipboardSetDataDelegate clipboardSetData;

        public delegate T ClipboardGetData<T>();
        public static ClipboardGetData<Image> clipboardGetImage;

        public delegate void ClipboardSetImageDelegate(Image i);
        public static ClipboardSetImageDelegate clipboardSetImage;



        [DllImport("user32.dll")]
        static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        #region Connection handling methods

        public static int FindFreePort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();

            return port;
        }
        
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
        public static void KeyUp(byte key,byte scanCode)
        {
            keybd_event(key, scanCode, 2, 0);

        }

        public static void KeyDown(byte key,byte scanCode)
        {
            keybd_event(key, scanCode, 0, 0);
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
        public static bool handleClipboardData()
        {
            try
            {
                string clipbdMsg;
                object clipContent;
                string[] path;
                int n;
                byte[] buffer;

                IDataObject iData = new DataObject();
                iData = getDataObject();

                if (clipboardContains(DataFormats.Text))
                {
                    string text = (string)iData.GetData(DataFormats.Text);

                    clipbdMsg = MyProtocol.COPY + text + MyProtocol.END_OF_MESSAGE;
                    buffer = Encoding.ASCII.GetBytes(clipbdMsg);

                    SendClipboard(buffer, buffer.Length);

                    return true;
                }

                if (clipboardContains(DataFormats.Bitmap))
                {
                    Console.WriteLine("Bitmap");

                    //invio comando bitmap + terminatore
                    buffer = Encoding.ASCII.GetBytes(MyProtocol.IMG + MyProtocol.END_OF_MESSAGE);
                    SendClipboard(buffer, buffer.Length);
                    
                    //attendo ack di ricezione
                    ReceiveClipboard(MyProtocol.POSITIVE_ACK.Length);
                    
                    //preparazione invio dati bitmap
                    Image img = clipboardGetImage();
                    //converto da bitmap obj a byte array
                    byte[] res = ConvertBitmapToByteArray(img);
                    //invio la dimensione dei dati
                    Int32 dim = res.Length;
                    SendClipboard(BitConverter.GetBytes(dim), sizeof(Int32));

                    //attendo ack di ricezione
                    ReceiveClipboard(MyProtocol.POSITIVE_ACK.Length);
                    
                    //invio dati
                    SendClipboard(res, res.Length);

                    return true;
                }

                if (clipboardContains(DataFormats.FileDrop))
                {
                    buffer = Encoding.ASCII.GetBytes(MyProtocol.CLEAN + MyProtocol.END_OF_MESSAGE);
                    SendClipboard(buffer, buffer.Length);

                    Console.WriteLine("Clean!");
                    Console.WriteLine("preparazione invio file...");


                    IDataObject data = getDataObject();
                    clipContent = data.GetData(DataFormats.FileDrop);
                    path = (string[])clipContent;
                    n = path.Length;

                    for (int i = 0; i < n; i++)
                    {
                        if (File.Exists(path[i]))
                            ClipboardSendFile(path[i]);
                        else if (Directory.Exists(path[i]))
                            ClipboardRecursiveDirectorySend(path[i]);
                        else
                            Console.WriteLine("Isn't a File or a Directory!!!");              
                    }

                    Console.WriteLine("done.");
                    
                    return true;
                }
            }
            catch (SocketException)
            {
                throw;
            }

            return false;
        }


        public static void receiveFile(Socket sock, string fileName, Int64 fileSize)
        {
            Int64 bytes_sent = 0;
            UInt32 perc;
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
                    chunkSize = MyProtocol.CHUNK_SIZE;
                else
                {
                    resto = Convert.ToInt32(fileSize % MyProtocol.CHUNK_SIZE);
                    if (resto > 0)
                        chunkSize = resto;
                    else
                        break;
                }

                chunks--;

                try
                {
                    buffer = ReceiveClipboard(chunkSize);

                    bytes_sent += chunkSize;
                    perc = (UInt32)(bytes_sent * 100 / fileSize);
                
                    if (update_label !=null && update_progressbar!=null)
                    {
                        update_label("Ricevuti " + bytes_sent + "/" + fileSize + " byte (" + perc + "%).");
                        update_progressbar(perc);
                    }

                }
                catch (SocketException)
                {
                    output.Close();
                    throw;
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

            if (update_label != null && update_progressbar != null)
            {
                update_label("File ricevuto correttamente.");
                update_progressbar(0);
            }
                

            Console.WriteLine("Done!");
            output.Close();
        }

        public static void handleFileDrop(Socket sock, string baseDir)
        {
            string fileName = null;
            StringCollection FileCollection = new StringCollection();
            byte[] buffer = null;

            try
            {
                Console.WriteLine("Inizio ricezione di un file.");

                // Ricezione della dimensione del file
                Console.WriteLine("Aspetto di ricevere dim del file...");

                buffer = ReceiveClipboard(sizeof(Int64));

                Int64 fileSize = BitConverter.ToInt64(buffer, 0);

                // Ricezione del nome del file
                Console.WriteLine("Aspetto di ricevere nome del file...");

                byte[] bytes = new byte[1];

                fileName = ReceiveTillTerminator(sock);

                Console.WriteLine("Nome del file: " + fileName);

                if (baseDir != null)
                    fileName = baseDir + Path.DirectorySeparatorChar + fileName;
                else
                    fileName = MyProtocol.CLIPBOARD_DIR + Path.DirectorySeparatorChar + fileName;

                Console.WriteLine("Inizio trasferimento file: " + fileName);

                receiveFile(sock, fileName, fileSize);
                Console.WriteLine("Fine trasferimento file: " + fileName);
            }
            catch (SocketException)
            {
                throw;
            }
        }

        public static void ReceiveSubDirectory(Socket sock, string basedir)
        {
            //byte[] msg;
            string cmd = String.Empty;
            //Int32 dirNameSize;
            string dirName, newBaseDir;
            byte[] bytes = null;

            try
            {
                do
                {
                    cmd = ReceiveTillTerminator(sock);

                    switch (cmd)
                    {
                        case MyProtocol.FILE_SEND:
                            bytes = Encoding.ASCII.GetBytes(MyProtocol.POSITIVE_ACK);
                            SendClipboard(bytes, bytes.Length);
                            handleFileDrop(sock, basedir);
                            break;

                        case MyProtocol.DIRE_SEND:
                            // nome directory
                            dirName = ReceiveTillTerminator(sock);
                            dirName = System.IO.Path.GetInvalidFileNameChars().Aggregate(dirName, (current, c) => current.Replace(c.ToString(), string.Empty));
                            newBaseDir = System.IO.Path.Combine(basedir, dirName);

                            Directory.CreateDirectory(newBaseDir);

                            Console.WriteLine("Nuova cartella: " + newBaseDir);

                            ReceiveSubDirectory(sock, newBaseDir);
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
            catch (SocketException)
            {
                throw;
            }
        }

        public static string ReceiveDirectory(Socket sock)
        {
            // Int32 dirNameSize;
            // byte[] msg = null;
            string basedir, myBaseDir;

            // Ricevo il nome della directory
            byte[] bytes = new byte[1];

            try
            {
                basedir = ReceiveTillTerminator(sock);

                Console.WriteLine("Ricevuto nome dir: " + basedir);

                myBaseDir = Path.Combine(MyProtocol.CLIPBOARD_DIR, basedir);
                DirectoryInfo di = Directory.CreateDirectory(myBaseDir);

                ReceiveSubDirectory(sock, myBaseDir);

                return myBaseDir;
            }
            catch (SocketException)
            {
                throw;
            }
        }

        public static void SendFile(FileStream fs, Int64 fileSize)
        {
            UInt32 perc = 0;
            Int32 chunks, chunkSize;
            Int64 bytes_sent = 0;
            BinaryReader reader = new BinaryReader(fs);
         
            chunks = Convert.ToInt32(fileSize / MyProtocol.CHUNK_SIZE);
            chunkSize = 0;
            perc = 0;
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
                    SendClipboard(bufferFile, chunkSize);
                    bytes_sent += chunkSize;
                    perc = (UInt32)(bytes_sent * 100 / fileSize);

                    Console.WriteLine("Inviati " + bytes_sent + " byte su " + fileSize + " byte (" + perc + "%).");

                    if(update_label != null && update_progressbar != null)
                    {
                        update_label("Inviati " + bytes_sent + " byte su " + fileSize + " byte (" + perc + "%).");
                        update_progressbar(perc);
                    }
                }
                catch
                {
                    if (update_label != null && update_progressbar != null)
                    {
                        update_label("Impossibile inviare il file per un errore di rete.");
                        update_progressbar(0);
                    }

                    fs.Close();

                    // Motorino d'avviamento
                    keybd_event(0, 0, 0, 0);
                    keybd_event(0, 0, 2, 0);

                    return;
                }
            } while (chunks >= 0);

            Console.WriteLine("The end!");

            if(update_label!=null && update_progressbar!=null)
            {
                update_label("File inviato con successo.");
                update_progressbar(0);
            }
            
        }

        public static void ClipboardSendFile(string name)
        {
            string fileName;
            FileInfo f;
            FileStream fs;
            Int64 fileSize;
            byte[] bytes = null;

            if (!File.Exists(name)) throw new Exception("File doesn't exist!");

            f = new FileInfo(name);
            fileSize = f.Length;
            fileName = f.Name;

            
            MessageBox.Show("Invio file '" + fileName + "' di lunghezza " + fileSize + " byte.");
            try
            {
                fs = File.Open(name, FileMode.Open);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Non hai i permessi per aprire questo file.");
                return;
            }
            

            try
            {
                bytes = Encoding.ASCII.GetBytes(MyProtocol.FILE_SEND + MyProtocol.END_OF_MESSAGE);
                SendClipboard(bytes, bytes.Length);

                Console.WriteLine("Comando invio_file inviato!");

                
                SendClipboard(BitConverter.GetBytes(fileSize), sizeof(Int64));

                Console.WriteLine("FileSize inviata!");

                bytes = Encoding.ASCII.GetBytes(fileName + MyProtocol.END_OF_MESSAGE);
                SendClipboard(bytes, bytes.Length);

                Console.WriteLine("Sto inviando il file: " + fileName);

                SendFile(fs, fileSize);

                fs.Close();

                Console.WriteLine("Done.");
            }
            catch (SocketException)
            {
                fs.Close();
                throw;
            }
            
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

            try
            {
                buffer = Encoding.ASCII.GetBytes(com + MyProtocol.END_OF_MESSAGE);
                SendClipboard(buffer, buffer.Length);

                buffer = Encoding.ASCII.GetBytes(dirName + MyProtocol.END_OF_MESSAGE);
                SendClipboard(buffer, buffer.Length);

                foreach (string f in Directory.GetFiles(sDir))
                    ClipboardSendFile(f);

                foreach (string d in Directory.GetDirectories(sDir))
                    ClipboardRecursiveDirectorySend(d);

                buffer = Encoding.ASCII.GetBytes(MyProtocol.END_OF_DIR + MyProtocol.END_OF_MESSAGE);
                SendClipboard(buffer, buffer.Length);
            }
            catch (SocketException)
            {
                throw;
            }
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

            clipboardSetData(DataFormats.FileDrop, paths.ToArray());
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

        #endregion

        #region Communication methods
        // "consts" to help understand calculations
        const int bytesperlong = 4; // 32 / 8
        const int bitsperbyte = 8;

        public static bool SetKeepAlive(Socket sock, ulong time, ulong interval)
        {
            try
            {
                // resulting structure
                byte[] SIO_KEEPALIVE_VALS = new byte[3 * bytesperlong];

                // array to hold input values
                ulong[] input = new ulong[3];

                // put input arguments in input array
                if (time == 0 || interval == 0) // enable disable keep-alive
                    input[0] = (0UL); // off
                else
                    input[0] = (1UL); // on

                input[1] = (time); // time millis
                input[2] = (interval); // interval millis

                // pack input into byte struct
                for (int i = 0; i < input.Length; i++)
                {
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 3] = (byte)(input[i] >> ((bytesperlong - 1) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 2] = (byte)(input[i] >> ((bytesperlong - 2) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 1] = (byte)(input[i] >> ((bytesperlong - 3) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 0] = (byte)(input[i] >> ((bytesperlong - 4) * bitsperbyte) & 0xff);
                }
                // create bytestruct for result (bytes pending on server socket)
                byte[] result = BitConverter.GetBytes(0);
                // write SIO_VALS to Socket IOControl
                sock.IOControl(IOControlCode.KeepAliveValues, SIO_KEEPALIVE_VALS, result);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public static void AddApplicatinToCurrentUserStartup() {

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("MyProject", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
            }
        
        }



        public static string ReceiveTillTerminator(Socket s)
        {
            byte[] bytes = new byte[1];
            string recvbuf = string.Empty, cmd = null;
            do
            {
                bytes = ReceiveClipboard(1);
                recvbuf += Encoding.ASCII.GetString(bytes);
                //Console.WriteLine("recvbuf: " + recvbuf);
            }
            while (recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE) == -1);

            cmd = recvbuf.Substring(0, recvbuf.IndexOf(MyProtocol.END_OF_MESSAGE));

            return cmd;
        }

        
        public static void SendData(Socket sock, byte[] buffer, int offset, int length)
        {
            NetworkStream clientStream;
            clientStream = new NetworkStream(sock);

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
        
        public static byte[] ReceiveData(Socket sock, int size)
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


        #endregion

        #region Data conversion methods
        public static byte[] ConvertBitmapToByteArray(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        public static Bitmap ConvertByteArrayToBitmap(byte[] imageSource)
        {
            var imageConverter = new ImageConverter();
            var image = (Image)imageConverter.ConvertFrom(imageSource);

            return new Bitmap(image);
        }
        #endregion

    }
}
