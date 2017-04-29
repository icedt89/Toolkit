namespace JanHafner.Toolkit.Windows.Hooks.Keyboard
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information about a low-level keyboard input event.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        /// <summary>
        /// A virtual-key code. The code must be a value in the range 1 to 254.
        /// </summary>
        public Int32 VirtualKeyCode;

        /// <summary>
        /// A hardware scan code for the key.
        /// </summary>
        public Int32 ScanCode;

        /// <summary>
        /// The extended-key flag, event-injected flags, context code, and transition-state flag. 
        /// </summary>
        public KBDLLHOOKSTRUCTFlags Flags;

        /// <summary>
        /// The time stamp for this message, equivalent to what <c>GetMessageTime</c> would return for this message.
        /// </summary>
        public Int32 Time;

        /// <summary>
        /// Additional information associated with the message.
        /// </summary>
        public IntPtr ExtraInfo;
    }
}