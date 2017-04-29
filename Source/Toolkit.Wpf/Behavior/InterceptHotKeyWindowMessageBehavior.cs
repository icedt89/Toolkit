using System;

namespace JanHafner.Toolkit.Wpf.Behavior
{
    using System.Windows;
    using JanHafner.Toolkit.Windows.HotKey;

    public sealed class InterceptHotKeyWindowMessageBehavior : InterceptWndProcBehavior
    {
        #region HotKeyManager

        public static readonly DependencyProperty HotKeyManagerProperty = DependencyProperty.Register(
            "HotKeyManager", typeof (HotKeyManager), typeof (InterceptHotKeyWindowMessageBehavior), new PropertyMetadata(default(HotKeyManager)));

        public HotKeyManager HotKeyManager
        {
            get { return (HotKeyManager) this.GetValue(HotKeyManagerProperty); }
            set { this.SetValue(HotKeyManagerProperty, value); }
        }

        #endregion

        protected override IntPtr InterceptWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            if (msg == (Int32)WindowMessage.WM_HOTKEY && this.HotKeyManager != null && this.Enabled)
            {
                var hotKeyModifier = ExtractHotKeyModifierFromLParam(lParam);
                var virtualKeyCode = ExtractVirtualKeyCodeFromLParam(lParam);

                handled = this.HotKeyManager.HandleHotKey(hotKeyModifier, virtualKeyCode);
            }

            return IntPtr.Zero;
        }

        private static HotKeyModifier ExtractHotKeyModifierFromLParam(IntPtr lParam)
        {
            return (HotKeyModifier)((Int32)lParam & 0xFFFF);
        }

        private static UInt32 ExtractVirtualKeyCodeFromLParam(IntPtr lParam)
        {
            return (UInt32)((Int32)lParam >> 16) & 0xFFFF;
        }
    }
}
