namespace WindowsImports
{
    using System;
    using System.Runtime.InteropServices;

    public class User32
    {
        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        public static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        public static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetLastInputInfo(ref LastInputInfo lastInputInfo);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AddClipboardFormatListener(IntPtr hWnd);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hWnd);
    }
}