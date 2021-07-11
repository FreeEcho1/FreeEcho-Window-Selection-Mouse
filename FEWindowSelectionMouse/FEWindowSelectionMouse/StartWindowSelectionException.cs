namespace FreeEcho
{
    namespace FEWindowSelectionMouse
    {
        /// <summary>
        /// ウィンドウ選択開始の例外
        /// </summary>
        public class StartWindowSelectionException : System.Exception
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public StartWindowSelectionException()
                : base("Failed to start window selection.")
            {
            }
        }
    }
}
