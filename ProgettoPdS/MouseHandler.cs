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

namespace ProgettoPdS
{
    class MouseHandler
    {
        // Dll import. -> method mouse_move
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private UdpClient s;
        private IPEndPoint remoteEP;
        private Int32 ScreenWidth, ScreenHeight, clientWidth, clientHeight;


        private byte[] data;

        SynchronousSocketListener server = null;
        
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

        public MouseHandler(UdpClient s, IPEndPoint remoteEP, Int32 clientWidth, Int32 clientHeight)
        {
            this.s = s;
            this.remoteEP = remoteEP;
            this.clientWidth = clientWidth;
            this.clientHeight = clientHeight;

            ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
        }

        private void Move()
        {
            Int32 x = BitConverter.ToInt32(data, 0) * ScreenWidth / clientWidth;
            Int32 y = BitConverter.ToInt32(data, sizeof(Int32)) * ScreenHeight / clientHeight;

            Cursor.Position = new Point(x, y);
        }

        private void RightClick()
        {
            // Send click to system
            mouse_event((int)MouseEventFlagsAPI.RIGHTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.RIGHTUP, 0, 0, 0, 0);
        }

        
        private void LeftClick()
        {
            // Send click to system
            // mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.LEFTUP, 0, 0, 0, 0);
        }

        private void Scroll(int delta)
        {
            //Console.WriteLine("Server esegue wheel");

            mouse_event((int)MouseEventFlagsAPI.WHEEL, 0, 0, delta, 0);
        }

        

        private void DragAndDrop()
        {
            //mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
        }

        public void Run()
        {
            while (true)
            {
                data = s.Receive(ref remoteEP);

                string data_s = Encoding.ASCII.GetString(data);
                
                int delta = 0;

                if (data_s.StartsWith("CS"))
                {
                    string sub = data_s.Substring(2, data.Length - 2 * sizeof(char));

                    // Trovo indice in cui finisce il numero
                    int i;
                    for (i = 0; sub[i] != '<' && i < sub.Length; i++) ;

                    delta = Convert.ToInt32(sub.Substring(0, i));

                    //Console.WriteLine("Ricevuto d=" + delta);

                    Scroll(delta);

                    continue;
                }
                
                switch (data_s)
                {
                    case "CLD<EOF>":
                        DragAndDrop();
                        break;

                    case "CLU<EOF>":
                        LeftClick();
                        break;

                    case "CR<EOF>":
                        RightClick();
                        break;

                        /*
                    case "CS<EOF>":
                        Scroll(120);
                        break;
                         * */

                    default:
                        Move();
                        break;
                }
            }

        }
    }
}
