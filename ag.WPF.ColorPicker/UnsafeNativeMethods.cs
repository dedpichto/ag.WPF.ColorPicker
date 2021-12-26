using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.ColorPicker
{
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        internal static void SetCursorPosition(int x, int y)
        {
            _ = SetCursorPos(x, y);
        }
    }
}
