namespace JanHafner.Toolkit.Windows.HotKey
{
    using System;

    /// <summary>
    /// Defines possible modifier keys for use in RegisterHotKey(...).
    /// </summary>
    [Flags]
    public enum HotKeyModifier : uint
    {
        /// <summary>
        /// Represents no valid modifier.
        /// </summary>
        None = 0,

        /// <summary>
        /// Either ALT key must be held down.
        /// </summary>
        Alt = 1,

        /// <summary>
        /// Either CTRL key must be held down.
        /// </summary>
        Control = 2,

        /// <summary>
        /// Either SHIFT key must be held down.
        /// </summary>
        Shift = 4
    }
}