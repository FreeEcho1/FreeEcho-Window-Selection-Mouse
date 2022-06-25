namespace FreeEcho.FEWindowSelectionMouse;

/// <summary>
/// ウィンドウ選択枠
/// </summary>
public class WindowSelectionFrame : System.IDisposable
{
    /// <summary>
    /// Disposeが呼ばれたかの値 (いいえ「false」/はい「true」)
    /// </summary>
    private bool Disposed;
    /// <summary>
    /// フレームウィンドウ
    /// </summary>
    private FrameWindow FrameWindow;
    /// <summary>
    /// フレームウィンドウの色
    /// </summary>
    public System.Drawing.Color ColorFrameWindow
    {
        get;
        set;
    } = System.Drawing.Color.FromArgb(255, 0, 0);
    /// <summary>
    /// フレームの幅
    /// </summary>
    public int FrameWidth
    {
        get;
        set;
    } = 5;
    /// <summary>
    /// 選択したウィンドウのハンドル
    /// </summary>
    public System.IntPtr SelectedHwnd
    {
        get;
        protected set;
    }
    /// <summary>
    /// マウスの左ボタンが離されたら停止するかの値 (停止しない「false」/停止する「true」)
    /// </summary>
    public bool MouseLeftUpStop
    {
        get;
        set;
    }
    /// <summary>
    /// マウスのフックプロシージャのハンドル
    /// </summary>
    private System.IntPtr Handle;
    /// <summary>
    /// フックチェーンにインストールするフックプロシージャのイベント
    /// </summary>
    private event NativeMethodsDelegate.MouseHookCallback HookCallback;
    /// <summary>
    /// マウスの左ボタンが離されたイベントのデリゲート
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MouseLeftButtonUpEventHandler(
        object sender,
        MouseLeftButtonUpEventArgs e
        );
    /// <summary>
    /// マウスの左ボタンが離されたときのイベント
    /// </summary>
    public event MouseLeftButtonUpEventHandler MouseLeftButtonUp;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public WindowSelectionFrame()
    {
        HookCallback = MouseHookProcedure;
    }

    /// <summary>
    /// デストラクタ
    /// </summary>
    ~WindowSelectionFrame()
    {
        Dispose(false);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        System.GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 非公開Dispose
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(
        bool disposing
        )
    {
        if (Disposed == false)
        {
            Disposed = true;
            Delete();
        }
    }

    /// <summary>
    /// 削除
    /// </summary>
    private void Delete()
    {
        if (FrameWindow != null)
        {
            FrameWindow.Close();
            FrameWindow.Dispose();
            FrameWindow = null;
        }
        if (Handle != System.IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(Handle);
            Handle = System.IntPtr.Zero;
        }
    }

    /// <summary>
    /// フレームウィンドウ処理
    /// </summary>
    private void FrameWindowProcessing()
    {
        System.IntPtr nowSelectedHwnd;      // 今選択しているウィンドウのハンドル

        if ((nowSelectedHwnd = NativeMethods.WindowFromPoint(System.Windows.Forms.Control.MousePosition)) != System.IntPtr.Zero)
        {
            if ((nowSelectedHwnd = NativeMethods.GetAncestor(nowSelectedHwnd, 2)) != System.IntPtr.Zero)       // 2 = GA_ROOT (親ウィンドウのルートウィンドウを取得の値)
            {
                if ((nowSelectedHwnd != SelectedHwnd) && (nowSelectedHwnd != FrameWindow.Handle))
                {
                    SelectedHwnd = nowSelectedHwnd;

                    RECT windowRect;
                    NativeMethods.GetWindowRect(nowSelectedHwnd, out windowRect);
                    windowRect.right -= windowRect.left;
                    windowRect.bottom -= windowRect.top;

                    // 枠の位置とサイズを計算
                    WINDOWPLACEMENT windowPlacement;
                    NativeMethods.GetWindowPlacement(nowSelectedHwnd, out windowPlacement);
                    System.Drawing.Rectangle settingFrameWindowRectangle = new();
                    if (windowPlacement.showCmd == 3)      // 3 = SW_SHOWMAXIMIZED
                    {
                        System.Drawing.Rectangle screenRectangle = System.Windows.Forms.Screen.FromHandle(nowSelectedHwnd).WorkingArea;
                        settingFrameWindowRectangle.X = screenRectangle.Left;
                        settingFrameWindowRectangle.Y = screenRectangle.Top;
                        settingFrameWindowRectangle.Width = windowRect.right - ((screenRectangle.Left - windowRect.left) * 2);
                        settingFrameWindowRectangle.Height = windowRect.bottom - ((screenRectangle.Top - windowRect.top) * 2);
                    }
                    else
                    {
                        settingFrameWindowRectangle.X = windowRect.left;
                        settingFrameWindowRectangle.Y = windowRect.top;
                        settingFrameWindowRectangle.Width = windowRect.right;
                        settingFrameWindowRectangle.Height = windowRect.bottom;
                    }

                    // ディスプレイと同じサイズの場合は、全画面モード機能が反応しないようにサイズ調整
                    System.Drawing.Rectangle screen = System.Windows.Forms.Screen.FromHandle(nowSelectedHwnd).Bounds;
                    if ((screen.Left == settingFrameWindowRectangle.Left)
                        && (screen.Top == settingFrameWindowRectangle.Top)
                        && (screen.Right == settingFrameWindowRectangle.Right)
                        && (screen.Bottom == settingFrameWindowRectangle.Bottom))
                    {
                        settingFrameWindowRectangle.X += 1;
                        settingFrameWindowRectangle.Y += 1;
                        settingFrameWindowRectangle.Width -= 2;
                        settingFrameWindowRectangle.Height -= 2;
                    }

                    // パスを設定
                    System.Drawing.Drawing2D.GraphicsPath path = new();
                    path.AddRectangle(new(0, 0, settingFrameWindowRectangle.Width, settingFrameWindowRectangle.Height));     // 外側の四角形
                    path.AddRectangle(new(FrameWidth, FrameWidth, settingFrameWindowRectangle.Width - (FrameWidth + FrameWidth), settingFrameWindowRectangle.Height - (FrameWidth + FrameWidth)));       // 内側の四角形

                    FrameWindow.Location = new(settingFrameWindowRectangle.X, settingFrameWindowRectangle.Y);
                    FrameWindow.Size = new(settingFrameWindowRectangle.Width, settingFrameWindowRectangle.Height);
                    FrameWindow.Region = new(path);
                    FrameWindow.Opacity = 1.0;
                }
            }
        }
    }

    /// <summary>
    /// ウィンドウ選択開始
    /// </summary>
    /// <exception cref="StartWindowSelectionException">ウィンドウ選択開始の例外</exception>
    /// <exception cref="System.ComponentModel.Win32Exception">Win 32のエラーコードの例外</exception>
    public void StartWindowSelection()
    {
        try
        {
            if (Handle == System.IntPtr.Zero)
            {
                SelectedHwnd = System.IntPtr.Zero;
                FrameWindow = new FrameWindow
                {
                    BackColor = ColorFrameWindow,
                    Visible = false
                };
                FrameWindow.Show();

                Handle = NativeMethods.SetWindowsHookEx(14, HookCallback, System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetEntryAssembly().GetModules()[0]), 0);      // 14 = WH_MOUSE_LL
                if (Handle == System.IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
        }
        catch
        {
            Delete();
            throw new StartWindowSelectionException();
        }
    }

    /// <summary>
    /// ウィンドウ選択停止
    /// </summary>
    /// <exception cref="StopWindowSelectionException">ウィンドウ選択停止の例外</exception>
    public void StopWindowSelection()
    {
        try
        {
            Delete();
        }
        catch
        {
            throw new StopWindowSelectionException();
        }
    }

    /// <summary>
    /// マウスのフックプロシージャ
    /// </summary>
    /// <param name="nCode">フックコード</param>
    /// <param name="msg">フックプロシージャに渡す値</param>
    /// <param name="s">フックプロシージャに渡す値</param>
    /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
    private System.IntPtr MouseHookProcedure(
        int nCode,
        uint msg,
        ref MSLLHOOKSTRUCT s
        )
    {
        try
        {
            if (0 <= nCode)
            {
                switch (msg)
                {
                    case 0x0202:        // WM_LBUTTONUP
                        if (MouseLeftUpStop)
                        {
                            StopWindowSelection();
                            MouseLeftButtonUp?.Invoke(this, new MouseLeftButtonUpEventArgs());
                        }
                        break;
                    case 0x0200:      // WM_MOUSEMOVE
                        FrameWindowProcessing();
                        break;
                }
            }
        }
        catch
        {
        }

        return (NativeMethods.CallNextHookEx(Handle, nCode, msg, ref s));
    }
}
