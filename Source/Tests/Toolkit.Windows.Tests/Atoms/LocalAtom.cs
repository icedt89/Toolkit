namespace JanHafner.Toolkit.Windows.Tests.Atoms
{
    using System;
    using System.ComponentModel;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LocalAtom
    {
        [TestMethod]
        public void CreateNewWillFaillIfAtomNameIsEmpty()
        {
            // Arrange
            var localAtomName = String.Empty;
            Action createNewFunctionActor = () =>
            {
                using (JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
                {
                }
            };

            // Act, Assert
            createNewFunctionActor.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void FromExistingWillFailIfAtomNameIsEmpty()
        {
            // Arrange
            var localAtomName = String.Empty;
            Action fromExistingFunctionActor = () =>
            {
                using (JanHafner.Toolkit.Windows.Atoms.LocalAtom.FromExisting(localAtomName))
                {
                }
            };

            // Act, Assert
            fromExistingFunctionActor.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateNewWillReturnNewLocalAtom()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();

            // Act
            using (var localAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
            {
                // Assert
                localAtom.Should().BeOfType<JanHafner.Toolkit.Windows.Atoms.LocalAtom>();
                localAtom.Should().NotBeNull();
                localAtom.Id.Should().NotBe(0);
                localAtom.Name.Should().Be(localAtomName);
            }
        }

        [TestMethod]
        public void GetAtomNameWillReturnTheNameOfAnExistingLocalAtom()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();

            using (var localAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
            {
                // Act
                var existingLocalAtomName = JanHafner.Toolkit.Windows.Atoms.LocalAtom.GetAtomName(localAtom.Id);

                // Assert
                existingLocalAtomName.Should().NotBeNullOrEmpty();
                existingLocalAtomName.Should().Be(localAtom.Name);
            }
        }

        [TestMethod]
        public void GetAtomNameWillFailIfAtomIdIsZero()
        {
            // Arrange
            var localAtomId = (UInt16)0;
            Action getAtomNameFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.Atoms.LocalAtom.GetAtomName(localAtomId);
            };

            // Act, Assert
            getAtomNameFunctionActor.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void GetAtomNameWillFailIfTheAtomDoesNotExists()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();
            var localAtomId = (UInt16)0;

            // Make sure we get an unique atom id, delete the created atom.
            using (var localAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
            {
                localAtomId = localAtom.Id;
            }

            Action getAtomNameFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.Atoms.LocalAtom.GetAtomName(localAtomId);
            };

            // Act, Assert
            getAtomNameFunctionActor.Should().Throw<Win32Exception>();
        }

        [TestMethod]
        public void CreateNewWillFaillIfTheAtomAlreadyExists()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();
            Action createNewFunctionActor = () =>
            {
                using (JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
                {
                }
            };

            // Act
            using (JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
            {
                // Assert
                createNewFunctionActor.Should().Throw<Win32Exception>();
            }
        }

        [TestMethod]
        public void FromExistingWillCreateLocalAtomIfOneExists()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();

            // Act
            using (var localAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
            {
                Action fromExistingFunctionActor = () =>
                {
                    // using not required because atom reference count is not incremented.
                    var existingLocalAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.FromExisting(localAtomName);

                    // Assert
                    existingLocalAtom.Should().NotBeNull();
                    existingLocalAtom.Id.Should().Be(localAtom.Id);
                    existingLocalAtom.Name.Should().Be(localAtom.Name);
                };

                // Assert
                fromExistingFunctionActor.Should().NotThrow<Win32Exception>();
            }
        }

        [TestMethod]
        public void LocalAtomEqualsTwoAtomsAsEqualEvenIfInstancesAreNotTheSame()
        {
            // Arrange
            var localAtomOneName = Guid.NewGuid().ToString();
            var localAtomTwoName = Guid.NewGuid().ToString();

            // Act
            using (var localAtomOne = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomOneName))
            {
                using (var existingLocalAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomTwoName))
                {
                    // Assert
                    existingLocalAtom.Equals(localAtomOne).Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void DeleteWillDeleteTheAtom()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();
            var localAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName);

            // Act
            localAtom.Invoking(ga => ga.Delete()).Should().NotThrow();

            // Assert
            localAtom.Id.Should().Be(0);
            localAtom.Name.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void DeleteWillFailIfAtomWasAlreadyDeleted()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();

            var localAtom = JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName);
            localAtom.Delete();

            // Act, Assert
            localAtom.Invoking(ga => ga.Delete()).Should().Throw<ObjectDisposedException>();
        }

        [TestMethod]
        public void DisposeWillDisposeTheAtom()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();
            JanHafner.Toolkit.Windows.Atoms.LocalAtom localAtom = null;

            // Act
            using (localAtom = (JanHafner.Toolkit.Windows.Atoms.LocalAtom)JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
            {
            }

            // Assert
            localAtom.Id.Should().Be(0);
            localAtom.Name.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void DisposeWillNotFailIfAtomWasAlreadyDisposed()
        {
            // Arrange
            var localAtomName = Guid.NewGuid().ToString();

            JanHafner.Toolkit.Windows.Atoms.LocalAtom localAtom = null;
            using (localAtom = (JanHafner.Toolkit.Windows.Atoms.LocalAtom)JanHafner.Toolkit.Windows.Atoms.LocalAtom.CreateNew(localAtomName))
            {
            }

            // Act, Assert
            localAtom.Invoking(ga => ga.Dispose()).Should().NotThrow<ObjectDisposedException>();
        }
    }
}