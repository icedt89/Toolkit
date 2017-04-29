namespace JanHafner.Toolkit.Windows.HotKey.Tests
{
    using System;
    using JanHafner.Toolkit.Windows.HotKey;

    internal sealed class DummyHotKeyManager : JanHafner.Toolkit.Windows.HotKey.HotKeyManager
    {
        public Int32 Register(DummyWindow dummyWindow, HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode, Action action)
        {
            return this.Register(dummyWindow.WindowHandle, hotKeyModifier, virtualKeyCode, action);
        }

        public Int32? FindHotKeyId(HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            return base.FindHotKeyId(hotKeyModifier, virtualKeyCode);
        }
    }
}
