namespace JanHafner.Toolkit.Windows.HotKey
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using JanHafner.Toolkit.Windows.Atoms;
    using JetBrains.Annotations;

    /// <summary>
    /// Represents a successful registered global hot key.
    /// </summary>
    public sealed class GlobalHotKey : IDisposable
    {
        private Boolean isDisposed;

        [CanBeNull]
        private GlobalAtom globalAtom;

        private GlobalHotKey(IntPtr windowHandle, [NotNull] GlobalAtom globalAtom, HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            if (globalAtom == null)
            {
                throw new ArgumentNullException(nameof(globalAtom));
            }

            this.WindowHandle = windowHandle;
            this.globalAtom = globalAtom;
            this.HotKeyModifier = hotKeyModifier;
            this.VirtualKeyCode = virtualKeyCode;
        }

        /// <summary>
        /// Gets the window handle this hot key is associated with.
        /// </summary>
        public IntPtr WindowHandle { get; private set; }

        /// <summary>
        /// Gets the id of the hot key.
        /// </summary>
        public UInt16 Id
        {
            get
            {
                if (this.globalAtom == null)
                {
                    return 0;
                }

                return this.globalAtom.Id;
            }
        }

        /// <summary>
        /// Gets the modifier key of the hot key.
        /// </summary>
        public HotKeyModifier HotKeyModifier { get; private set; }

        /// <summary>
        /// Gets the key of the hot key.
        /// </summary>
        public UInt32 VirtualKeyCode { get; private set; }

        /// <summary>
        /// Registers a new hot key and uses a <see cref="Guid"/> as identifier name.
        /// </summary>
        /// <param name="windowHandle">The handle of the associated window which will receive WM_HOTKEY.</param>
        /// <param name="hotKeyModifier">The associated <see cref="HotKey.HotKeyModifier"/>.</param>
        /// <param name="virtualKeyCode">The associated virtual key code.</param>
        /// <returns>The registered hot key.</returns>
        [NotNull]
        public static GlobalHotKey Register(IntPtr windowHandle, HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(windowHandle));
            }

            if (hotKeyModifier == HotKeyModifier.None)
            {
                throw new ArgumentOutOfRangeException(nameof(hotKeyModifier));
            }

            return GlobalHotKey.RegisterCore(windowHandle, hotKeyModifier, virtualKeyCode);
        }

        [NotNull]
        private static GlobalHotKey RegisterCore(IntPtr windowHandle, HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            if (hotKeyModifier == HotKeyModifier.None)
            {
                throw new ArgumentException(String.Empty, nameof(hotKeyModifier));
            }

            var globalAtomName = Guid.NewGuid().ToString();
            var globalAtom = (GlobalAtom)GlobalAtom.CreateNew(globalAtomName);

            var registerHotKeyResult = NativeMethods.RegisterHotKey(windowHandle, globalAtom.Id, (UInt32)hotKeyModifier, virtualKeyCode);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (!registerHotKeyResult)
            {
                // It is possible that the next line throws...
                globalAtom.Delete();   
                
                throw new Win32Exception(lastWin32Error); 
            }

            return new GlobalHotKey(windowHandle, globalAtom, hotKeyModifier, virtualKeyCode);
        }

        /// <summary>
        /// Registers the supplied hot key and returns a value indicating the registration was successful.
        /// The hot key will be unregistered before the method returns.
        /// </summary>
        /// <param name="hotKeyModifier">The associated <see cref="HotKey.HotKeyModifier"/>.</param>
        /// <param name="virtualKeyCode">The associated virtual key code.</param>
        /// <returns>A value, indicating whether, the hot key is unused.</returns>
        public static Boolean Probe(HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            if (hotKeyModifier == HotKeyModifier.None)
            {
                throw new ArgumentException(String.Empty, nameof(hotKeyModifier));
            }

            try
            {
                using (RegisterCore(IntPtr.Zero, hotKeyModifier, virtualKeyCode))
                {
                    return true;
                }
            }
            catch (Win32Exception win32Exception)
            {
                if (win32Exception.NativeErrorCode == (Int32) NativeMethods.HotKeyErrorCode.ERROR_HOTKEY_ALREADY_REGISTERED)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        /// Unregisters this hot key instance.
        /// </summary>
        /// <remarks>
        /// If this instance was created be supplying the <see cref="GlobalAtom"/> manually, it is not automaticall freed.
        /// </remarks>
        /// <exception cref="Win32Exception">If the function fails, a <see cref="Win32Exception"/> containing the reason is thrown.</exception>
        /// <exception cref="ObjectDisposedException">Is thrown if <see cref="Unregister"/> was successfully called already.</exception>
        public void Unregister()
        {
            this.CheckDisposed();

            try
            {
                this.UnregisterHotKey();
            }
            finally
            {
                this.globalAtom.Delete();
                this.globalAtom = null;
            }
        }

        private void UnregisterHotKey()
        {
            // I hope if the window handle is destroyed from outside [DestroyWindow()] that the hot key 
            // will be invalidated too.
            // If the hot key is still active but associated with a invalid window handle
            // it is impossible to unregister it here.
            // Same applies to the global atom whose id is accessible through the Id property
            // of the GlobalHotKey class.
            var unregisterHotKeyResult = NativeMethods.UnregisterHotKey(this.WindowHandle, this.Id);
            if (!unregisterHotKeyResult)
            {
                throw new Win32Exception();
            }

            this.HotKeyModifier = HotKeyModifier.None;
            this.VirtualKeyCode = 0;
            this.WindowHandle = IntPtr.Zero;
            this.isDisposed = true;
        }

        private void Dispose(Boolean disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.UnregisterHotKey();

            if (disposing)
            {
                this.globalAtom.Dispose();
                this.globalAtom = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GlobalHotKey()
        {
            this.Dispose(false);   
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.globalAtom?.Name ?? this.GetType().Name);
            }
        }

        /// <summary>
        /// Checks if <c>this</c> contains the supplied modifier and virtual key code.
        /// </summary>
        /// <param name="hotKeyModifier">The modifier keys.</param>
        /// <param name="virtualKeyCode">The virtual key code.</param>
        /// <returns><c>True</c> if <paramref name="hotKeyModifier"/> and <paramref name="virtualKeyCode"/> match, otherweise <c>false</c>.</returns>
        public Boolean Equals(HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            return this.VirtualKeyCode == virtualKeyCode && this.HotKeyModifier == hotKeyModifier;
        }

        private static class NativeMethods
        {
            /// <summary>
            /// Defines error codes possibly thrown by RegisterHotKey.
            /// </summary>
            public enum HotKeyErrorCode
            {
                /// <summary>
                /// Hot key is already registered.
                /// </summary>
                ERROR_HOTKEY_ALREADY_REGISTERED = 1409
            }

            /// <summary>
            /// Defines a system-wide hot key.
            /// </summary>
            /// <param name="hwnd">A handle to the window that will receive WM_HOTKEY messages generated by the hot key. If this parameter is NULL, WM_HOTKEY messages are posted to the message queue of the calling thread and must be processed in the message loop.</param>
            /// <param name="id">The identifier of the hot key. If the hWnd parameter is NULL, then the hot key is associated with the current thread rather than with a particular window. If a hot key already exists with the same hWnd and id parameters, see Remarks for the action taken.</param>
            /// <param name="modifier">The keys that must be pressed in combination with the key specified by the uVirtKey parameter in order to generate the WM_HOTKEY message. The fsModifiers parameter can be a combination of the following values.</param>
            /// <param name="virtualKeyCode">The virtual-key code of the hot key. See Virtual Key Codes.</param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern Boolean RegisterHotKey(IntPtr hwnd, Int32 id, UInt32 modifier, UInt32 virtualKeyCode);

            /// <summary>
            /// Frees a hot key previously registered by the calling thread.
            /// </summary>
            /// <param name="hwnd">A handle to the window associated with the hot key to be freed. This parameter should be NULL if the hot key is not associated with a window.</param>
            /// <param name="id">The identifier of the hot key to be freed.</param>
            /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern Boolean UnregisterHotKey(IntPtr hwnd, Int32 id);
        }
    }
}