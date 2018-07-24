namespace JanHafner.Toolkit.Windows.Tests
{
    using System;
    using System.Collections;
    using System.Drawing;
    using FluentAssertions;
    using JanHafner.Toolkit.Windows.Tests.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class Extensions
    {
        [TestMethod]
        public void ToUInt32WillFailIfBitarrayIsNull()
        {
            // Arrange
            var bitArray = (BitArray) null;
            Action toUInt32FunctionActor = () =>
            {
                bitArray.ToUInt32();
            };

            // Act, Assert
            toUInt32FunctionActor.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ToUInt32WillFailIfBitarrayLengthIsGreaterThan64()
        {
            // Arrange
            var bitArrayLength = new Random().Next(65, Int32.MaxValue);
            var bitArray = new BitArray(bitArrayLength);
            Action toUInt32FunctionActor = () =>
            {
                bitArray.ToUInt32();
            };

            // Act, Assert
            toUInt32FunctionActor.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void ToIconWilOwnershipWillReturnNewIconAndInvalidateTheHandle()
        {
            // Arrange
            var iconHandle = Resources.IconWith6Sizes.Handle;
            Action fromHandleFunctionActor = () =>
            {
                var icon = Icon.FromHandle(iconHandle);

                // Will fail because the handle is already invalid.
                icon.ToBitmap();
            };

            // Act
            var clonedIcon = iconHandle.ToIconWithOwnership();

            // Assert
            clonedIcon.Should().NotBeNull();
            clonedIcon.Handle.Should().NotBe(iconHandle);
            fromHandleFunctionActor.Should().Throw<Exception>();
        }

        [TestMethod]
        public void ToIconWilOwnershipWillFailIfIntPtr()
        {
            // Arrange
            var iconHandle = IntPtr.Zero;
            Action fromHandleFunctionActor = () =>
            {
                iconHandle.ToIconWithOwnership();
            };

            // Act, Assert
            fromHandleFunctionActor.Should().Throw<Exception>();
        }
    }
}
