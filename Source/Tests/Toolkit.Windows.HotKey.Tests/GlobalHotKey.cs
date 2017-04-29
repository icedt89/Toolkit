namespace JanHafner.Toolkit.Windows.HotKey.Tests
{
    using System;
    using System.ComponentModel;
    using FluentAssertions;
    using JanHafner.Toolkit.Windows.HotKey;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class GlobalHotKey
    {

        [TestMethod]
        public void RegisterWillFaillIfWindowHandleIsZero()
        {
            // Arrange
            var windowHandle = IntPtr.Zero;
            var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
            var virtualKeyCode = (UInt32)65; // A

            Action registerFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(windowHandle, hotKeyModifier, virtualKeyCode);
            };

            // Act, Assert
            registerFunctionActor.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void RegisterWillFaillIfHotKeyModifierIsNone()
        {
            // Arrange
            var windowHandle = new Random().Next(1, Int32.MaxValue);
            var hotKeyModifier = HotKeyModifier.None;
            var virtualKeyCode = (UInt32)65; // A

            Action registerFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(new IntPtr(windowHandle), hotKeyModifier, virtualKeyCode);
            };

            // Act, Assert
            registerFunctionActor.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void RegisterWillCreateGlobalHotKey()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
                var virtualKeyCode = (UInt32)65; // A

                // Act
                using (
                    var globalHotKey = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle,
                        hotKeyModifier, virtualKeyCode))
                {
                    globalHotKey.Should().NotBeNull();
                    globalHotKey.Id.Should().NotBe(0);
                    globalHotKey.HotKeyModifier.Should().Be(hotKeyModifier);
                    globalHotKey.VirtualKeyCode.Should().Be(virtualKeyCode);
                    globalHotKey.WindowHandle.Should().Be(dummyWindow.WindowHandle);
                }
            }
        }

        [TestMethod]
        public void RegisterWillFailIfWindowHandleIsInvalid()
        {
            // Arrange
            var windowHandle = IntPtr.Zero;
            var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Shift | HotKeyModifier.Control;
            var virtualKeyCode = (UInt32)65; // A

            using (var dummyWindow = DummyWindow.Create())
            {
                // Invalidate window handle
                windowHandle = dummyWindow.WindowHandle;
            }

            Action registerFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(windowHandle, hotKeyModifier, virtualKeyCode);
            };

            // Act
            registerFunctionActor.ShouldThrow<Win32Exception>();
        }

        [TestMethod]
        public void RegisterWillFailIfHotKeyIsAlreadyRegistered()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                const Int32 hotKeyAlreadyRegisteredErrorCode = 1409;
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
                var virtualKeyCode = (UInt32) 65; // A

                using (JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle, hotKeyModifier, virtualKeyCode))
                {
                    Action registerFunctionActor = () =>
                    {
                        JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle, hotKeyModifier, virtualKeyCode);
                    };

                    // Act
                    registerFunctionActor.ShouldThrow<Win32Exception>().Where(ex => ex.NativeErrorCode == hotKeyAlreadyRegisteredErrorCode);
                }
            }
        }

        [TestMethod]
        public void UnregisterWillFailIfWindowHandleIsInvalid()
        {
            // Arrange
            var windowHandle = IntPtr.Zero;
            var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
            var virtualKeyCode = (UInt32)65; // A
            JanHafner.Toolkit.Windows.HotKey.GlobalHotKey globalHotKey = null;

            using (var dummyWindow = DummyWindow.Create())
            {
                windowHandle = dummyWindow.WindowHandle;

                globalHotKey = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(windowHandle, hotKeyModifier, virtualKeyCode);
            }

            globalHotKey.Invoking(ghk => ghk.Unregister()).ShouldThrow<Win32Exception>();
        }

        [TestMethod]
        public void ProbeWillByTrueIfHotKeyIsNotRegistered()
        {
            // Arrange
            var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
            var virtualKeyCode = (UInt32)65; // A

            // Act
            var probeResult = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Probe(hotKeyModifier, virtualKeyCode);

            // Assert
            probeResult.Should().BeTrue();
        }

        [TestMethod]
        public void ProbeWillBeFalseIfHotKeyIsRegistered()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
                var virtualKeyCode = (UInt32)65; // A
                using (JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle, hotKeyModifier, virtualKeyCode))
                {
                    // Act
                    var probeResult = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Probe(hotKeyModifier, virtualKeyCode);

                    // Assert
                    probeResult.Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void UnregisterWillUnregisterTheHotKey()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
                var virtualKeyCode = (UInt32)65; // A
                var globalHotKey = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle, hotKeyModifier, virtualKeyCode);

                // Acti
                globalHotKey.Unregister();

                // Assert
                globalHotKey.HotKeyModifier.Should().Be(HotKeyModifier.None);
                globalHotKey.WindowHandle.Should().Be(IntPtr.Zero);
                globalHotKey.VirtualKeyCode.Should().Be(0);
            }
        }

        [TestMethod]
        public void UnregisterWillFailIfHotKeyAlreadyUnregistered()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
                var virtualKeyCode = (UInt32)65; // A
                var globalHotKey = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle, hotKeyModifier, virtualKeyCode);
                globalHotKey.Unregister();

                // Act, Assert
                globalHotKey.Invoking(ghk => ghk.Unregister()).ShouldThrow<ObjectDisposedException>();
            }
        }

        [TestMethod]
        public void DisposeWillDisposeTheHotKey()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
                var virtualKeyCode = (UInt32)65; // A
                JanHafner.Toolkit.Windows.HotKey.GlobalHotKey globalHotKey = null;

                // Act
                using (globalHotKey = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle, 
                        hotKeyModifier, virtualKeyCode))
                {
                }

                // Assert
                globalHotKey.HotKeyModifier.Should().Be(HotKeyModifier.None);
                globalHotKey.WindowHandle.Should().Be(IntPtr.Zero);
                globalHotKey.VirtualKeyCode.Should().Be(0);
            }
        }

        [TestMethod]
        public void DisposeWillNotFailIfHotKeyWasAlreadyDisposed()
        {
            using (var dummyWindow = DummyWindow.Create())
            {
                // Arrange
                var hotKeyModifier = HotKeyModifier.Alt | HotKeyModifier.Control | HotKeyModifier.Shift;
                var virtualKeyCode = (UInt32)65; // A
                JanHafner.Toolkit.Windows.HotKey.GlobalHotKey globalHotKey = null;

                using (globalHotKey = JanHafner.Toolkit.Windows.HotKey.GlobalHotKey.Register(dummyWindow.WindowHandle,
                        hotKeyModifier, virtualKeyCode))
                {
                }

                // Act, Assert
                globalHotKey.Invoking(ghk => ghk.Dispose()).ShouldNotThrow<ObjectDisposedException>();
            }
        }
    }
}
