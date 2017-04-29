namespace JanHafner.Toolkit.Windows.Atoms
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>
    /// Representation of a local atom.
    /// </summary>
    public sealed class LocalAtom : Atom
    {
        public LocalAtom(UInt16 id, [NotNull] String name) 
            : base(id, name)
        {
        }

        /// <summary>
        /// Creates a new string <see cref="LocalAtom"/> based on the supplied <paramref name="name"/>.
        /// If the atom already exists, a <see cref="Win32Exception"/> is thrown.
        /// </summary>
        /// <param name="name">The name of the atom.</param>
        /// <exception cref="Win32Exception">If the function fails a <see cref="Win32Exception"/>, containing the reason, is thrown.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>, contains only witespaces or is empty.</exception>
        /// <returns>A <see cref="LocalAtom"/> representing the atom.</returns>
        [NotNull]
        public static Atom CreateNew(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var existingLocalAtom = LocalAtomNativeMethods.FindAtom(name);
            if (existingLocalAtom == 0)
            {
                existingLocalAtom = LocalAtomNativeMethods.AddAtom(name);
                if (existingLocalAtom == 0)
                {
                    throw new Win32Exception();
                }

                return new LocalAtom(existingLocalAtom, name);
            }

            throw new Win32Exception();
        }

        /// <summary>
        /// Creates an existing <see cref="LocalAtom"/> based on the supplied <paramref name="name"/>.
        /// Reference count on the atom is not incremented, so that an atom which was created by this method could dispose an atom created previously.
        /// </summary>
        /// <param name="name">The name of the atom.</param>
        /// <exception cref="Win32Exception">If the function fails a <see cref="Win32Exception"/>, containing the reason, is thrown.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>, contains only witespaces or is empty.</exception>
        /// <returns>A <see cref="LocalAtom"/> representing the atom.</returns>
        [NotNull]
        public static Atom FromExisting([NotNull] String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var atomId = LocalAtomNativeMethods.FindAtom(name);
            if (atomId == 0)
            {
                throw new Win32Exception();
            }

            return new LocalAtom(atomId, name);
        }

        /// <summary>
        /// Retrieves the name of the atom supplied by <paramref name="atomId"/>.
        /// </summary>
        /// <param name="atomId">The id of the atom from which to retrieve the name.</param>
        /// <returns>The name of the requested atom.</returns>
        [NotNull]
        public static String GetAtomName(UInt16 atomId)
        {
            if (atomId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(atomId));
            }

            var buffer = new StringBuilder(1024);
            var result = LocalAtomNativeMethods.GetAtomName(atomId, buffer, buffer.Capacity);
            if (result == 0)
            {
                throw new Win32Exception();
            }

            return buffer.ToString();
        }

        protected override void DeleteAtom(UInt16 atomid)
        {
            LocalAtomNativeMethods.DeleteAtom(atomid);
        }

        private static class LocalAtomNativeMethods
        {
            /// <summary>
            /// Adds a character string to the local atom table and returns a unique value (an atom) identifying the string.
            /// </summary>
            /// <param name="name">The null-terminated string to be added. The string can have a maximum size of 255 bytes. Strings that differ only in case are considered identical</param>
            /// <returns>If the function succeeds, the return value is the newly created atom. If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern UInt16 AddAtom(String name);

            /// <summary>
            /// Decrements the reference count of a local string atom. If the atom's reference count reaches zero, DeleteAtom removes the string associated with the atom from the local atom table.
            /// </summary>
            /// <param name="atomId">The atom and character string to be deleted.</param>
            /// <returns>The function always returns (ATOM) 0. To determine whether the function has failed, call SetLastError with ERROR_SUCCESS before calling DeleteAtom, then call GetLastError.If the last error code is still ERROR_SUCCESS, DeleteAtom has succeeded.</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern UInt16 DeleteAtom(UInt16 atomId);

            /// <summary>
            /// Searches the local atom table for the specified character string and retrieves the local atom associated with that string.
            /// </summary>
            /// <param name="name">The null-terminated character string for which to search. Alternatively, you can use an integer atom that has been converted using the MAKEINTATOM macro.See the Remarks for more information.</param>
            /// <returns>If the function succeeds, the return value is the local atom associated with the given string. If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern UInt16 FindAtom(String name);

            /// <summary>
            /// Retrieves a copy of the character string associated with the specified local atom.
            /// </summary>
            /// <param name="atomId">The local atom associated with the character string to be retrieved.</param>
            /// <param name="buffer">The buffer for the character string.</param>
            /// <param name="size">The size, in characters, of the buffer.</param>
            /// <returns>If the function succeeds, the return value is the length of the string copied to the buffer, in characters, not including the terminating null character.</returns>
            [DllImport("kernel32.dll", ThrowOnUnmappableChar = true, SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern UInt32 GetAtomName(UInt16 atomId, StringBuilder buffer, Int32 size);
        }
    }
}
