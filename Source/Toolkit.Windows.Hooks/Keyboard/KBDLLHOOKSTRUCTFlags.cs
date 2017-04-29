namespace JanHafner.Toolkit.Windows.Hooks.Keyboard
{
    using System;

    [Flags]
    public enum KBDLLHOOKSTRUCTFlags
    {
        LLKHF_EXTENDED = 256 >> 8,

        LLKHF_LOWER_IL_INJECTED = 2,

        LLKHF_INJECTED = 16,

        LLKHF_ALTDOWN = 8192 >> 8,

        LLKHF_UP = 32768 >> 8
    }
}