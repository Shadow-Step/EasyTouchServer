using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyTouchServer.serlib
{
    static class Controller
    {
        //Mouse Event
        [DllImport("user32.dll")]
        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public static void ControllerEvent(DataPackage data) //Check data
        {
            switch (data)
            {
                case MouseData mdata:
                    mouse_event(mdata.Action, (uint)mdata.DirX, (uint)mdata.DirY, 0, IntPtr.Zero);
                    break;
                case KeyboardData kdata:
                    log.Add(kdata.Action + " : " + kdata.Key_code);
                    log_changed = true;
                    //keybd_event((byte)kdata.Key_code, 0, kdata.Action, 0);
                    break;
                default:
                    break;
            }
        }

        //TEMP!!!
        public static List<string> log = new List<string>();
        public static bool log_changed = false;
    }
}
