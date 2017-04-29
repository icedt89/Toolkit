namespace JanHafner.Toolkit.Windows.Hooks
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;

    /// <summary>
    /// Base class for all hooks.
    /// </summary>
    public abstract class WindowsHook : IDisposable
    {
        private readonly HookType hookType;

        private IntPtr hookHandle;

        private Boolean isDisposed;

        protected internal WindowsHook(HookType hookType)
        {
            this.hookType = hookType;
        }

        /// <summary>
        /// When overridden in a derived class, handles the hook.
        /// </summary>
        protected abstract void HandleHook(HookCode nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// If <paramref name="nCode"/> is &lt; 0 <see cref="HandleHook"/> will not be called.
        /// When overridden in a derived class, the implementor should ensure to return the value returned by <see cref="CallNextHook"/>.
        /// When overridden in a derived class and this function returns ea value lower than IntPtr.Zero and <see cref="CallNextHook"/> was not called,
        /// the message will be omitted. 
        /// E.g. Hooking low level keyboard actions and returning -1 without calling <see cref="CallNextHook"/> can lead to suppressing all key events for the whole system.
        /// </summary>
        /// <returns></returns>
        protected virtual IntPtr NextHook(HookCode nCode, IntPtr wParam, IntPtr lParam)
        {
            this.CheckDisposed();

            if (nCode >= 0)
            {
                try
                {
                    this.HandleHook(nCode, wParam, lParam);
                }
                catch (Exception)
                {
                    // TODO: Implement handling if i know how to handle such a case.

                    Debugger.Break();
                }
            }

            return this.CallNextHook(nCode, wParam, lParam);
        }

        protected IntPtr CallNextHook(HookCode nCode, IntPtr wParam, IntPtr lParam)
        {
            this.CheckDisposed();

            return NativeMethods.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        /// <summary>
        /// Installs this hook instance.
        /// </summary>
        public void Install()
        {
            this.CheckDisposed();

            var hookAssembly = Assembly.GetExecutingAssembly();

            var module = hookAssembly.Modules.First();

            this.Install(module);
        }

        private void Install([NotNull] Module module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            this.Install(module.Name);
        }

        private void Install([NotNull] String moduleName)
        {
            if (String.IsNullOrEmpty(moduleName))
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            var moduleHandle = NativeMethods.LoadLibrary(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            this.Install(moduleHandle, 0);
        }

        private void Install(IntPtr associatedModuleHandle, UInt32 threadId)
        {
            this.hookHandle = NativeMethods.SetWindowsHookEx(this.hookType, this.NextHook, associatedModuleHandle, threadId);
            if (this.hookHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Deinstalls this hook.
        /// </summary>
        public void Uninstall()
        {
            this.CheckDisposed();

            this.UninstallCore();
        }

        private void UninstallCore()
        {
            var result = NativeMethods.UnhookWindowsHookEx(this.hookHandle);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (!result)
            {
                if (lastWin32Error != (Int32)NativeMethods.HookErrorCode.ERROR_INVALID_HOOK_HANDLE)
                {
                    throw new Win32Exception(lastWin32Error);
                }
            }

            this.hookHandle = IntPtr.Zero;

            this.isDisposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.UninstallCore();
        }

        ~WindowsHook()
        {
            this.Dispose(false);
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(null);
            }
        }

        private static class NativeMethods
        {
            public enum HookErrorCode
            {
                /// <summary>
                /// Invalid hook handle.
                /// </summary>
                ERROR_INVALID_HOOK_HANDLE = 1404
            }

            public delegate IntPtr HookProc(HookCode nCode, IntPtr wParam, IntPtr lparam);

            /// <summary>
            /// Installs an application-defined hook procedure into a hook chain. 
            /// You would install a hook procedure to monitor the system for certain types of events. 
            /// These events are associated either with a specific thread or with all threads in the same desktop as the calling thread.
            /// </summary>
            /// <param name="idHook">The type of hook procedure to be installed.</param>
            /// <param name="lpfn">A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread created by a different process, the lpfn parameter must point to a hook procedure in a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current process.</param>
            /// <param name="hMod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be set to <c>NULL</c> if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process.</param>
            /// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated. For desktop apps, if this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread. For Windows Store apps, see the Remarks section.</param>
            /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
            [DllImport("user32", SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(HookType idHook, [MarshalAs(UnmanagedType.FunctionPtr)] HookProc lpfn,
                IntPtr hMod, UInt32 dwThreadId);


            /// <summary>
            /// Removes a hook procedure installed in a hook chain by the <see cref="SetWindowsHookEx"/> function.
            /// </summary>
            /// <param name="hhk">A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to <see cref="SetWindowsHookEx"/>.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("user32", SetLastError = true)]
            public static extern Boolean UnhookWindowsHookEx(IntPtr hhk);

            /// <summary>
            /// Passes the hook information to the next hook procedure in the current hook chain. 
            /// A hook procedure can call this function either before or after processing the hook information.
            /// </summary>
            /// <param name="hhk">This parameter is ignored.</param>
            /// <param name="nCode">The hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
            /// <param name="wParam">The wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <param name="lParam">The lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type. For more information, see the descriptions of the individual hook procedures.</returns>
            [DllImport("user32", SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, HookCode nCode, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// Used to get a valid module handle to a library.
            /// </summary>
            /// <param name="fileName">Will be User32.dll.</param>
            /// <returns>The module handle.</returns>
            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern IntPtr LoadLibrary(String fileName);
        }
    }
}