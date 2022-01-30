using System.Runtime.InteropServices;
using System.Security;

namespace ag.WPF.ColorPicker
{
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        internal static void SetCursorPosition(int x, int y) => _ = SetCursorPos(x, y);
    }
}
