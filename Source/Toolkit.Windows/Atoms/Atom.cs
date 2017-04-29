namespace JanHafner.Toolkit.Windows.Atoms
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;

    public abstract class Atom : IDisposable, IEquatable<Atom>
    {
        private Boolean isDisposed;

        protected Atom(UInt16 id, [NotNull] String name)
        {
            if (id == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Id = id;
            this.Name = name;
        }

        public UInt16 Id { get; private set; }

        [NotNull]
        public String Name { get; private set; }

        protected abstract void DeleteAtom(UInt16 atomid);

        public void Delete()
        {
            this.CheckDisposed();

            this.DeleteCore();
        }

        private void DeleteCore()
        {
            NativeMethods.SetLastError(0);

            this.DeleteAtom(this.Id);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != 0)
            {
                throw new Win32Exception(lastWin32Error);
            }

            this.Id = 0;
            this.Name = null;

            this.isDisposed = true;
        }

        ~Atom()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private void Dispose(Boolean disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.DeleteCore();
        }

        public override Int32 GetHashCode()
        {
            return this.Name.GetHashCode() + this.Id.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="GlobalAtom"/> insances by comparing their <see cref="Id"/> and <see cref="Name"/> property.
        /// </summary>
        /// <param name="other">The <see cref="GlobalAtom"/> for the compare.</param>
        /// <returns>A value indicating if the two <see cref="GlobalAtom"/> instances are considered equal.</returns>
        public override Boolean Equals(Object other)
        {
            var globalAtom = other as GlobalAtom;

            return this.Equals(globalAtom);
        }

        public Boolean Equals(Atom other)
        {
            if (other == null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // Make sure LocalAtom is ever unequal to GlobalAtom, even if their Id and Name would match!
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return other.Id == this.Id && this.Name == other.Name;
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern void SetLastError(UInt32 errorCode);
        }
    }
}
