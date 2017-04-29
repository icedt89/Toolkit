namespace JanHafner.Toolkit.Wpf.Behavior
{
    /// <summary>
    /// Defines window messages which are send from windows to a native window.
    /// </summary>
    public enum WindowMessage
    {
        /// <summary>
        /// The window is moving around.
        /// </summary>
        WM_MOVE = 534,

        /// <summary>
        /// Is send to a window if a hot key is triggered which is registered to the window.
        /// </summary>
        WM_HOTKEY = 786
    }
}
