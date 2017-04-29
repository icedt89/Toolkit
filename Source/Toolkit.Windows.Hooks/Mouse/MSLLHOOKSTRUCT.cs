﻿namespace JanHafner.Toolkit.Windows.Hooks.Mouse
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        /// <summary>
        /// The x- and y-coordinates of the cursor, in screen coordinates.
        /// </summary>
        public POINT Point;

        /// <summary>
        /// If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. The low-order word is reserved. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120.
        /// If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, and the low-order word is reserved.This value can be one or more of the following values.Otherwise, mouseData is not used.
        /// </summary>
        public Int32 MouseData;

        /// <summary>
        /// The event-injected flags. An application can use the following values to test the flags. Testing LLMHF_INJECTED (bit 0) will tell you whether the event was injected. If it was, then testing LLMHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
        /// </summary>
        public MSLLHOOKSTRUCTFlags Flags;

        /// <summary>
        /// The time stamp for this message.
        /// </summary>
        public Int32 Time;

        /// <summary>
        /// Additional information associated with the message.
        /// </summary>
        public IntPtr ExtraInfo;
    }
}