namespace JanHafner.Toolkit.Windows.Hooks.Tests
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using JanHafner.Toolkit.Windows.Hooks.Mouse;

    internal sealed class DummyLowLevelMouseWindowsHook : LowLevelMouseWindowsHook
    {
        private readonly Action<LowLevelMouseMessageIdentifier, MSLLHOOKSTRUCT> handle;

        public DummyLowLevelMouseWindowsHook(Action<LowLevelMouseMessageIdentifier, MSLLHOOKSTRUCT> handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            this.handle = handle;
        }

        protected override void Handle(LowLevelMouseMessageIdentifier mouseMessageIdentifier, MSLLHOOKSTRUCT lowlevelMouseHookStructure)
        {
            this.handle(mouseMessageIdentifier, lowlevelMouseHookStructure);
        }

        public void SimulateLeftMouseClick()
        {
            var inputs = new[]
            {
                new SendInputHelper.INPUT
                {
                    Type = 0,
                    Data = new SendInputHelper.UNIONINPUT
                    {
                        MouseInput = new SendInputHelper.MOUSEINPUT
                        {
                            Flags = 2,
                            ExtraInfo = SendInputHelper.NativeMethods.GetMessageExtraInfo()
                        }
                    }
                },
                new SendInputHelper.INPUT
                {
                    Type = 0,
                    Data = new SendInputHelper.UNIONINPUT
                    {
                        MouseInput = new SendInputHelper.MOUSEINPUT
                        {
                            Flags = 4,
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