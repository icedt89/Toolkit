namespace JanHafner.Toolkit.Windows.Hooks.Tests
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using JanHafner.Toolkit.Windows.Hooks.Mouse;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LowlevelMouseWindowsHook
    {
        [TestMethod]
        public void LeftMouseClickIsRecognized()
        {
            // Arrange
            var autoResetEvent = new AutoResetEvent(false);
            var leftMouseButtonDownOccured = false;
            var leftMouseButtonUpOccured = false;
            var leftMouseClickOccured = false;

            using (
                var dummyLowLevelMouseWindowsHook =
                    new DummyLowLevelMouseWindowsHook((identifier, kbdllhookstruct) =>
                    {
                        if (identifier == LowLevelMouseMessageIdentifier.WM_LBUTTONDOWN)
                        {
                            leftMouseButtonDownOccured = true;
                        }

                        if (identifier == LowLevelMouseMessageIdentifier.WM_LBUTTONUP)
                        {
                            leftMouseButtonUpOccured = true;
                        }

                        if (leftMouseButtonDownOccured && leftMouseButtonUpOccured)
                        {
                            leftMouseClickOccured = autoResetEvent.Set();
                        }
                    }))
            {

                dummyLowLevelMouseWindowsHook.Install();

                // Act
                dummyLowLevelMouseWindowsHook.SimulateLeftMouseClick();

                autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

                // Assert
                leftMouseClickOccured.Should().BeTrue();
            }
        }

        [TestMethod]
        public void UninstallWillFailIfHookAlreadyUninstalled()
        {
            // Arrange
            var dummyLowLevelMouseWindowsHook = new DummyLowLevelMouseWindowsHook((identifier, kbdllhookstruct) => { });

            dummyLowLevelMouseWindowsHook.Install();

            // Act
            dummyLowLevelMouseWindowsHook.Uninstall();

            // Assert
            dummyLowLevelMouseWindowsHook.Invoking(h => h.Uninstall()).Should().Throw<ObjectDisposedException>();
        }

        [TestMethod]
        public void UninstallWillFailIfHookWasNeverInstalled()
        {
            // Arrange
            var dummyLowLevelMouseWindowsHook = new DummyLowLevelMouseWindowsHook((identifier, kbdllhookstruct) => { });

            // Act
            dummyLowLevelMouseWindowsHook.Uninstall();

            // Assert
            dummyLowLevelMouseWindowsHook.Invoking(h => h.Uninstall()).Should().Throw<ObjectDisposedException>();
        }

        [TestMethod]
        public void DisposeWillNotFailIfHookWasAlreadyDisposed()
        {
            // Arrange
            DummyLowLevelMouseWindowsHook dummyLowLevelMouseWindowsHook = null;
            using (dummyLowLevelMouseWindowsHook = new DummyLowLevelMouseWindowsHook((identifier, kbdllhookstruct) => { }))
            {
                dummyLowLevelMouseWindowsHook.Install();
            }

            // Act, Assert
            dummyLowLevelMouseWindowsHook.Invoking(h => h.Dispose()).Should().NotThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void DisposeWillNotFailIfHookWasNeverInstalled()
        {
            // Arrange
            var dummyLowLevelMouseWindowsHook = new DummyLowLevelMouseWindowsHook((identifier, kbdllhookstruct) => { });

            // Act, Assert
            dummyLowLevelMouseWindowsHook.Invoking(h => h.Dispose()).Should().NotThrow<ObjectDisposedException>();
        }
    }
}