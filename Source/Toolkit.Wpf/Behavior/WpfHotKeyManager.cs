namespace JanHafner.Toolkit.Wpf.Behavior
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using JanHafner.Toolkit.Windows.HotKey;
    using JetBrains.Annotations;

    public sealed class WpfHotKeyManager : HotKeyManager
    {
        [CanBeNull]
        private HwndSource windowHandle;

        public WpfHotKeyManager()
        {
        }

        public WpfHotKeyManager([NotNull] Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            this.BindToWindow(window);
        }

        public void BindToWindow([NotNull] Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (this.windowHandle != null)
            {
                // TODO: ExceptionMessage
                throw new InvalidOperationException();
            }

            this.windowHandle = (HwndSource)PresentationSource.FromDependencyObject(window);
            if (this.windowHandle == null)
            {
                // TODO: ExceptionMessage
                throw new InvalidOperationException();
            }
        }

        public static Boolean ProbeHotKey(HotKeyModifier hotKeyModifier, Key hotKey)
        {
            var virtualkeyCode = KeyInterop.VirtualKeyFromKey(hotKey);

            return HotKeyManager.ProbeHotKey(hotKeyModifier, (UInt32)virtualkeyCode);
        }

        private Boolean IsBoundToWindow
        {
            get
            {
                return this.windowHandle != null && !this.windowHandle.IsDisposed;
            }
        }

        private void CheckBinding()
        {
            if (!this.IsBoundToWindow)
            {
                // TODO: ExceptionMessage
                throw new InvalidOperationException();
            }
        }

        public Int32 Register(HotKeyModifier hotKeyModifier, Key hotKey, [NotNull] Action action)
        {
            this.CheckBinding();  

            var virtualkeyCode = KeyInterop.VirtualKeyFromKey(hotKey);

            return base.Register(this.windowHandle.Handle, hotKeyModifier, (UInt32)virtualkeyCode, action);
        }

        public void Unregister(HotKeyModifier hotKeyModifier, Key hotKey)
        {
            var virtualkeyCode = KeyInterop.VirtualKeyFromKey(hotKey);

            base.Unregister(hotKeyModifier, (UInt32)virtualkeyCode);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                this.windowHandle?.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
