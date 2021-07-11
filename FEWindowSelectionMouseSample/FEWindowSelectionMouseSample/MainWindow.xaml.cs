namespace FEWindowSelectionMouseSample
{
    public partial class MainWindow : System.Windows.Window
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int GetWindowTextLength(System.IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(System.IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        FreeEcho.FEWindowSelectionMouse.WindowSelectionFrame WindowSelectionFrame;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                WindowSelectionLeftButtonUpButton.PreviewMouseDown += WindowSelectionLeftButtonUpButton_PreviewMouseDown;
            }
            catch
            {
            }
        }

        private void WindowSelectionLeftButtonUpButton_PreviewMouseDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
            )
        {
            try
            {
                WindowSelectionFrame = new()
                {
                    MouseLeftUpStop = true
                };
                WindowSelectionFrame.MouseLeftButtonUp += WindowSelectionFrame_MouseLeftButtonUpEvent;
                WindowSelectionFrame.StartWindowSelection();
            }
            catch
            {
            }
        }

        private void WindowSelectionFrame_MouseLeftButtonUpEvent(
            object sender,
            FreeEcho.FEWindowSelectionMouse.MouseLeftButtonUpEventArgs e
            )
        {
            try
            {
                System.Text.StringBuilder stringData = new(GetWindowTextLength(WindowSelectionFrame.SelectedHwnd) + 1);
                GetWindowText(WindowSelectionFrame.SelectedHwnd, stringData, stringData.Capacity);
                System.Diagnostics.Debug.WriteLine(stringData.ToString());

                WindowSelectionFrame = null;
            }
            catch
            {
            }
        }
    }
}
