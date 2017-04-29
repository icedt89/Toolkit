namespace JanHafner.Toolkit.Windows
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public sealed class NativeExecutable : NativeModule
    {
        public NativeExecutable([NotNull] String file) 
            : base(file)
        {
        }

        [CanBeNull]
        public String GetResourceString([NotNull] NativeResourceDescriptor nativeResourceDescriptor)
        {
            if (nativeResourceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(nativeResourceDescriptor));
            }

            var resourceId = nativeResourceDescriptor.ResourceId;
            return this.GetResourceString(resourceId);
        }

        [CanBeNull]
        public String GetResourceString(UInt32 resourceId)
        {
            var resultBuffer = new StringBuilder(2048);
            var result = NativeMethods.LoadString(this.Handle, resourceId, resultBuffer, resultBuffer.MaxCapacity);
            if (result < 1)
            {
                throw new Win32Exception();
            }

            return resultBuffer.ToString();
        }

        [CanBeNull]
        public Icon ExtractIconResource([NotNull] NativeResourceDescriptor nativeResourceDescriptor)
        {
            if (nativeResourceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(nativeResourceDescriptor));
            }

            return this.ExtractIconResource(nativeResourceDescriptor, Size.Empty);
        }

        [CanBeNull]
        public Icon ExtractIconResource([NotNull] NativeResourceDescriptor nativeResourceDescriptor, Size desiredSize)
        {
            if (nativeResourceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(nativeResourceDescriptor));
            }

            var resourceId = nativeResourceDescriptor.ResourceId;
            return this.ExtractIconResource(resourceId, desiredSize);
        }

        [NotNull]
        public Icon ExtractIconResource(UInt32 resourceId)
        {
            return this.ExtractIconResource(resourceId, Size.Empty);
        }
        
        [NotNull]
        public Icon ExtractIconResource(UInt32 resourceId, Size desiredSize)
        {
            var iconHandle = NativeMethods.LoadImage(this.Handle, resourceId, NativeMethods.LoadImageType.IMAGE_ICON, desiredSize.Width, desiredSize.Height, NativeMethods.LoadImageLoadResult.LR_DEFAULTCOLOR);
            if (iconHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            return iconHandle.ToIconWithOwnership();
        }

        private static Boolean IsIntResource(IntPtr value)
        {
            return (UInt32)value <= UInt16.MaxValue;
        }

        [NotNull]
        public IEnumerable<UInt32> EnumerateIconResources(IntPtr resourceType, CancellationToken cancellationToken)
        {
            var resourceIds = new HashSet<UInt32>();

            NativeMethods.EnumResNameProc enumResNameProc = (module, type, value, param) =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (NativeExecutable.IsIntResource(value))
                {
                    resourceIds.Add((UInt32)Math.Abs(value.ToInt32()));
                }
                else
                {
                    // value is a pointer to a string which represants the name of the resource
                    var resourceName = Marshal.PtrToStringAnsi(value);
                    Int32 resourceId;
                    if (NativeResourceDescriptor.TryParseResourceIdentifier(resourceName, out resourceId))
                    {
                        resourceIds.Add((UInt32)Math.Abs(resourceId));
                    }
                }

                return true;
            };

            var result = NativeMethods.EnumResourceNames(this.Handle, resourceType, enumResNameProc, IntPtr.Zero);
            // This happens if the specified file contains no resources of the desired type.
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (result || lastWin32Error == (Int32)NativeMethods.SystemErrorCode.ERROR_RESOURCE_TYPE_NOT_FOUND || lastWin32Error == (Int32)NativeMethods.SystemErrorCode.ERROR_RESOURCE_ENUM_USER_STOP)
            {
                return resourceIds;
            }

            throw new Win32Exception(lastWin32Error);
        }
        
        [NotNull]
        public async Task<IEnumerable<IconResourceBag>> ExtractIconsAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var result = new List<IconResourceBag>();

                var resourceIds = new HashSet<UInt32>();
                resourceIds.UnionWith(this.EnumerateIconResources(NativeMethods.RT_GROUP_ICON, cancellationToken));
                resourceIds.UnionWith(this.EnumerateIconResources(NativeMethods.RT_ICON, cancellationToken));

                foreach (var resourceId in resourceIds)
                {
                    var iconHandle = NativeMethods.LoadImage(this.Handle, resourceId, NativeMethods.LoadImageType.IMAGE_ICON, 0, 0, NativeMethods.LoadImageLoadResult.LR_DEFAULTCOLOR);
                    var lastWin32Error = Marshal.GetLastWin32Error();
                    if (iconHandle == IntPtr.Zero)
                    {
                        // This happens sometimes... i have observed this behavior on
                        // resource number 55 in Shell32.dll on Windows 8.1 64 bit.
                        if (lastWin32Error == (Int32)NativeMethods.SystemErrorCode.ERROR_RESOURCE_TYPE_NOT_FOUND)
                        {
                            continue;
                        }

                        throw new Win32Exception(lastWin32Error);
                    }

                    result.Add(new IconResourceBag((Int32)resourceId, IconIdentifierType.ResourceId, iconHandle.ToIconWithOwnership()));
                }

                return result;
            }, cancellationToken);
        }

        internal static class NativeMethods
        {
            /// <summary>
            /// Hardware-dependent icon resource.
            /// </summary>
            public static readonly IntPtr RT_ICON = new IntPtr(3);

            /// <summary>
            /// Hardware-independent icon resource.
            /// </summary>
            public static readonly IntPtr RT_GROUP_ICON = RT_ICON + 11;

            public enum LoadImageType : uint
            {
                /// <summary>
                /// Loads an icon.
                /// </summary>
                IMAGE_ICON = 1
            }

            public enum SystemErrorCode : uint
            {
                /// <summary>
                /// The specified resource type cannot be found in the image file.
                /// </summary>
                ERROR_RESOURCE_TYPE_NOT_FOUND = 1813,

                /// <summary>
                /// User stopped resource enumeration.
                /// </summary>
                ERROR_RESOURCE_ENUM_USER_STOP = 15106
            }

            public enum LoadImageLoadResult : uint
            {
                /// <summary>
                /// The default flag; it does nothing.
                /// </summary>
                LR_DEFAULTCOLOR = 0
            }
            
            /// <summary>
            /// Loads an icon, cursor, animated cursor, or bitmap.
            /// </summary>
            /// <param name="hinst">A handle to the module of either a DLL or executable (.exe) that contains the image to be loaded.</param>
            /// <param name="lpszName">The image to be loaded.</param>
            /// <param name="uType">The type of image to be loaded.</param>
            /// <param name="cxDesired">The width, in pixels, of the icon or cursor.</param>
            /// <param name="cyDesired">The height, in pixels, of the icon or cursor.</param>
            /// <param name="fuLoad">Specifies how to load the image.</param>
            /// <returns>If the function succeeds, the return value is the handle of the newly loaded image. If the function fails, the return value is <see cref="IntPtr.Zero"/>.</returns>
            [DllImport("User32", SetLastError = true)]
            public static extern IntPtr LoadImage(IntPtr hinst, UInt32 lpszName, LoadImageType uType, Int32 cxDesired, Int32 cyDesired, LoadImageLoadResult fuLoad);

            /// <summary>
            /// Loads a string resource from the executable file associated with a specified module, copies the string into a buffer, and appends a terminating null character.
            /// </summary>
            /// <param name="hInstance">A handle to an instance of the module whose executable file contains the string resource.</param>
            /// <param name="uID">The identifier of the string to be loaded.</param>
            /// <param name="lpBuffer">The buffer is to receive the string.</param>
            /// <param name="nBufferMax">The size of the buffer, in characters. The string is truncated and null-terminated if it is longer than the number of characters specified. If this parameter is 0, then lpBuffer receives a read-only pointer to the resource itself.</param>
            /// <returns>If the function succeeds, the return value is the number of characters copied into the buffer, not including the terminating null character, or zero if the string resource does not exist.</returns>
            [DllImport("User32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern Int32 LoadString(IntPtr hInstance, UInt32 uID, StringBuilder lpBuffer, Int32 nBufferMax);

            /// <summary>
            /// Enumerates resources of a specified type within a binary module. 
            /// </summary>
            /// <param name="hModule">A handle to a module to be searched.</param>
            /// <param name="lpszType">The type of the resource for which the name is being enumerated.</param>
            /// <param name="lpEnumFunc">A pointer to the callback function to be called for each enumerated resource name or ID.</param>
            /// <param name="lParam">An application-defined value passed to the callback function.</param>
            /// <returns>The return value is <c>true</c> if the function succeeds or <c>false</c> if the function does not find a resource of the type specified, or if the function fails for another reason.</returns>
            [DllImport("Kernel32", SetLastError = true)]
            public static extern Boolean EnumResourceNames(IntPtr hModule, IntPtr lpszType, EnumResNameProc lpEnumFunc, IntPtr lParam);

            /// <summary>
            /// An application-defined callback function used with the EnumResourceNames and EnumResourceNamesEx functions. It receives the type and name of a resource.
            /// </summary>
            /// <param name="hModule">A handle to the module whose executable file contains the resources that are being enumerated.</param>
            /// <param name="lpszType ">The type of resource for which the name is being enumerated.</param>
            /// <param name="lpszName">The name of a resource of the type being enumerated. </param>
            /// <param name="lParam">An application-defined parameter.</param>
            /// <returns>Returns <c>true</c> to continue enumeration or <c>false</c> to stop enumeration.</returns>
            public delegate Boolean EnumResNameProc(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);
        }
    }
}