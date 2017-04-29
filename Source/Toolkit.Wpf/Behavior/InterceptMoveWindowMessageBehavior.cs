namespace JanHafner.Toolkit.Wpf.Behavior
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Base class for behaviors which intercept the WM_MOVE message sent to a WPF-<see cref="Window"/>.
    /// </summary>
    public abstract class InterceptMoveWindowMessageBehavior : InterceptWndProcBehavior
    {
        #region

        public static readonly DependencyProperty TemporaryDisableBehaviorOnModifierKeysProperty = DependencyProperty.Register(
            "TemporaryDisableBehaviorOnModifierKeys", typeof (Boolean), typeof (InterceptMoveWindowMessageBehavior), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value, indicating whether, enable or disable the behavior.
        /// The inheritor must manually check for this value and react to it.
        /// </summary>
        public Boolean TemporaryDisableBehaviorOnModifierKeys
        {
            get { return (Boolean) this.GetValue(TemporaryDisableBehaviorOnModifierKeysProperty); }
            set { this.SetValue(TemporaryDisableBehaviorOnModifierKeysProperty, value); }
        }

        #endregion

        #region ModifierKeys

        public static readonly DependencyProperty ModifierKeysProperty = DependencyProperty.Register(
            "ModifierKeys", typeof (ModifierKeys), typeof (InterceptMoveWindowMessageBehavior), new PropertyMetadata(default(ModifierKeys)));

        /// <summary>
        /// Gets or sets the modifier keys which must be used while examining the <see cref="TemporaryDisableBehaviorOnModifierKeys"/> property.
        /// </summary>
        public ModifierKeys ModifierKeys
        {
            get { return (ModifierKeys) this.GetValue(ModifierKeysProperty); }
            set { this.SetValue(ModifierKeysProperty, value); }
        }

        #endregion

        protected abstract Boolean ManipulatePosition(ref RECT rect);

        protected sealed override IntPtr InterceptWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            if (msg == (Int32)WindowMessage.WM_MOVE && this.Enabled)
            {
                var rect = Marshal.PtrToStructure<RECT>(lParam);
                handled = this.ManipulatePosition(ref rect);
                Marshal.StructureToPtr(rect, lParam, true);
            }

            return IntPtr.Zero;
        }
    }
}