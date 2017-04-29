namespace JanHafner.Toolkit.Windows.Hooks.Mouse
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public Int32 X;

        public Int32 Y;
    }
}