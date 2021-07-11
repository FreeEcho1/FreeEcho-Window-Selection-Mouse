namespace FreeEcho
{
    namespace FEWindowSelectionMouse
    {
        /// <summary>
        /// ウィンドウ選択停止の例外
        /// </summary>
        public class StopWindowSelectionException : System.Exception
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public StopWindowSelectionException()
                : base("Failed to stop window selection.")
            {
            }
        }
    }
}
