using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

namespace MyProject
{
    /// <summary>
    /// This class allows you to tap keyboard and mouse and / or to detect their activity even when an 
    /// application runes in background or does not have any user interface at all. This class raises 
    /// common .NET events with KeyEventArgs and MouseEventArgs so you can easily retrive any information you need.
    /// </summary>
    public class GlobalHook
    {
        #region Windows structure definitions

        /// <summary>
        /// The POINT structure defines the x- and y- coordinates of a point. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/rectangl_0tiq.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            /// <summary>
            /// Specifies the x-coordinate of the point. 
            /// </summary>
            public int x;
            /// <summary>
            /// Specifies the y-coordinate of the point. 
            /// </summary>
            public int y;
        }

        /// <summary>
        /// The MOUSEHOOKSTRUCT structure contains information about a mouse event passed to a WH_MOUSE hook procedure, MouseProc. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            /// <summary>
            /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates. 
            /// </summary>
            public POINT pt;
            /// <summary>
            /// Handle to the window that will receive the mouse message corresponding to the mouse event. 
            /// </summary>
            public int hwnd;
            /// <summary>
            /// Specifies the hit-test value. For a list of hit-test values, see the description of the WM_NCHITTEST message. 
            /// </summary>
            public int wHitTestCode;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }

        /// <summary>
        /// The MSLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseLLHookStruct
        {
            /// <summary>
            /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates. 
            /// </summary>
            public POINT pt;
            /// <summary>
            /// If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. 
            /// The low-order word is reserved. A positive value indicates that the wheel was rotated forward, 
            /// away from the user; a negative value indicates that the wheel was rotated backward, toward the user. 
            /// One wheel click is defined as WHEEL_DELTA, which is 120. 
            ///If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP,
            /// or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
            /// and the low-order word is reserved. This value can be one or more of the following values. Otherwise, mouseData is not used. 
            ///XBUTTON1
            ///The first X button was pressed or released.
            ///XBUTTON2
            ///The second X button was pressed or released.
            /// </summary>
            public int mouseData;
            /// <summary>
            /// Specifies the event-injected flag. An application can use the following value to test the mouse flags. Value Purpose 
            ///LLMHF_INJECTED Test the event-injected flag.  
            ///0
            ///Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it is 0.
            ///1-15
            ///Reserved.
            /// </summary>
            public int flags;
            /// <summary>
            /// Specifies the time stamp for this message.
            /// </summary>
            public int time;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }


        /// <summary>
        /// The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            /// </summary>
            public int vkCode;
            /// <summary>
            /// Specifies a hardware scan code for the key. 
            /// </summary>
            public int scanCode;
            /// <summary>
            /// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            public int flags;
            /// <summary>
            /// Specifies the time stamp for this message.
            /// </summary>
            public int time;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }
       
        #endregion

        #region Windows function imports

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
        /// You would install a hook procedure to monitor the system for certain types of events. These events 
        /// are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        /// [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a 
        /// thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link 
        /// library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        /// [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter. 
        /// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by 
        /// the current process and if the hook procedure is within the code associated with the current process. 
        /// </param>
        /// <param name="dwThreadId">
        /// [in] Specifies the identifier of the thread with which the hook procedure is to be associated. 
        /// If this parameter is zero, the hook procedure is associated with all existing threads running in the 
        /// same desktop as the calling thread. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook procedure.
        /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
           CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. 
        /// A hook procedure can call this function either before or after processing the hook information. 
        /// </summary>
        /// <param name="idHook">Ignored.</param>
        /// <param name="nCode">
        /// [in] Specifies the hook code passed to the current hook procedure. 
        /// The next hook procedure uses this code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies the wParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <param name="lParam">
        /// [in] Specifies the lParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain. 
        /// The current hook procedure must also return this value. The meaning of the return value depends on the hook type. 
        /// For more information, see the descriptions of the individual hook procedures.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        /// <summary>
        /// The CallWndProc hook procedure is an application-defined or library-defined callback 
        /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
        /// to this callback function. CallWndProc is a placeholder for the application-defined 
        /// or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// The ToAscii function translates the specified virtual-key code and keyboard 
        /// state to the corresponding character or characters. The function translates the code 
        /// using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">
        /// [in] Specifies the virtual-key code to be translated. 
        /// </param>
        /// <param name="uScanCode">
        /// [in] Specifies the hardware scan code of the key to be translated. 
        /// The high-order bit of this value is set if the key is up (not pressed). 
        /// </param>
        /// <param name="lpbKeyState">
        /// [in] Pointer to a 256-byte array that contains the current keyboard state. 
        /// Each element (byte) in the array contains the state of one key. 
        /// If the high-order bit of a byte is set, the key is down (pressed). 
        /// The low bit, if set, indicates that the key is toggled on. In this function, 
        /// only the toggle bit of the CAPS LOCK key is relevant. The toggle state 
        /// of the NUM LOCK and SCROLL LOCK keys is ignored.
        /// </param>
        /// <param name="lpwTransKey">
        /// [out] Pointer to the buffer that receives the translated character or characters. 
        /// </param>
        /// <param name="fuState">
        /// [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 
        /// </param>
        /// <returns>
        /// If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
        /// Value Meaning 
        /// 0 The specified virtual key has no translation for the current state of the keyboard. 
        /// 1 One character was copied to the buffer. 
        /// 2 Two characters were copied to the buffer. This usually happens when a dead-key character 
        /// (accent or diacritic) stored in the keyboard layout cannot be composed with the specified 
        /// virtual key to form a single character. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        /// <summary>
        /// The GetKeyboardState function copies the status of the 256 virtual keys to the 
        /// specified buffer. 
        /// </summary>
        /// <param name="pbKeyState">
        /// [in] Pointer to a 256-byte array that contains keyboard key states. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        #endregion

        #region Windows constants

        
        public enum WindowsVirtualKey   
        {
            [Description("Left mouse button")]
            VK_LBUTTON = 0x01,

            [Description("Right mouse button")]
            VK_RBUTTON = 0x02,

            [Description("Control-break processing")]
            VK_CANCEL = 0x03,

            [Description("Middle mouse button (three-button mouse)")]
            VK_MBUTTON = 0x04,

            [Description("X1 mouse button")]
            VK_XBUTTON1 = 0x05,

            [Description("X2 mouse button")]
            VK_XBUTTON2 = 0x06,

            [Description("BACKSPACE key")]
            VK_BACK = 0x08,

            [Description("TAB key")]
            VK_TAB = 0x09,

            [Description("CLEAR key")]
            VK_CLEAR = 0x0C,

            [Description("ENTER key")]
            VK_RETURN = 0x0D,

            [Description("SHIFT key")]
            VK_SHIFT = 0x10,

            [Description("CTRL key")]
            VK_CONTROL = 0x11,

            [Description("ALT key")]
            VK_MENU = 0x12,

            [Description("PAUSE key")]
            VK_PAUSE = 0x13,

            [Description("CAPS LOCK key")]
            VK_CAPITAL = 0x14,

            [Description("IME Kana mode")]
            VK_KANA = 0x15,

            [Description("IME Hanguel mode (maintained for compatibility; use VK_HANGUL)")]
            VK_HANGUEL = 0x15,

            [Description("IME Hangul mode")]
            VK_HANGUL = 0x15,

            [Description("IME Junja mode")]
            VK_JUNJA = 0x17,

            [Description("IME final mode")]
            VK_FINAL = 0x18,

            [Description("IME Hanja mode")]
            VK_HANJA = 0x19,

            [Description("IME Kanji mode")]
            VK_KANJI = 0x19,

            [Description("ESC key")]
            VK_ESCAPE = 0x1B,

            [Description("IME convert")]
            VK_CONVERT = 0x1C,

            [Description("IME nonconvert")]
            VK_NONCONVERT = 0x1D,

            [Description("IME accept")]
            VK_ACCEPT = 0x1E,

            [Description("IME mode change request")]
            VK_MODECHANGE = 0x1F,

            [Description("SPACEBAR")]
            VK_SPACE = 0x20,

            [Description("PAGE UP key")]
            VK_PRIOR = 0x21,

            [Description("PAGE DOWN key")]
            VK_NEXT = 0x22,

            [Description("END key")]
            VK_END = 0x23,

            [Description("HOME key")]
            VK_HOME = 0x24,

            [Description("LEFT ARROW key")]
            VK_LEFT = 0x25,

            [Description("UP ARROW key")]
            VK_UP = 0x26,

            [Description("RIGHT ARROW key")]
            VK_RIGHT = 0x27,

            [Description("DOWN ARROW key")]
            VK_DOWN = 0x28,

            [Description("SELECT key")]
            VK_SELECT = 0x29,

            [Description("PRINT key")]
            VK_PRINT = 0x2A,

            [Description("EXECUTE key")]
            VK_EXECUTE = 0x2B,

            [Description("PRINT SCREEN key")]
            VK_SNAPSHOT = 0x2C,

            [Description("INS key")]
            VK_INSERT = 0x2D,

            [Description("DEL key")]
            VK_DELETE = 0x2E,

            [Description("HELP key")]
            VK_HELP = 0x2F,

            [Description("0 key")]
            K_0 = 0x30,

            [Description("1 key")]
            K_1 = 0x31,

            [Description("2 key")]
            K_2 = 0x32,

            [Description("3 key")]
            K_3 = 0x33,

            [Description("4 key")]
            K_4 = 0x34,

            [Description("5 key")]
            K_5 = 0x35,

            [Description("6 key")]
            K_6 = 0x36,

            [Description("7 key")]
            K_7 = 0x37,

            [Description("8 key")]
            K_8 = 0x38,

            [Description("9 key")]
            K_9 = 0x39,

            [Description("A key")]
            K_A = 0x41,

            [Description("B key")]
            K_B = 0x42,

            [Description("C key")]
            K_C = 0x43,

            [Description("D key")]
            K_D = 0x44,

            [Description("E key")]
            K_E = 0x45,

            [Description("F key")]
            K_F = 0x46,

            [Description("G key")]
            K_G = 0x47,

            [Description("H key")]
            K_H = 0x48,

            [Description("I key")]
            K_I = 0x49,

            [Description("J key")]
            K_J = 0x4A,

            [Description("K key")]
            K_K = 0x4B,

            [Description("L key")]
            K_L = 0x4C,

            [Description("M key")]
            K_M = 0x4D,

            [Description("N key")]
            K_N = 0x4E,

            [Description("O key")]
            K_O = 0x4F,

            [Description("P key")]
            K_P = 0x50,

            [Description("Q key")]
            K_Q = 0x51,

            [Description("R key")]
            K_R = 0x52,

            [Description("S key")]
            K_S = 0x53,

            [Description("T key")]
            K_T = 0x54,

            [Description("U key")]
            K_U = 0x55,

            [Description("V key")]
            K_V = 0x56,

            [Description("W key")]
            K_W = 0x57,

            [Description("X key")]
            K_X = 0x58,

            [Description("Y key")]
            K_Y = 0x59,

            [Description("Z key")]
            K_Z = 0x5A,

            [Description("Left Windows key (Natural keyboard)")]
            VK_LWIN = 0x5B,

            [Description("Right Windows key (Natural keyboard)")]
            VK_RWIN = 0x5C,

            [Description("Applications key (Natural keyboard)")]
            VK_APPS = 0x5D,

            [Description("Computer Sleep key")]
            VK_SLEEP = 0x5F,

            [Description("Numeric keypad 0 key")]
            VK_NUMPAD0 = 0x60,

            [Description("Numeric keypad 1 key")]
            VK_NUMPAD1 = 0x61,

            [Description("Numeric keypad 2 key")]
            VK_NUMPAD2 = 0x62,

            [Description("Numeric keypad 3 key")]
            VK_NUMPAD3 = 0x63,

            [Description("Numeric keypad 4 key")]
            VK_NUMPAD4 = 0x64,

            [Description("Numeric keypad 5 key")]
            VK_NUMPAD5 = 0x65,

            [Description("Numeric keypad 6 key")]
            VK_NUMPAD6 = 0x66,

            [Description("Numeric keypad 7 key")]
            VK_NUMPAD7 = 0x67,

            [Description("Numeric keypad 8 key")]
            VK_NUMPAD8 = 0x68,

            [Description("Numeric keypad 9 key")]
            VK_NUMPAD9 = 0x69,

            [Description("Multiply key")]
            VK_MULTIPLY = 0x6A,

            [Description("Add key")]
            VK_ADD = 0x6B,

            [Description("Separator key")]
            VK_SEPARATOR = 0x6C,

            [Description("Subtract key")]
            VK_SUBTRACT = 0x6D,

            [Description("Decimal key")]
            VK_DECIMAL = 0x6E,

            [Description("Divide key")]
            VK_DIVIDE = 0x6F,

            [Description("F1 key")]
            VK_F1 = 0x70,

            [Description("F2 key")]
            VK_F2 = 0x71,

            [Description("F3 key")]
            VK_F3 = 0x72,

            [Description("F4 key")]
            VK_F4 = 0x73,

            [Description("F5 key")]
            VK_F5 = 0x74,

            [Description("F6 key")]
            VK_F6 = 0x75,

            [Description("F7 key")]
            VK_F7 = 0x76,

            [Description("F8 key")]
            VK_F8 = 0x77,

            [Description("F9 key")]
            VK_F9 = 0x78,

            [Description("F10 key")]
            VK_F10 = 0x79,

            [Description("F11 key")]
            VK_F11 = 0x7A,

            [Description("F12 key")]
            VK_F12 = 0x7B,

            [Description("F13 key")]
            VK_F13 = 0x7C,

            [Description("F14 key")]
            VK_F14 = 0x7D,

            [Description("F15 key")]
            VK_F15 = 0x7E,

            [Description("F16 key")]
            VK_F16 = 0x7F,

            [Description("F17 key")]
            VK_F17 = 0x80,

            [Description("F18 key")]
            VK_F18 = 0x81,

            [Description("F19 key")]
            VK_F19 = 0x82,

            [Description("F20 key")]
            VK_F20 = 0x83,

            [Description("F21 key")]
            VK_F21 = 0x84,

            [Description("F22 key")]
            VK_F22 = 0x85,

            [Description("F23 key")]
            VK_F23 = 0x86,

            [Description("F24 key")]
            VK_F24 = 0x87,

            [Description("NUM LOCK key")]
            VK_NUMLOCK = 0x90,

            [Description("SCROLL LOCK key")]
            VK_SCROLL = 0x91,

            [Description("Left SHIFT key")]
            VK_LSHIFT = 0xA0,

            [Description("Right SHIFT key")]
            VK_RSHIFT = 0xA1,

            [Description("Left CONTROL key")]
            VK_LCONTROL = 0xA2,

            [Description("Right CONTROL key")]
            VK_RCONTROL = 0xA3,

            [Description("Left MENU key")]
            VK_LMENU = 0xA4,

            [Description("Right MENU key")]
            VK_RMENU = 0xA5,

            [Description("Browser Back key")]
            VK_BROWSER_BACK = 0xA6,

            [Description("Browser Forward key")]
            VK_BROWSER_FORWARD = 0xA7,

            [Description("Browser Refresh key")]
            VK_BROWSER_REFRESH = 0xA8,

            [Description("Browser Stop key")]
            VK_BROWSER_STOP = 0xA9,

            [Description("Browser Search key")]
            VK_BROWSER_SEARCH = 0xAA,

            [Description("Browser Favorites key")]
            VK_BROWSER_FAVORITES = 0xAB,

            [Description("Browser Start and Home key")]
            VK_BROWSER_HOME = 0xAC,

            [Description("Volume Mute key")]
            VK_VOLUME_MUTE = 0xAD,

            [Description("Volume Down key")]
            VK_VOLUME_DOWN = 0xAE,

            [Description("Volume Up key")]
            VK_VOLUME_UP = 0xAF,

            [Description("Next Track key")]
            VK_MEDIA_NEXT_TRACK = 0xB0,

            [Description("Previous Track key")]
            VK_MEDIA_PREV_TRACK = 0xB1,

            [Description("Stop Media key")]
            VK_MEDIA_STOP = 0xB2,

            [Description("Play/Pause Media key")]
            VK_MEDIA_PLAY_PAUSE = 0xB3,

            [Description("Start Mail key")]
            VK_LAUNCH_MAIL = 0xB4,

            [Description("Select Media key")]
            VK_LAUNCH_MEDIA_SELECT = 0xB5,

            [Description("Start Application 1 key")]
            VK_LAUNCH_APP1 = 0xB6,

            [Description("Start Application 2 key")]
            VK_LAUNCH_APP2 = 0xB7,

            [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key")]
            VK_OEM_1 = 0xBA,

            [Description("For any country/region, the '+' key")]
            VK_OEM_PLUS = 0xBB,

            [Description("For any country/region, the ',' key")]
            VK_OEM_COMMA = 0xBC,

            [Description("For any country/region, the '-' key")]
            VK_OEM_MINUS = 0xBD,

            [Description("For any country/region, the '.' key")]
            VK_OEM_PERIOD = 0xBE,

            [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key")]
            VK_OEM_2 = 0xBF,

            [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key")]
            VK_OEM_3 = 0xC0,

            [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key")]
            VK_OEM_4 = 0xDB,

            [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\\|' key")]
            VK_OEM_5 = 0xDC,

            [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key")]
            VK_OEM_6 = 0xDD,

            [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key")]
            VK_OEM_7 = 0xDE,

            [Description("Used for miscellaneous characters; it can vary by keyboard.")]
            VK_OEM_8 = 0xDF,


            [Description("Either the angle bracket key or the backslash key on the RT 102-key keyboard")]
            VK_OEM_102 = 0xE2,

            [Description("IME PROCESS key")]
            VK_PROCESSKEY = 0xE5,


            [Description("Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP")]
            VK_PACKET = 0xE7,

            [Description("Attn key")]
            VK_ATTN = 0xF6,

            [Description("CrSel key")]
            VK_CRSEL = 0xF7,

            [Description("ExSel key")]
            VK_EXSEL = 0xF8,

            [Description("Erase EOF key")]
            VK_EREOF = 0xF9,

            [Description("Play key")]
            VK_PLAY = 0xFA,

            [Description("Zoom key")]
            VK_ZOOM = 0xFB,

            [Description("PA1 key")]
            VK_PA1 = 0xFD,

            [Description("Clear key")]
            VK_OEM_CLEAR = 0xFE,

        }

        //values from Winuser.h in Microsoft SDK.
        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        private const int WH_MOUSE_LL       = 14;
        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        private const int WH_KEYBOARD_LL    = 13;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
        /// </summary>
        private const int WH_MOUSE          = 7;
        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
        /// </summary>
        private const int WH_KEYBOARD       = 2;

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        private const int WM_MOUSEMOVE      = 0x200;
        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDOWN    = 0x201;
        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WM_RBUTTONDOWN    = 0x204;
        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONDOWN    = 0x207;
        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        private const int WM_LBUTTONUP      = 0x202;
        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        private const int WM_RBUTTONUP      = 0x205;
        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONUP      = 0x208;
        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDBLCLK  = 0x203;
        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
        /// </summary>
        private const int WM_RBUTTONDBLCLK  = 0x206;
        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
        /// </summary>
        private const int WM_MBUTTONDBLCLK  = 0x209;
        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        private const int WM_MOUSEWHEEL     = 0x020A;

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
        /// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        private const int WM_KEYDOWN = 0x100;
        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
        /// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        private const int WM_KEYUP = 0x101;
        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
        /// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
        /// presses another key. It also occurs when no window currently has the keyboard focus; 
        /// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
        /// receives the message can distinguish between these two contexts by checking the context 
        /// code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYDOWN = 0x104;
        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
        /// releases a key that was pressed while the ALT key was held down. It also occurs when no 
        /// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
        /// to the active window. The window that receives the message can distinguish between 
        /// these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT     = 0x10;
        private const byte VK_CAPITAL   = 0x14;
        private const byte VK_NUMLOCK   = 0x90;

        #endregion

        /// <summary>
        /// Creates an instance of UserActivityHook object and sets mouse and keyboard hooks.
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public GlobalHook()
        {
            Start();
        }

        /// <summary>
        /// Creates an instance of UserActivityHook object and installs both or one of mouse and/or keyboard hooks and starts rasing events
        /// </summary>
        /// <param name="InstallMouseHook"><b>true</b> if mouse events must be monitored</param>
        /// <param name="InstallKeyboardHook"><b>true</b> if keyboard events must be monitored</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        /// <remarks>
        /// To create an instance without installing hooks call new UserActivityHook(false, false)
        /// </remarks>
        public GlobalHook(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            Start(InstallMouseHook, InstallKeyboardHook);
        }

        /// <summary>
        /// Destruction.
        /// </summary>
        ~GlobalHook()
        {
            //uninstall hooks and do not throw exceptions
            Stop(true, true, false);
        }

        /// <summary>
        /// Mouse events
        /// </summary>
        public event MouseEventHandler MouseMove, MouseDown, MouseUp, MouseWheel;
        /// <summary>
        /// Keyboard events
        /// </summary>
        public delegate void MyCustomKeyEventHandler(object sender, byte vkCode, byte scanCode);
        public event MyCustomKeyEventHandler KeyDown, KeyUp;

        /// <summary>
        /// Stores the handle to the mouse hook procedure.
        /// </summary>
        private int hMouseHook = 0;
        /// <summary>
        /// Stores the handle to the keyboard hook procedure.
        /// </summary>
        private int hKeyboardHook = 0;


        /// <summary>
        /// Declare MouseHookProcedure as HookProc type.
        /// </summary>
        private static HookProc MouseHookProcedure;
        /// <summary>
        /// Declare KeyboardHookProcedure as HookProc type.
        /// </summary>
        private static HookProc KeyboardHookProcedure;


        /// <summary>
        /// Installs both mouse and keyboard hooks and starts rasing events
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start()
        {
            this.Start(true, true);
        }

        /// <summary>
        /// Installs both or one of mouse and/or keyboard hooks and starts rasing events
        /// </summary>
        /// <param name="InstallMouseHook"><b>true</b> if mouse events must be monitored</param>
        /// <param name="InstallKeyboardHook"><b>true</b> if keyboard events must be monitored</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            // install Mouse hook only if it is not installed and must be installed
            if (hMouseHook == 0 && InstallMouseHook)
            {
                // Create an instance of HookProc.
                MouseHookProcedure = new HookProc(MouseHookProc);
                //install hook
                hMouseHook = SetWindowsHookEx(
                    WH_MOUSE_LL,
                    MouseHookProcedure,
                    Marshal.GetHINSTANCE(
                        Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (hMouseHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(true, false, false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }

            // install Keyboard hook only if it is not installed and must be installed
            if (hKeyboardHook == 0 && InstallKeyboardHook)
            {
                // Create an instance of HookProc.
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                //install hook
                hKeyboardHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    KeyboardHookProcedure,
                    Marshal.GetHINSTANCE(
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (hKeyboardHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(false, true, false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>
        /// Stops monitoring both mouse and keyboard events and rasing events.
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop()
        {
            this.Stop(true, true, true);
        }

        /// <summary>
        /// Stops monitoring both or one of mouse and/or keyboard events and rasing events.
        /// </summary>
        /// <param name="UninstallMouseHook"><b>true</b> if mouse hook must be uninstalled</param>
        /// <param name="UninstallKeyboardHook"><b>true</b> if keyboard hook must be uninstalled</param>
        /// <param name="ThrowExceptions"><b>true</b> if exceptions which occured during uninstalling must be thrown</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop(bool UninstallMouseHook, bool UninstallKeyboardHook, bool ThrowExceptions)
        {
            //if mouse hook set and must be uninstalled
            if (hMouseHook != 0 && UninstallMouseHook)
            {
                //uninstall hook
                int retMouse = UnhookWindowsHookEx(hMouseHook);
                //reset invalid handle
                hMouseHook = 0;
                //if failed and exception must be thrown
                if (retMouse == 0 && ThrowExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }

            //if keyboard hook set and must be uninstalled
            if (hKeyboardHook != 0 && UninstallKeyboardHook)
            {
                //uninstall hook
                int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                //reset invalid handle
                hKeyboardHook = 0;
                //if failed and exception must be thrown
                if (retKeyboard == 0 && ThrowExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }


        /// <summary>
        /// A callback function which will be called every time a mouse activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            // if ok and someone listens to our events
            if (nCode >= 0)
            {
                //Marshall the data from callback.
                MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                //detect button clicked
                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;

                //double clicks
                int clickCount = 0;
                if (button != MouseButtons.None)
                    if (wParam == WM_LBUTTONDBLCLK || wParam == WM_RBUTTONDBLCLK) clickCount = 2;
                    else clickCount = 1;

                switch (wParam)
                {
                    case WM_MOUSEMOVE:
                        MouseMove(this, new MouseEventArgs(MouseButtons.None, clickCount, mouseHookStruct.pt.x, mouseHookStruct.pt.y, mouseDelta)); 
                        break;
                    case WM_LBUTTONDOWN:
                        MouseDown(this, new MouseEventArgs(MouseButtons.Left, clickCount, mouseHookStruct.pt.x, mouseHookStruct.pt.y, mouseDelta)); 
                        break;
                    case WM_LBUTTONUP:
                        MouseUp(this, new MouseEventArgs(MouseButtons.Left, clickCount, mouseHookStruct.pt.x, mouseHookStruct.pt.y, mouseDelta));
                        break;
                    case WM_RBUTTONDOWN:
                        MouseDown(this, new MouseEventArgs(MouseButtons.Right, clickCount, mouseHookStruct.pt.x, mouseHookStruct.pt.y, mouseDelta)); 
                        break;
                    case WM_MOUSEWHEEL:
                        //If the message is WM_MOUSEWHEEL, the high-order word of mouseData member is the wheel delta. 
                        //One wheel click is defined as WHEEL_DELTA, which is 120. 
                        //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.mouseData >> 16) & 0xffff);
                        //TODO: X BUTTONS (I havent them so was unable to test)
                        //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                        //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                        //and the low-order word is reserved. This value can be one or more of the following values. 
                        //Otherwise, mouseData is not used. 
                        MouseWheel(this, new MouseEventArgs(MouseButtons.None, clickCount, mouseHookStruct.pt.x, mouseHookStruct.pt.y, mouseDelta));         
                        break;
                }
            }
            //call next hook
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }


        /// <summary>
        /// A callback function which will be called every time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        public int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            bool handled = false;

            if (nCode >= 0)
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                
                byte vkCode = (byte)MyKeyboardHookStruct.vkCode;
                // Il tasto Windows deve essere inviato al server ma bloccato sul client
                if (vkCode == (byte)WindowsVirtualKey.VK_LWIN || vkCode == (byte)WindowsVirtualKey.VK_RWIN)
                    handled = true;

                byte scanCode = (byte)MyKeyboardHookStruct.scanCode;

                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    KeyDown(this, vkCode, scanCode);
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    KeyUp(this, vkCode, scanCode);
            }

            if (handled)
                return 1;
            else
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
    }
}
