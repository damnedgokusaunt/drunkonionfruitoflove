using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProgettoPdS
{
    class HotkeysHandler
    {
        #region DLL Imports
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        private IntPtr hWnd;
        private List<int> hotkeys;

        #region Constructor and destructor
        public HotkeysHandler(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            this.hotkeys = new List<int>();
        }

        ~HotkeysHandler()
        {
            this.Reset();
        }
        #endregion
        
        public bool Register(int id, int modifier, int key)
        {
            Unregister(id);

            if (RegisterHotKey(hWnd, id, modifier, key))
            {
                hotkeys.Add(id);

                return true;
            }

            return false;
        }

        public bool Unregister(int id)
        {
            if (UnregisterHotKey(hWnd, id))
            {
                hotkeys.Remove(id);

                return true;
            }

            return false;
        }

        public bool Reset()
        {
            bool result = true;

            int n = hotkeys.Count;

            for (int i = 0; i < n; i++)
                result &= Unregister(i);

            return result;
        }
    }
}
