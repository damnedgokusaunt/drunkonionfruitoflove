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

        #region Attributes
        private UdpClient s;
        private IPEndPoint remoteEP;
        private Int32 ScreenWidth, ScreenHeight, clientWidth, clientHeight, widthRatio, heightRatio;
        private byte[] data;
        SynchronousSocketListener server = null;
        #endregion

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

            widthRatio = ScreenWidth / clientWidth;
            heightRatio = ScreenHeight / clientHeight;
        }

        #region Mouse methods
        private void doMouseMove()
        {
            Int32 x = BitConverter.ToInt32(data, 0) * widthRatio;
            Int32 y = BitConverter.ToInt32(data, sizeof(Int32)) * heightRatio;

            Cursor.Position = new Point(x, y);
            //Console.WriteLine("Coordinate: " + x + "," + y);
        }
        private void doMouseRightClick()
        {
            // Send click to system
            mouse_event((int)MouseEventFlagsAPI.RIGHTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.RIGHTUP, 0, 0, 0, 0);
        }
        private void doMouseLeftClick()
        {
            // Send click to system
            // mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.LEFTUP, 0, 0, 0, 0);
        }
        private void doMouseWheel(int delta)
        {
            //Console.WriteLine("Server esegue wheel");

            mouse_event((int)MouseEventFlagsAPI.WHEEL, 0, 0, delta, 0);
        }
        private void doMouseDragAndDrop()
        {
            //mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlagsAPI.LEFTDOWN, 0, 0, 0, 0);
        }
        #endregion

        public void Run()
        {
            char firstCharOfEOM = MyProtocol.END_OF_MESSAGE[0];
            
            while (true)
            {
                data = s.Receive(ref remoteEP);

                string data_s = Encoding.ASCII.GetString(data);

                //Console.WriteLine("Server riceve comando mouse: " + data);

                int delta = 0;

                if (data_s.StartsWith("CS"))
                {
                    string sub = data_s.Substring(2, data.Length - 2 * sizeof(char));

                    // Trovo indice in cui finisce il numero
                    int i;

                    for (i = 0; sub[i] != firstCharOfEOM && i < sub.Length; i++) ;

                    delta = Convert.ToInt32(sub.Substring(0, i));

                    //Console.WriteLine("Ricevuto d=" + delta);

                    doMouseWheel(delta);

                    continue;
                }
                
                switch (data_s)
                {
                    case "CLD<EOF>":
                        doMouseDragAndDrop();
                        break;

                    case "CLU<EOF>":
                        doMouseLeftClick();
                        break;

                    case "CR<EOF>":
                        doMouseRightClick();
                        break;

                        /*
                    case "CS<EOF>":
                        Scroll(120);
                        break;
                         * */

                    default:
                        doMouseMove();
                        break;
                }
            }

        }
    }
}
