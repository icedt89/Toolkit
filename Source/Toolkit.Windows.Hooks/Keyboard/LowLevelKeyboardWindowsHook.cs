namespace JanHafner.Toolkit.Windows.Hooks.Keyboard
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class LowLevelKeyboardWindowsHook : WindowsHook
    {
        protected LowLevelKeyboardWindowsHook() 
            : base(HookType.WH_KEYBOARD_LL)
        {
        }

        protected override void HandleHook(HookCode nCode, IntPtr wParam, IntPtr lParam)
        {
            var keyboardMessageIdentifier = (LowLevelKeyboardMessageIdentifier) wParam.ToInt32();
            var lowLevelKeyboardHookStructure = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            this.Handle(keyboardMessageIdentifier, lowLevelKeyboardHookStructure);
        }

        protected abstract void Handle(LowLevelKeyboardMessageIdentifier keyboardMessageIdentifier, KBDLLHOOKSTRUCT lowLevelKeyboardHookStructure);
    }
}