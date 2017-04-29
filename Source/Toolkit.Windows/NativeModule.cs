namespace JanHafner.Toolkit.Windows
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;

    public abstract class NativeModule : IDisposable
    {
        [NotNull]
        private readonly Version WindowsVistaVersionNumber = new Version("6.0.6000");

        [NotNull]
        protected readonly String file;

        protected Boolean isDisposed;

        protected NativeModule([NotNull] String file)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            // Special case: I have seen several input files contain environment variables, we need to expand these to get the full path.
            file = Environment.ExpandEnvironmentVariables(file);

            // Special case: I have seen several input files starting with an "@"-sign.
            if (file.StartsWith("@", StringComparison.OrdinalIgnoreCase))
            {
                file = file.Remove(0, 1);
            }

            if (!File.Exists(file))
            {
                throw new FileNotFoundException(null, file);
            }

            // According to MSDN: LoadLibraryExFlags.LOAD_LIBRARY_AS_IMAGE_RESOURCE is not present until Windows Vista.
            // We dont want to provoke an error, so we check for this.
            var loadLibraryExFlags = NativeMethods.LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE;
            if (Environment.OSVersion.Version >= this.WindowsVistaVersionNumber)
            {
                loadLibraryExFlags |= NativeMethods.LoadLibraryExFlags.LOAD_LIBRARY_AS_IMAGE_RESOURCE;
            }

            // Load the referenced file as a data file into the address space of the executing application.
            var moduleHandle = NativeMethods.LoadLibraryEx(file, IntPtr.Zero, loadLibraryExFlags);
            if (moduleHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            this.Handle = moduleHandle;
            this.file = file;
        }

        protected IntPtr Handle { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Free()
        {
            this.CheckDisposed();

            this.FreeCore();
        }

        private void FreeCore()
        {
            var moduleFreed = NativeMethods.FreeLibrary(this.Handle);
            if (!moduleFreed)
            {
                throw new Win32Exception();
            }
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.FreeCore();

            this.isDisposed = true;
        }

        protected void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        ~NativeModule()
        {
            this.Dispose(false);
        }

        private static class NativeMethods
        {
            [Flags]
            public enum LoadLibraryExFlags : uint
            {
                /// <summary>
                /// If this value is used, the system maps the file into the calling process's virtual address space as if it were a data file. 
                /// Nothing is done to execute or prepare to execute the mapped file. 
                /// Therefore, you cannot call functions like GetModuleFileName, GetModuleHandle or GetProcAddress with this DLL. 
                /// Using this value causes writes to read-only memory to raise an access violation. 
                /// Use this flag when you want to load a DLL only to extract messages or resources from it.
                /// </summary>
                LOAD_LIBRARY_AS_DATAFILE = 0x00000002,

                /// <summary>
                /// If this value is used, the system maps the file into the process's virtual address space as an image file. 
                /// However, the loader does not load the static imports or perform the other usual initialization steps. 
                /// Use this flag when you want to load a DLL only to extract messages or resources from it. 
                /// Unless the application depends on the file having the in-memory layout of an image, 
                /// this value should be used with either LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE or LOAD_LIBRARY_AS_DATAFILE. 
                /// </summary>
                LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020
            }

            /// <summary>
            /// Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.
            /// </summary>
            /// <param name="lpFileName">A string that specifies the file name of the module to load.</param>
            /// <param name="hFile">This parameter is reserved for future use. It must be <see cref="IntPtr.Zero"/>.</param>
            /// <param name="dwFlags">The action to be taken when loading the module.</param>
            /// <returns>If the function succeeds, the return value is a handle to the loaded module. If the function fails, the return value is <see cref="IntPtr.Zero"/>.</returns>
            [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern IntPtr LoadLibraryEx(String lpFileName, IntPtr hFile, LoadLibraryExFlags dwFlags);

            /// <summary>
            /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count. When the reference count reaches zero, the module is unloaded from the address space of the calling process and the handle is no longer valid.
            /// </summary>
            /// <param name="hModule">A handle to the loaded library module</param>
            /// <returns>If the function succeeds, the return value is <c>true</c>. If the function fails, the return value is <c>false</c>.</returns>
            [DllImport("Kernel32", SetLastError = true)]
            public static extern Boolean FreeLibrary(IntPtr hModule);
        }
    }
}
