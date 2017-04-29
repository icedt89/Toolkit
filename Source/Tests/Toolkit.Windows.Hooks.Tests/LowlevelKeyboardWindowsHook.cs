namespace JanHafner.Toolkit.Windows.Hooks.Tests
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using JanHafner.Toolkit.Windows.Hooks.Keyboard;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LowlevelKeyboardWindowsHook
    {
        [TestMethod]
        public void KeySequenceIsRecognized()
        {
            // Arrange
            var autoResetEvent = new AutoResetEvent(false);
            var keyOOccured = false;
            var keyKOccured = false;
            var okSequenceOccured = false;

            var oKey = 79;
            var kKey = 75;

            using (var dummyLowLevelKeyboardWindowsHook = new DummyLowLevelKeyboardWindowsHook(
                (identifier, kbdllhookstruct) =>
                {
                    if (identifier == LowLevelKeyboardMessageIdentifier.WM_KEYUP && kbdllhookstruct.VirtualKeyCode == oKey)
                    {
                        keyOOccured = true;
                    }

                    if (identifier == LowLevelKeyboardMessageIdentifier.WM_KEYUP && kbdllhookstruct.VirtualKeyCode == kKey)
                    {
                        keyKOccured = true;
                    }

                    if (keyOOccured && keyKOccured)
                    {
                        okSequenceOccured = autoResetEvent.Set();
                    }
                }))
            {

                dummyLowLevelKeyboardWindowsHook.Install();

                // Act
                dummyLowLevelKeyboardWindowsHook.SimulateKeyPress((UInt16)oKey);
                dummyLowLevelKeyboardWindowsHook.SimulateKeyPress((UInt16)kKey);

                autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

                // Assert
                okSequenceOccured.Should().BeTrue();
            }
        }

        [TestMethod]
        public void UninstallWillFailIfHookAlreadyUninstalled()
        {
            // Arrange
            var dummyLowLevelKeyboardWindowsHook = new DummyLowLevelKeyboardWindowsHook((identifier, kbdllhookstruct) => { });

            dummyLowLevelKeyboardWindowsHook.Install();

            // Act
            dummyLowLevelKeyboardWindowsHook.Uninstall();

            // Assert
            dummyLowLevelKeyboardWindowsHook.Invoking(h => h.Uninstall()).ShouldThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void UninstallWillFailIfHookWasNeverInstalled()
        {
            // Arrange
            var dummyLowLevelKeyboardWindowsHook = new DummyLowLevelKeyboardWindowsHook((identifier, kbdllhookstruct) => { });

            // Act
            dummyLowLevelKeyboardWindowsHook.Uninstall();

            // Assert
            dummyLowLevelKeyboardWindowsHook.Invoking(h => h.Uninstall()).ShouldThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void DisposeWillNotFailIfHookWasAlreadyDisposed()
        {
            // Arrange
            DummyLowLevelKeyboardWindowsHook dummyLowLevelKeyboardWindowsHook = null;
            using (dummyLowLevelKeyboardWindowsHook = new DummyLowLevelKeyboardWindowsHook((identifier, kbdllhookstruct) => { }))
            {
                dummyLowLevelKeyboardWindowsHook.Install();
            }

            // Act, Assert
            dummyLowLevelKeyboardWindowsHook.Invoking(h => h.Dispose()).ShouldNotThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void DisposeWillNotFailIfHookWasNeverInstalled()
        {
            // Arrange
            var dummyLowLevelKeyboardWindowsHook = new DummyLowLevelKeyboardWindowsHook((identifier, kbdllhookstruct) => { });

            // Act, Assert
            dummyLowLevelKeyboardWindowsHook.Invoking(h => h.Dispose()).ShouldNotThrow<ObjectDisposedException>();
        }
    }
}