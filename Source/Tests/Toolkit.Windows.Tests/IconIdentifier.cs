namespace JanHafner.Toolkit.Windows.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class IconIdentifier
    {
        [TestMethod]
        public void IdentityWillReturnResourceIdIfLowerThanMinusOne()
        {
            // Arrange
            var iconIdentifier = new Random().Next(Int32.MinValue, - 2);

            // Act
            var iconIdentiferType = JanHafner.Toolkit.Windows.IconIdentifier.Identify(iconIdentifier);

            // Arrange
            iconIdentiferType.Should().Be(JanHafner.Toolkit.Windows.IconIdentifierType.ResourceId);
        }

        [TestMethod]
        public void IdentityWillReturnIndexIfGreaterThanMinusOne()
        {
            // Arrange
            var iconIdentifier = new Random().Next(-1, Int32.MaxValue);

            // Act
            var iconIdentiferType = JanHafner.Toolkit.Windows.IconIdentifier.Identify(iconIdentifier);

            // Arrange
            iconIdentiferType.Should().Be(JanHafner.Toolkit.Windows.IconIdentifierType.Index);
        }
    }
}
