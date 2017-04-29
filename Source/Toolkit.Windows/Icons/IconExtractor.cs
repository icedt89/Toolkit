namespace JanHafner.Toolkit.Windows.Icons
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>
    /// Extracts icon resources from streams.
    /// </summary>
    public sealed class IconExtractor : IDisposable
    {
        [NotNull]
        private IconFileHeader header;

        [NotNull]
        private readonly ICollection<IconDirectoryEntry> directoryEntries;

        private readonly Boolean disposeStream;

        private Boolean isDisposed;

        [NotNull]
        private readonly Stream stream;

        private IconExtractor()
        {
            this.directoryEntries = new List<IconDirectoryEntry>();
        }

        /// <summary>
        /// Initialized a new instance of the <see cref="IconExtractor"/> class with the specified *.ico file.
        /// </summary>
        /// <param name="iconFile">The *.ico file to open.</param>
        private IconExtractor([NotNull] String iconFile)
            : this()
        {
            if (iconFile == null)
            {
                throw new ArgumentNullException(nameof(iconFile));
            }

            this.disposeStream = true;
        
            this.stream = new FileStream(iconFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            this.Initialize();
        }

        /// <summary>
        /// Initialized a new instance of the <see cref="IconExtractor"/> class with the specified <see cref="Stream"/>.
        /// The supplied <see cref="Stream"/> will not be closed if this instance is disposed.
        /// </summary>
        /// <param name="beginOfHeader">The <see cref="Stream"/> where the first byte points to the start of the header.</param>
        private IconExtractor([NotNull] Stream beginOfHeader)
            : this()
        {
            if (beginOfHeader == null)
            {
                throw new ArgumentNullException(nameof(beginOfHeader));
            }

            this.stream = beginOfHeader;

            this.Initialize();
        }

        private void Initialize()
        {
            using (var binaryReader = new BinaryReader(this.stream, Encoding.UTF8, true))
            {
                this.header = new IconFileHeader(binaryReader);

                for (var i = 0; i < this.header.EntryCount; i++)
                {
                    var directoryEntry = new IconDirectoryEntry(binaryReader);

                    this.directoryEntries.Add(directoryEntry);
                }
            }
        }

        /// <summary>
        /// Indicates if there are icons present.
        /// </summary>
        public Boolean ContainsIcons
        {
            get { return this.header.EntryCount > 0; }
        }

        /// <summary>
        /// tries to create an <see cref="IconExtractor"/> from the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="iconExtractor">The created <see cref="IconExtractor"/>.</param>
        /// <returns><c>True</c> if successfully created.</returns>
        public static Boolean TryCreate(String file, out IconExtractor iconExtractor)
        {
            try
            {
                iconExtractor = new IconExtractor(file);

                return true;
            }
            catch
            {
                iconExtractor = null;

                return false;
            }
        }

        public static Boolean TryCreate(Stream stream, out IconExtractor iconExtractor)
        {
            try
            {
                iconExtractor = new IconExtractor(stream);

                return true;
            }
            catch
            {
                iconExtractor = null;

                return false;
            }
        }

        /// <summary>
        /// Asynchronously creates a list of all icons.
        /// </summary>
        /// <returns>The icons.</returns>
        public Task<IEnumerable<Icon>> EnumerateIconsAsync()
        {
            return this.EnumerateIconsAsync(CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously creates a list of all icons.
        /// </summary>
        /// <returns>The icons.</returns>
        public async Task<IEnumerable<Icon>> EnumerateIconsAsync(CancellationToken cancellationToken)
        {
            this.CheckDisposed();

            var result = new List<Icon>();
            foreach (var directoryEntry in this.directoryEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                this.stream.Seek(directoryEntry.OffsetOfData, SeekOrigin.Begin);

                var buffer = new Byte[directoryEntry.SizeOfData];
                await this.stream.ReadAsync(buffer, 0, directoryEntry.SizeOfData, cancellationToken);

                var iconHandle = NativeMethods.CreateIconFromResourceEx(buffer, (UInt32)buffer.Length, true, NativeMethods.Version, directoryEntry.Width, directoryEntry.Height, NativeExecutable.NativeMethods.LoadImageLoadResult.LR_DEFAULTCOLOR);
                if (iconHandle == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                var icon = iconHandle.ToIconWithOwnership();

                result.Add(icon);
            }

            return result;
        }

        /// <summary>
        /// Lazy enumerates all icons in the source.
        /// </summary>
        /// <returns>The icons.</returns>
        public IEnumerable<Icon> EnumerateIcons()
        {
            this.CheckDisposed();

            foreach (var directoryEntry in this.directoryEntries)
            {
                this.stream.Seek(directoryEntry.OffsetOfData, SeekOrigin.Begin);

                var buffer = new Byte[directoryEntry.SizeOfData];

                this.stream.Read(buffer, 0, directoryEntry.SizeOfData);

                var iconHandle = NativeMethods.CreateIconFromResourceEx(buffer, (UInt32)buffer.Length, true, NativeMethods.Version, directoryEntry.Width, directoryEntry.Height, NativeExecutable.NativeMethods.LoadImageLoadResult.LR_DEFAULTCOLOR);

                yield return iconHandle.ToIconWithOwnership();
            }
        }

        /// <summary>
        /// Simply checks if the supplied <see cref="String"/> ends with ".ico".
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>True</c> if <paramref name="file"/> ends with ".ico".</returns>
        public static Boolean CouldBeIconFile([NotNull] String file)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            return file.EndsWith(".ico", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Disposes this instance and the <see cref="Stream"/> held if this instance was created by using the <see cref="String"/> overload.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.disposeStream)
            {
                this.stream.Dispose();
            }

            this.directoryEntries.Clear();

            this.isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private static class NativeMethods
        {
            public const UInt32 Version = 0x00030000;

            [DllImport("User32", SetLastError = true)]
            public static extern IntPtr CreateIconFromResourceEx(Byte[] buffer, UInt32 size, Boolean bufferContainsIcons, UInt32 version, Int32 desiredWidth, Int32 desiredHeight, NativeExecutable.NativeMethods.LoadImageLoadResult flags);
        }
    }
}
