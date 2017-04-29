namespace JanHafner.Toolkit.Windows.Tests.Icons
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using JanHafner.Toolkit.Windows.Tests.Properties;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class IconExtractor
    {
        [TestMethod]
        public void TryCreateWillCreateTheIconExtractorFromAStream()
        {
            // Arrange
            using (var iconStream = Resources.IconWith6Sizes.ToStream())
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
                var tryCreateResult = JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(iconStream, out iconExtractor);

                iconExtractor.Dispose();

                // Act, Assert
                tryCreateResult.Should().BeTrue();
                iconExtractor.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void TryCreateWillFailIfTheStreamIsNull()
        {
            // Arrange
            var stream = (Stream)null;
            JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
            var tryCreateResult = JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(stream, out iconExtractor);

            // Act, Assert
            tryCreateResult.Should().BeFalse();
            iconExtractor.Should().BeNull();
        }

        [TestMethod]
        public void TryCreateWillFailIfTheFileIsNull()
        {
            // Arrange
            var file = (String)null;
            JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
            var tryCreateResult = JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(file, out iconExtractor);

            // Act, Assert
            tryCreateResult.Should().BeFalse();
            iconExtractor.Should().BeNull();
        }

        [TestMethod]
        public void CouldBeIconFileWillReturnFalseIfNotEndingWithIco()
        {
            // Arrange
            var file = $"{Guid.NewGuid()}.{Guid.NewGuid()}";

            // Act
            var couldBeIconFileResult = JanHafner.Toolkit.Windows.Icons.IconExtractor.CouldBeIconFile(file);

            // Assert
            couldBeIconFileResult.Should().BeFalse();
        }

        [TestMethod]
        public void CouldBeIconFileWillReturnTrueIfEndingWithIco()
        {
            // Arrange
            var file = $"{Guid.NewGuid()}.ico";

            // Act
            var couldBeIconFileResult = JanHafner.Toolkit.Windows.Icons.IconExtractor.CouldBeIconFile(file);

            // Assert
            couldBeIconFileResult.Should().BeTrue();
        }
        [TestMethod]
        public void CouldBeIconFileWillFailIfFileIsNull()
        {
            // Arrange
            var file = (String) null;
            Action couldBeIconFileFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor.CouldBeIconFile(file);
            };

            // Act, Arrange
            couldBeIconFileFunctionActor.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public async Task EnumerateIconsAsyncWillEnumerate6Icons()
        {
            // Arrange
            using (var iconStream = Resources.IconWith6Sizes.ToStream())
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
                JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(iconStream, out iconExtractor);

                using (iconExtractor)
                {
                    // Act
                    var enumeratedIcons = (await iconExtractor.EnumerateIconsAsync()).ToList();

                    // Assert
                    enumeratedIcons.Count.Should().Be(6);

                    enumeratedIcons.ForEach(icon => icon.Dispose());
                }
            }
        }

        [TestMethod]
        public void EnumerateIconsWillEnumerate6Icons()
        {
            // Arrange
            using (var iconStream = Resources.IconWith6Sizes.ToStream())
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
                JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(iconStream, out iconExtractor);

                using (iconExtractor)
                {
                    // Act
                    var enumeratedIcons = iconExtractor.EnumerateIcons().ToList();

                    // Assert
                    enumeratedIcons.Count.Should().Be(6);

                    enumeratedIcons.ForEach(icon => icon.Dispose());
                }
            }
        }

        [TestMethod]
        public void ContainsIconsWillReturnTrue()
        {
            // Arrange
            using (var iconStream = Resources.IconWith6Sizes.ToStream())
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;

                // Act
                JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(iconStream, out iconExtractor);

                using (iconExtractor)
                {
                    // Assert
                    iconExtractor.ContainsIcons.Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void EnumerateIconsWillFailIfIconExtractorIsDisposed()
        {
            // Arrange
            using (var iconStream = Resources.IconWith6Sizes.ToStream())
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
                JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(iconStream, out iconExtractor);

                Action enumerateIconsFunctionActor = () =>
                {
                    iconExtractor.EnumerateIcons().ToList();
                };

                iconExtractor.Dispose();

                // Act, Arrange
                enumerateIconsFunctionActor.ShouldThrow<ObjectDisposedException>();
            }
        }

        [TestMethod]
        public async Task EnumerateIconsAsyncWillFailIfIconExtractorIsDisposed()
        {
            // Arrange
            using (var iconStream = Resources.IconWith6Sizes.ToStream())
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
                JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(iconStream, out iconExtractor);

                iconExtractor.Dispose();

                var result = false;
                try
                {
                    // Act
                    await iconExtractor.EnumerateIconsAsync();

                    result = false;
                }
                catch (ObjectDisposedException)
                {
                    result = true;
                }

                result.Should().BeTrue();
            }
        }

        [TestMethod]
        public void DisposeWillNotFailIfDisposed()
        {
            // Arrange
            using (var iconStream = Resources.IconWith6Sizes.ToStream())
            {
                JanHafner.Toolkit.Windows.Icons.IconExtractor iconExtractor;
                JanHafner.Toolkit.Windows.Icons.IconExtractor.TryCreate(iconStream, out iconExtractor);

                Action disposeFunctionActor = () =>
                {
                    iconExtractor.Dispose();
                };

                iconExtractor.Dispose();

                // Act, Arrange
                disposeFunctionActor.ShouldNotThrow<ObjectDisposedException>();
            }
        }
    }
}
