namespace FreeEcho
{
    namespace FEWindowSelectionMouse
    {
        /// <summary>
        /// NativeMethods
        /// </summary>
        internal class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr WindowFromPoint(System.Drawing.Point Point);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr GetAncestor(System.IntPtr hWnd, uint gaFlags);
            [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern System.IntPtr GetWindowRect(System.IntPtr hWnd, out RECT lpRect);
            [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern bool GetWindowPlacement(System.IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr SetWindowsHookEx(int idHook, NativeMethodsDelegate.MouseHookCallback lpfn, System.IntPtr hMod, uint dwThreadId);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr CallNextHookEx(System.IntPtr hhk, int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(System.IntPtr hhk);
        }
    }
}
