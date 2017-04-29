namespace JanHafner.Toolkit.Wpf.Behavior
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// .NET representation of the Win32 RECT-structure.
    /// </summary>
    [DebuggerDisplay("RECT: left={left}, top={top}, right={right}, bottom={bottom}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public Int32 left;

        public Int32 top;

        public Int32 right;

        public Int32 bottom;
    }
}
