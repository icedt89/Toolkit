namespace JanHafner.Toolkit.Windows.Hooks.Tests
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using JanHafner.Toolkit.Windows.Hooks.Keyboard;

    internal sealed class DummyLowLevelKeyboardWindowsHook : LowLevelKeyboardWindowsHook
    {
        private readonly Action<LowLevelKeyboardMessageIdentifier, KBDLLHOOKSTRUCT> handle;

        public DummyLowLevelKeyboardWindowsHook(Action<LowLevelKeyboardMessageIdentifier, KBDLLHOOKSTRUCT> handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            this.handle = handle;
        }

        protected override void Handle(LowLevelKeyboardMessageIdentifier keyboardMessageIdentifier, KBDLLHOOKSTRUCT lowLevelKeyboardHookStructure)
        {
            this.handle(keyboardMessageIdentifier, lowLevelKeyboardHookStructure);
        }

        public void SimulateKeyPress(UInt16 virtualKeyCode)
        {
            var inputs = new[]
            {
                new SendInputHelper.INPUT
                {
                    Type = 1,
                    Data = new SendInputHelper.UNIONINPUT
                    {
                        KeyboardInput = new SendInputHelper.KEYBDINPUT
                        {
                            VirtualKeyCode = virtualKeyCode,
                            ExtraInfo = SendInputHelper.NativeMethods.GetMessageExtraInfo()
                        }
                    }
                },
                 new SendInputHelper.INPUT
                {
                    Type = 1,
                    Data = new SendInputHelper.UNIONINPUT
                    {
                        KeyboardInput = new SendInputHelper.KEYBDINPUT
                        {
                            Flags = 2,
                            VirtualKeyCode = virtualKeyCode,
                            ExtraInfo = SendInputHelper.NativeMethods.GetMessageExtraInfo()
                        }
                    }
                }
            };

            var sizeOfInput = Marshal.SizeOf<SendInputHelper.INPUT>();

            var result = SendInputHelper.NativeMethods.SendInput((UInt32) inputs.Length, inputs, sizeOfInput);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (result != inputs.Length)
            {
                throw new Win32Exception(lastWin32Error);
            }
        }
    }
}