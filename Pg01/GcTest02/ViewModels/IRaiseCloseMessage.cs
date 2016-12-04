namespace GcTest.ViewModels
{
    /// <summary>
    ///     ウィンドウを閉じるためのビューモデルのインターフェイスです。
    /// </summary>
    public interface IRaiseCloseMessage
    {
        /// <summary>
        ///     ビューモデルからウィンドウへ Close メッセージを通知するメソッドです。
        /// </summary>
        void RaiseCloseMessage();
    }
}