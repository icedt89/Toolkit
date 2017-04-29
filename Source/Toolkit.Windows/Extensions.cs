namespace JanHafner.Toolkit.Windows
{
    using System;
    using System.Collections;
    using System.Drawing;
    using JetBrains.Annotations;

    public static class Extensions 
    {
        public static UInt32 ToUInt32([NotNull] this BitArray bitArray)
        {
            if (bitArray == null)
            {
                throw new ArgumentNullException(nameof(bitArray));
            }

            if (bitArray.Length > 64)
            {
                throw new ArgumentException("Argument length shall be at most 64 bits.");
            }

            var array = new Byte[8];
            bitArray.CopyTo(array, 0);

            return BitConverter.ToUInt32(array, 0);
        }

        /// <summary>
        /// Constructs an <see cref="System.Drawing.Icon"/> from the supplied handle... but:
        /// Because the constructor of <see cref="System.Drawing.Icon"/> which accepts an <see cref="IntPtr"/> does not take ownership of the handle,
        /// we make sure we have the ownership and destroy to old handle to prevent memory leaks.
        /// </summary>
        /// <param name="iconHandle">The icon handle.</param>
        /// <returns>An <see cref="System.Drawing.Icon"/> with ownership.</returns>
        [NotNull]
        public static Icon ToIconWithOwnership(this IntPtr iconHandle)
        {
            // According to the source of the Icon-class, this constructor does not take ownership of the handle.
            // To prevent a memory leak in this case, we clone the created icon, and destroy the handle returned from ExtractIcon(...).
            // The clone now has ownership of the newly created handle during the Clone() and gets correctly freed on Dispose().
            var icon = (Icon)Icon.FromHandle(iconHandle).Clone();

            SafeNativeMethods.DestroyIcon(iconHandle);

            return icon;
        }
    }
}
