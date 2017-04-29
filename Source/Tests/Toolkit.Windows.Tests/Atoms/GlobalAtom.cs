namespace JanHafner.Toolkit.Windows.Tests.Atoms
{
    using System;
    using System.ComponentModel;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class GlobalAtom
    {
        [TestMethod]
        public void CreateNewWillFaillIfAtomNameIsEmpty()
        {
            // Arrange
            var globalAtomName = String.Empty;
            Action createNewFunctionActor = () =>
            {
                using (JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
                {
                }
            };

            // Act, Assert
            createNewFunctionActor.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void FromExistingWillFailIfAtomNameIsEmpty()
        {
            // Arrange
            var globalAtomName = String.Empty;
            Action fromExistingFunctionActor = () =>
            {
                using (JanHafner.Toolkit.Windows.Atoms.GlobalAtom.FromExisting(globalAtomName))
                {
                }
            };

            // Act, Assert
            fromExistingFunctionActor.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateNewWillReturnNewGlobalAtom()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();

            // Act
            using (var globalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
            {
                // Assert
                globalAtom.Should().BeOfType<JanHafner.Toolkit.Windows.Atoms.GlobalAtom>();
                globalAtom.Should().NotBeNull();
                globalAtom.Id.Should().NotBe(0);
                globalAtom.Name.Should().Be(globalAtomName);
            }
        }

        [TestMethod]
        public void GetAtomNameWillReturnTheNameOfAnExistingGlobalAtom()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();

            using (var globalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
            {
                // Act
                var existingGlobalAtomName = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.GetAtomName(globalAtom.Id);

                // Assert
                existingGlobalAtomName.Should().NotBeNullOrEmpty();
                existingGlobalAtomName.Should().Be(globalAtom.Name);
            }
        }

        [TestMethod]
        public void GetAtomNameWillFailIfAtomIdIsZero()
        {
            // Arrange
            var globalAtomId = (UInt16)0;
            Action getAtomNameFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.Atoms.GlobalAtom.GetAtomName(globalAtomId);
            };

            // Act, Assert
            getAtomNameFunctionActor.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void GetAtomNameWillFailIfTheAtomDoesNotExists()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();
            var globalAtomId = (UInt16)0;

            // Make sure we get an unique atom id, delete the created atom.
            using (var globalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
            {
                globalAtomId = globalAtom.Id;
            }

            Action getAtomNameFunctionActor = () =>
            {
                JanHafner.Toolkit.Windows.Atoms.GlobalAtom.GetAtomName(globalAtomId);
            };

            // Act, Assert
            getAtomNameFunctionActor.ShouldThrow<Win32Exception>();
        }

        [TestMethod]
        public void CreateNewWillFaillIfTheAtomAlreadyExists()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();
            Action createNewFunctionActor = () =>
            {
                using (JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
                {
                }
            };

            // Act
            using (JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
            {
                // Assert
                createNewFunctionActor.ShouldThrow<Win32Exception>();
            }
        }

        [TestMethod]
        public void FromExistingWillCreateGlobalAtomIfOneExists()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();

            // Act
            using (var globalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
            {
                Action fromExistingFunctionActor = () =>
                {
                    // using not required because atom reference count is not incremented.
                    var existingGlobalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.FromExisting(globalAtomName);

                    // Assert
                    existingGlobalAtom.Should().NotBeNull();
                    existingGlobalAtom.Id.Should().Be(globalAtom.Id);
                    existingGlobalAtom.Name.Should().Be(globalAtom.Name);
                };

                // Assert
                fromExistingFunctionActor.ShouldNotThrow<Win32Exception>();
            }
        }

        [TestMethod]
        public void GlobalAtomEqualsTwoAtomsAsEqualEvenIfInstancesAreNotTheSame()
        {
            // Arrange
            var globalAtomOneName = Guid.NewGuid().ToString();
            var globalAtomTwoName = Guid.NewGuid().ToString();

            // Act
            using (var globalAtomOne = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomOneName))
            {
                using (var existingGlobalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomTwoName))
                {
                    // Assert
                    existingGlobalAtom.Equals(globalAtomOne).Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void DeleteWillDeleteTheAtom()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();
            var globalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName);

            // Act
            globalAtom.Invoking(ga => ga.Delete()).ShouldNotThrow();

            // Assert
            globalAtom.Id.Should().Be(0);
            globalAtom.Name.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void DeleteWillFailIfAtomWasAlreadyDeleted()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();

            var globalAtom = JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName);
            globalAtom.Delete();

            // Act, Assert
            globalAtom.Invoking(ga => ga.Delete()).ShouldThrow<ObjectDisposedException>();
        }

        [TestMethod]
        public void DisposeWillDisposeTheAtom()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();
            JanHafner.Toolkit.Windows.Atoms.GlobalAtom globalAtom = null;

            // Act
            using (globalAtom = (JanHafner.Toolkit.Windows.Atoms.GlobalAtom)JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
            {
            }

            // Assert
            globalAtom.Id.Should().Be(0);
            globalAtom.Name.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void DisposeWillNotFailIfAtomWasAlreadyDisposed()
        {
            // Arrange
            var globalAtomName = Guid.NewGuid().ToString();

            JanHafner.Toolkit.Windows.Atoms.GlobalAtom globalAtom = null;
            using (globalAtom = (JanHafner.Toolkit.Windows.Atoms.GlobalAtom)JanHafner.Toolkit.Windows.Atoms.GlobalAtom.CreateNew(globalAtomName))
            {
            }

            // Act, Assert
            globalAtom.Invoking(ga => ga.Dispose()).ShouldNotThrow<ObjectDisposedException>();
        }
    }
}