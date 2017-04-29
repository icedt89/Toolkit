namespace JanHafner.Toolkit.Windows.HotKey.Tests
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using JanHafner.Toolkit.Windows.HotKey;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class HotKeyManager
    {
        [TestMethod]
        public void WindowReceivedWmHotkey()
        {
            const Int32 WM_HotKey = 786;
            var autoResetEvent = new AutoResetEvent(false);
            var wmHotKeyReceived = false;
            
            using (var dummyWindowWithMessageLoop = new DummyWindowWithMessageLoop((windowHandle, message, wParam, lParam) =>
            {
                if (message == WM_HotKey)
                {
                    wmHotKeyReceived = autoResetEvent.Set();
                }
            }))
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    Action hotKeyAction = () => {};

                    dummyHotKeyManager.Register(dummyWindowWithMessageLoop, hotKeyModifier, virtualKeyCode, hotKeyAction);

                    // Act
                    dummyWindowWithMessageLoop.SendMessage(WM_HotKey, IntPtr.Zero, IntPtr.Zero);

                    // Wait one second to receive the sent message
                    autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

                    // Assert
                    wmHotKeyReceived.Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void RegisterWillFailIfActionIsNull()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    Action hotKeyAction = null;

                    // Act, Assert
                    dummyHotKeyManager.Invoking(hkm => hkm.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction)).ShouldThrow<ArgumentNullException>();
                }
            }
        }

        [TestMethod]
        public void RegisterWillRegisterTheHotKey()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    Action hotKeyAction = () => { };

                    // Act
                    var hotKeyId = dummyHotKeyManager.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction);

                    // Assert
                    hotKeyId.Should().NotBe(0);
                }
            }
        }

        [TestMethod]
        public void FindHotKeyIdWillReturnIdIfRegistered()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    Action hotKeyAction = () => { };
                    var hotKeyId = dummyHotKeyManager.Register(dummyWindow, hotKeyModifier, virtualKeyCode,  hotKeyAction);

                    // Act
                    var registeredHotKeyId = dummyHotKeyManager.FindHotKeyId(hotKeyModifier, virtualKeyCode);

                    // Assert
                    registeredHotKeyId.Should().Be(hotKeyId);
                }
            }
        }

        [TestMethod]
        public void FindHotKeyIdWillReturnNullIfNotRegistered()
        {
            using (var dummyHotKeyManager = new DummyHotKeyManager())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                var virtualKeyCode = (UInt32)65; // A

                // Act
                var registeredHotKeyId = dummyHotKeyManager.FindHotKeyId(hotKeyModifier, virtualKeyCode);

                // Assert
                registeredHotKeyId.Should().Be(null);
            }
        }

        [TestMethod]
        public void HasRegisteredHotKeysWillReturnTrueIfRegistrationWasSuccessfull()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    Action hotKeyAction = () => { };
                    dummyHotKeyManager.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction);

                    // Act
                    var hasRegisteredHotKeys = dummyHotKeyManager.HasRegisteredHotKeys;

                    // Assert
                    hasRegisteredHotKeys.Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void HasRegisteredHotKeysWillReturnFalseIfNoHotKeyWasRegistered()
        {
            using (var dummyHotKeyManager = new DummyHotKeyManager())
            {
                // Act
                var hasRegisteredHotKeys = dummyHotKeyManager.HasRegisteredHotKeys;

                // Assert
                hasRegisteredHotKeys.Should().BeFalse();
            }
        }

        [TestMethod]
        public void RegisterWillFailIfDisposed()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                var virtualKeyCode = (UInt32) 65; // A
                Action hotKeyAction = () => { };
                DummyHotKeyManager dummyHotKeyManager = null;
                using (dummyHotKeyManager = new DummyHotKeyManager())
                {
                }

                // Act, Assert
                dummyHotKeyManager.Invoking(hkm => hkm.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction)).ShouldThrow<ObjectDisposedException>();
            }
        }

        [TestMethod]
        public void UnregisterByIdWillFailIfDisposed()
        {
            // Arrange
            var hotKeyId = 0;
            DummyHotKeyManager dummyHotKeyManager = null;
            using (dummyHotKeyManager = new DummyHotKeyManager())
            {
            }

            // Act, Assert
            dummyHotKeyManager.Invoking(hkm => hkm.Unregister(hotKeyId)).ShouldThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void UnregisterByIdWillUnregisterTheHotKey()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    Action hotKeyAction = () => { };
                    var hotKeyId = dummyHotKeyManager.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction);

                    // Act
                    dummyHotKeyManager.Unregister(hotKeyId);

                    // Assert
                    dummyHotKeyManager.HasRegisteredHotKeys.Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void UnregisterByModifierAndKeyCodeWillFailIfDisposed()
        {
            // Arrange
            var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
            var virtualKeyCode = (UInt32)65; // A
            DummyHotKeyManager dummyHotKeyManager = null;
            using (dummyHotKeyManager = new DummyHotKeyManager())
            {
            }

            // Act, Assert
            dummyHotKeyManager.Invoking(hkm => hkm.Unregister(hotKeyModifier, virtualKeyCode)).ShouldThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void UnregisterByModifierAndKeyCodeWillUnregisterTheHotKey()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    Action hotKeyAction = () => { };
                    dummyHotKeyManager.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction);

                    // Act
                    dummyHotKeyManager.Unregister(hotKeyModifier, virtualKeyCode);

                    // Assert
                    dummyHotKeyManager.HasRegisteredHotKeys.Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void HandleHotKeyWillFailIfDisposed()
        {
            // Arrange
            var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
            var virtualKeyCode = (UInt32)65; // A
            DummyHotKeyManager dummyHotKeyManager = null;
            using (dummyHotKeyManager = new DummyHotKeyManager())
            {
            }

            // Act, Assert
            dummyHotKeyManager.Invoking(hkm => hkm.HandleHotKey(hotKeyModifier, virtualKeyCode)).ShouldThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void HandleHotKeyWillReturnFalseIfHotKeyWasNotFound()
        {
            using (var dummyHotKeyManager = new DummyHotKeyManager())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                var virtualKeyCode = (UInt32)65; // A

                // Act
                var handleHotKeyResult = dummyHotKeyManager.HandleHotKey(hotKeyModifier, virtualKeyCode);

                // Assert
                handleHotKeyResult.Should().BeFalse();
            }
        }

        [TestMethod]
        public void HandleHotKeyWillReturnTrueAndCallActionIfHotKeyWasFound()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                using (var dummyHotKeyManager = new DummyHotKeyManager())
                {
                    // Arrange
                    var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                    var virtualKeyCode = (UInt32)65; // A
                    var hotKeyActionSet = false;
                    Action hotKeyAction = () =>
                    {
                        hotKeyActionSet = true;
                    };
                    dummyHotKeyManager.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction);

                    // Act
                    var handleHotKeyResult = dummyHotKeyManager.HandleHotKey(hotKeyModifier, virtualKeyCode);

                    // Assert
                    handleHotKeyResult.Should().BeTrue();
                    hotKeyActionSet.Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void DisposeWillDisposeTheHotKeyManager()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                var virtualKeyCode = (UInt32)65; // A
                var hotKeyActionSet = false;
                Action hotKeyAction = () =>
                {
                    hotKeyActionSet = true;
                };

                // Act
                DummyHotKeyManager dummyHotKeyManager;
                using (dummyHotKeyManager = new DummyHotKeyManager())
                {  
                    dummyHotKeyManager.Register(dummyWindow, hotKeyModifier, virtualKeyCode, hotKeyAction);
                }

                // Assert
                dummyHotKeyManager.HasRegisteredHotKeys.Should().BeFalse();
            }
        }

        [TestMethod]
        public void DisposeWillNotFailIfHotKeyManagerWasAlreadyDisposed()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                var virtualKeyCode = (UInt32)65; // A
                var hotKeyActionSet = false;
                Action hotKeyAction = () =>
                {
                    hotKeyActionSet = true;
                };

                DummyHotKeyManager dummyHotKeyManager;
                using (dummyHotKeyManager = new DummyHotKeyManager())
                {
                }

                // Act, Assert
                dummyHotKeyManager.Invoking(hkm => hkm.Dispose()).ShouldNotThrow<ObjectDisposedException>();
            }
        }
    }
}
