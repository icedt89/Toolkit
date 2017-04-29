namespace JanHafner.Toolkit.Windows.Hooks.Keyboard
{
    using System;

    public sealed class DebugLowLevelKeyboardHook : LowLevelKeyboardWindowsHook
    {
        protected override void Handle(LowLevelKeyboardMessageIdentifier keyboardMessageIdentifier, KBDLLHOOKSTRUCT lowLevelKeyboardHookStructure)
        {
            Console.WriteLine($"{keyboardMessageIdentifier} {lowLevelKeyboardHookStructure.Flags} {lowLevelKeyboardHookStructure.VirtualKeyCode}");
        }
    }
}