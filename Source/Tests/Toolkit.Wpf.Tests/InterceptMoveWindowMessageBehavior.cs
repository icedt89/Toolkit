namespace JanHafner.Toolkit.Wpf.Tests
{
    using System;
    using System.Windows;
    using FluentAssertions;
    using JanHafner.Toolkit.Wpf.Behavior;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class InterceptMoveWindowMessageBehavior
    {
        [TestMethod]
        public void InterceptWndProcWillCallManipulatePositionIfMessageIsWmMoveAndEnabledIsTrue()
        {
            // Arrange
            var interceptMoveWindowMessageBehavior = new DummyInterceptMoveWindowMessageBehavior();
            var manipulatePositionResult = false;
            var windowMessage = (Int32) WindowMessage.WM_MOVE;
            interceptMoveWindowMessageBehavior.Enabled = true;

            using (var rectStructure = DisposableStructure<Rect>.Create())
            {
                // Act
                interceptMoveWindowMessageBehavior.DummyInterceptWndProc(IntPtr.Zero, windowMessage, IntPtr.Zero, rectStructure.StructurePointer, ref manipulatePositionResult);

                // Assert
                manipulatePositionResult.Should().BeTrue();
            }
        }

        [TestMethod]
        public void InterceptWndProcWillNotCallManipulatePositionIfMessageIsNotWmMoveAndEnabledIsTrue()
        {
            // Arrange
            var interceptMoveWindowMessageBehavior = new DummyInterceptMoveWindowMessageBehavior();
            var manipulatePositionResult = false;
            var windowMessage = new Random().Next(535, Int32.MaxValue);
            interceptMoveWindowMessageBehavior.Enabled = true;

            // Act
            interceptMoveWindowMessageBehavior.DummyInterceptWndProc(IntPtr.Zero, windowMessage, IntPtr.Zero, IntPtr.Zero, ref manipulatePositionResult);

            // Assert
            manipulatePositionResult.Should().BeFalse();
        }

        [TestMethod]
        public void InterceptWndProcWillNotCallManipulatePositionIfMessageIsWmMoveAndEnabledIsFalse()
        {
            // Arrange
            var interceptMoveWindowMessageBehavior = new DummyInterceptMoveWindowMessageBehavior();
            var manipulatePositionResult = false;
            var windowMessage = (Int32)WindowMessage.WM_MOVE;

            // Act
            interceptMoveWindowMessageBehavior.DummyInterceptWndProc(IntPtr.Zero, windowMessage, IntPtr.Zero, IntPtr.Zero, ref manipulatePositionResult);

            // Assert
            manipulatePositionResult.Should().BeFalse();
        }

        [TestMethod]
        public void InterceptWndProcWillReturnIntPtrZeroIfWmMoveAndEnabledIsTrue()
        {
            // Arrange
            var interceptMoveWindowMessageBehavior = new DummyInterceptMoveWindowMessageBehavior();
            var manipulatePositionResult = false;
            var windowMessage = (Int32)WindowMessage.WM_MOVE;
            interceptMoveWindowMessageBehavior.Enabled = true;
            var interceptWndProcResult = new IntPtr(new Random().Next(1, Int32.MaxValue));

            using (var rectStructure = DisposableStructure<Rect>.Create())
            {
                // Act
                interceptWndProcResult = interceptMoveWindowMessageBehavior.DummyInterceptWndProc(IntPtr.Zero, windowMessage, IntPtr.Zero, rectStructure.StructurePointer, ref manipulatePositionResult);

                // Assert
                interceptWndProcResult.Should().Be(IntPtr.Zero);
            }
        }

        [TestMethod]
        public void InterceptWndProcWillReturnIntPtrZeroIfNotWmMoveAndEnabledIsTrue()
        {
            // Arrange
            var interceptMoveWindowMessageBehavior = new DummyInterceptMoveWindowMessageBehavior();
            var manipulatePositionResult = false;
            var windowMessage = new Random().Next(535, Int32.MaxValue);
            interceptMoveWindowMessageBehavior.Enabled = true;
            var interceptWndProcResult = new IntPtr(new Random().Next(1, Int32.MaxValue));

            // Act
            interceptWndProcResult = interceptMoveWindowMessageBehavior.DummyInterceptWndProc(IntPtr.Zero, windowMessage, IntPtr.Zero, IntPtr.Zero, ref manipulatePositionResult);

            // Assert
            interceptWndProcResult.Should().Be(IntPtr.Zero);
        }

        [TestMethod]
        public void InterceptWndProcWillReturnIntPtrZeroIfWmMoveAndEnabledIsFalse()
        {
            // Arrange
            var interceptMoveWindowMessageBehavior = new DummyInterceptMoveWindowMessageBehavior();
            var manipulatePositionResult = false;
            var windowMessage = (Int32)WindowMessage.WM_MOVE;
            var interceptWndProcResult = new IntPtr(new Random().Next(1, Int32.MaxValue));

            // Act
            interceptWndProcResult = interceptMoveWindowMessageBehavior.DummyInterceptWndProc(IntPtr.Zero, windowMessage, IntPtr.Zero, IntPtr.Zero, ref manipulatePositionResult);

            // Assert
            interceptWndProcResult.Should().Be(IntPtr.Zero);
        }
    }
}
