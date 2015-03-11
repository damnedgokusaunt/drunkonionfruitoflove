using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgettoPdS
{

    class KeyboardHandler
    {
        private IPEndPoint ep;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte key, byte scan, int flags, int extraInfo); 

        public void setLocalEndPoint(IPEndPoint ep)
        {
            this.ep = ep;
        }

        public void Run()
        {

            byte[] data = new byte[sizeof(char)];

            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(ep);
            listener.Listen(1000);
            
            Socket tcpChannel = listener.Accept();

            listener.Close();

            while (true)
            {
                tcpChannel.Receive(data);

                
                switch ((char)data[0])
                {
                    case 'D':
                        keybd_event(data[1], 0, 0, 0);
                        //Console.WriteLine("Server esegue comando " + (char)data[0] + ":" + data[1]);   
                        break;
                    case 'U':
                        keybd_event(data[1], 0, 2, 0);
                        //Console.WriteLine("Server esegue comando " + (char)data[0] + ":" + data[1]);  
                        break;
                    default:
                        MessageBox.Show("Comando da tastiera non riconosciuto");
                        break;
                }
            }
        }
    }
}
