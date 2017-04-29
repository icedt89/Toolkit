namespace JanHafner.Toolkit.Wpf.Tests
{
    using System;
    using JanHafner.Toolkit.Wpf.Behavior;

    public sealed class DummyInterceptMoveWindowMessageBehavior : Behavior.InterceptMoveWindowMessageBehavior
    {
        protected override Boolean ManipulatePosition(ref RECT rect)
        {
            return this.DummyManipulatePosition(ref rect);
        }

        public Boolean DummyManipulatePosition(ref RECT rect)
        {
            return true;
        }

        public IntPtr DummyInterceptWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            return this.InterceptWndProc(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}
