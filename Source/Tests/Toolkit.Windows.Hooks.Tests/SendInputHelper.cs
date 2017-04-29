namespace JanHafner.Toolkit.Windows.Hooks.Tests
{
    using System;
    using System.Runtime.InteropServices;

    internal static class SendInputHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public UInt32 Type;

            public UNIONINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct UNIONINPUT
        {
            [FieldOffset(0)]
            public HARDWAREINPUT HardwareInput;

            [FieldOffset(0)]
            public KEYBDINPUT KeyboardInput;

            [FieldOffset(0)]
            public MOUSEINPUT MouseInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public UInt16 VirtualKeyCode;

            public UInt16 Scan;

            public UInt32 Flags;

            public UInt32 Time;

            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public UInt32 Message;

            public UInt16 WParam;

            public UInt16 LParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public Int32 X;

            public Int32 Y;

            public UInt32 MouseData;

            public UInt32 Flags;

            public UInt32 Time;

            public IntPtr ExtraInfo;
        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern UInt32 SendInput(UInt32 inputsCount, INPUT[] inputs, Int32 sizeOfInput);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetMessageExtraInfo();
        }
    }
}
