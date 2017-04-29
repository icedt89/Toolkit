namespace JanHafner.Toolkit.Windows.Hooks.Mouse
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class LowLevelMouseWindowsHook : WindowsHook
    {
        protected LowLevelMouseWindowsHook() 
            : base(HookType.WH_MOUSE_LL)
        {
        }

        protected override void HandleHook(HookCode nCode, IntPtr wParam, IntPtr lParam)
        {
            var mouseMessageIdentifier = (LowLevelMouseMessageIdentifier) wParam.ToInt32();
            var lowlevelMouseHookStructure = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            this.Handle(mouseMessageIdentifier, lowlevelMouseHookStructure);
        }

        protected abstract void Handle(LowLevelMouseMessageIdentifier mouseMessageIdentifier,
            MSLLHOOKSTRUCT lowlevelMouseHookStructure);
    }
}