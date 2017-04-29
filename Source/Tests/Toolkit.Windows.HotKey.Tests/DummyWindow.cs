namespace JanHafner.Toolkit.Windows.HotKey.Tests
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public class DummyWindow : IDisposable
    {
        protected DummyWindow()
        {
        }

        private DummyWindow(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(windowHandle));
            }

            this.WindowHandle = windowHandle;
        }

        public IntPtr WindowHandle { get; protected set; }

        public static DummyWindow Create()
        {
            var result = NativeMethods.CreateWindowEx(0, "Message", "RegisterDummyWindow", 134217728, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (result == IntPtr.Zero)
            {
                throw new Win32Exception(lastWin32Error);
            }

            return new DummyWindow(result);
        }

        public virtual void Dispose()
        {
            var result = NativeMethods.DestroyWindow(this.WindowHandle);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (!result)
            {
                throw new Win32Exception(lastWin32Error);
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateWindowEx(UInt32 exStyle, String className, String windowName, UInt32 windowStyle, Int32 x, Int32 y, Int32 width, Int32 height, IntPtr parentWindowHandle, IntPtr menuHandle, IntPtr instanceHandle, IntPtr param);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern Boolean DestroyWindow(IntPtr windowHandle);
        }
    }
}
