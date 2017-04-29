namespace JanHafner.Toolkit.Windows.HotKey.Tests
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public sealed class DummyWindowWithMessageLoop : DummyWindow
    {
        private readonly Action<IntPtr, UInt32, IntPtr, IntPtr> wndProc;

        private readonly UInt16 registeredClassHandle;

        public DummyWindowWithMessageLoop(Action<IntPtr, UInt32, IntPtr, IntPtr> wndProc)
            : base()
        {
            if (wndProc == null)
            {
                throw new ArgumentNullException(nameof(wndProc));
            }

            this.wndProc = wndProc;
            var windowClassEx = NativeMethods.WNDCLASSEX.MakeNew();
            windowClassEx.lpfnWndProc = this.WndProc;
            windowClassEx.style = 3;
            windowClassEx.lpszClassName = Guid.NewGuid().ToString();

            var classId = NativeMethods.RegisterClassEx(ref windowClassEx);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (classId == 0)
            {
                throw new Win32Exception(lastWin32Error);
            }

            var windowHandle = NativeMethods.CreateWindowEx(0, classId, "RegisterDummyWindowWithMessageLoop", 134217728, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            lastWin32Error = Marshal.GetLastWin32Error();
            if (windowHandle == IntPtr.Zero)
            {
                var unregisterClassResult = NativeMethods.UnregisterClass(classId, IntPtr.Zero);
                var unregisterClassLastWin32Error = Marshal.GetLastWin32Error();
                if (!unregisterClassResult)
                {
                    throw new Win32Exception(unregisterClassLastWin32Error);
                }

                throw new Win32Exception(lastWin32Error);
            }

            this.registeredClassHandle = classId;
            this.WindowHandle = windowHandle;
        }

        public void SendMessage(UInt32 message, IntPtr wParam, IntPtr lParam)
        {
            NativeMethods.SendMessage(this.WindowHandle, message, wParam, lParam);
        }

        private IntPtr WndProc(IntPtr windowHandle, UInt32 message, IntPtr wParam, IntPtr lParam)
        {
            this.wndProc(windowHandle, message, wParam, lParam);

            return NativeMethods.DefWindowProc(windowHandle, message, wParam, lParam);
        }

        public override void Dispose()
        {
            var result = NativeMethods.DestroyWindow(this.WindowHandle);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (!result)
            {
                throw new Win32Exception(lastWin32Error);
            }

            result = NativeMethods.UnregisterClass(this.registeredClassHandle, IntPtr.Zero);
            lastWin32Error = Marshal.GetLastWin32Error();
            if (!result)
            {
                throw new Win32Exception(lastWin32Error);
            }
        }

        private static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct WNDCLASSEX
            {
                public UInt32 cbSize;

                public Int32 style;

                public WndProc lpfnWndProc;

                public Int32 cbClsExtra;

                public Int32 cbWndExtra;

                public IntPtr hInstance;

                public IntPtr hIcon;

                public IntPtr hCursor;

                public IntPtr hbrBackground;

                public String lpszMenuName;

                public String lpszClassName;

                public IntPtr hIconSm;

                public static WNDCLASSEX MakeNew()
                {
                    return new WNDCLASSEX
                    {
                        cbSize = (UInt32)Marshal.SizeOf(typeof(WNDCLASSEX))
                    };
                }
            }

            public delegate IntPtr WndProc(IntPtr windowHandle, UInt32 message, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr DefWindowProc(IntPtr windowHandle, UInt32 message, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateWindowEx(UInt32 exStyle, UInt16 className, String windowName, UInt32 windowStyle, Int32 x, Int32 y, Int32 width, Int32 height, IntPtr parentWindowHandle, IntPtr menuHandle, IntPtr instanceHandle, IntPtr param);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern Boolean DestroyWindow(IntPtr windowHandle);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern UInt16 RegisterClassEx(ref WNDCLASSEX lpwcx);

            [DllImport("user32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode)]
            public static extern Boolean UnregisterClass(UInt16 className, IntPtr instanceHandle);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SendMessage(IntPtr windowHandle, UInt32 message, IntPtr wParam, IntPtr lParam);
        }
    }
}
