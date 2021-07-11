namespace FreeEcho
{
    namespace FEWindowSelectionMouse
    {
        /// <summary>
        /// NativeMethods delegate
        /// </summary>
        internal class NativeMethodsDelegate
        {
            public delegate System.IntPtr MouseHookCallback(int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);
        }
    }
}
