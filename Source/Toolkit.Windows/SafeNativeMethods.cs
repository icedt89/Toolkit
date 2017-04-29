namespace JanHafner.Toolkit.Windows
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;

    public static class SafeNativeMethods
    {
        [NotNull]
        public static String FindExecutable([NotNull] String file)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            var executableBuffer = new StringBuilder(2048);
            var result = NativeMethods.FindExecutable(file, null, executableBuffer).ToInt32();
            if (result <= 32)
            {
                result.ThrowExceptionFromFindExecutableResult(file);
            }

            var executable = executableBuffer.ToString();
            return Path.GetFullPath(Environment.ExpandEnvironmentVariables(executable));
        }

        private static void ThrowExceptionFromFindExecutableResult(this Int32 findExecutableResult, String file)
        {
            switch (findExecutableResult)
            {
                case (Int32)NativeMethods.FindExecutableResult.SE_ERR_NOASSOC:
                    throw new ExecutableNotFoundException(file);
                default:
                    throw new Win32Exception(findExecutableResult);
            }
        }

        [NotNull]
        private static String AssocQueryString([NotNull] String extensionOrFileOrProtocol, NativeMethods.ASSOCSTR assocQuery)
        {
            if (String.IsNullOrWhiteSpace(extensionOrFileOrProtocol))
            {
                throw new ArgumentNullException(nameof(extensionOrFileOrProtocol));
            }

            var resultBuffer = new StringBuilder(2048);
            var bufferSize = (UInt32)resultBuffer.Capacity;

            var returnValue = NativeMethods.AssocQueryString(NativeMethods.ASSOCF.ASSOCF_NONE, assocQuery, extensionOrFileOrProtocol, null, resultBuffer, ref bufferSize);
            if (returnValue != NativeMethods.AssocQueryStringResult.S_OK)
            {
                if (returnValue == NativeMethods.AssocQueryStringResult.COM_NoAssociation)
                {
                    return String.Empty;
                }

                throw new Win32Exception();
            }

            return resultBuffer.ToString();
        }

        [NotNull]
        public static String RetrieveAssociatedExecutable([NotNull] String protocol)
        {
            return SafeNativeMethods.AssocQueryString(protocol, NativeMethods.ASSOCSTR.ASSOCSTR_EXECUTABLE);
        }

        [CanBeNull]
        public static String RetrieveAssociatedIcon([NotNull] String extensionOrFileOrProtocol, [CanBeNull] out Int32? identifier, out IconIdentifierType identifierType)
        {
            if (String.IsNullOrWhiteSpace(extensionOrFileOrProtocol))
            {
                throw new ArgumentNullException(nameof(extensionOrFileOrProtocol));
            }

            identifier = null;
            identifierType = IconIdentifierType.Unknown;

            // Special case: The result buffer contains "%1". We treat this case like: "oh hey, your supplied file path is already the file containing the icon!".
            var result = SafeNativeMethods.AssocQueryString(extensionOrFileOrProtocol, NativeMethods.ASSOCSTR.ASSOCSTR_DEFAULTICON);
            if (result == "%1")
            {
                return String.Empty;
            }

            if (result.IndexOf(',') == -1)
            {
                identifier = 0;
                identifierType = IconIdentifierType.Index;
                return result;
            }

            Int32 extractedIdentifier;
            var file = NativeResourceDescriptor.SplitResourceString(result, out extractedIdentifier);

            identifier = extractedIdentifier;
            identifierType = IconIdentifier.Identify(extractedIdentifier);
            return file;
        }

        public static void DestroyIcon(IntPtr iconHandle)
        {
            var iconFreed = NativeMethods.DestroyIcon(iconHandle);
            if (!iconFreed)
            {
                throw new Win32Exception();
            }
        }

        [CanBeNull]
        public static Icon ExtractIcon([NotNull] String file, Int32 index)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            var result = NativeMethods.ExtractIcon(IntPtr.Zero, file, index);
            switch (result.ToInt32())
            {
                case 0:
                    return null;
                case 1:
                    throw new Win32Exception();
                default:
                    return result.ToIconWithOwnership();
            }
        }

        internal static class NativeMethods
        {
            public enum FindExecutableResult
            {
                /// <summary>
                /// The specified file was not found.
                /// </summary>
                SE_ERR_FNF = 2,

                /// <summary>
                /// The specified path is invalid.
                /// </summary>
                SE_ERR_PNF = 3,

                /// <summary>
                /// The specified file cannot be accessed.
                /// </summary>
                SE_ERR_ACCESSDENIED = 5,

                /// <summary>
                /// The system is out of memory or resources.
                /// </summary>
                SE_ERR_OOM = 8,

                /// <summary>
                /// There is no association for the specified file type with an executable file.
                /// </summary>
                SE_ERR_NOASSOC = 31
            }

            /// <summary>
            /// Provides information to the IQueryAssociations interface methods.
            /// </summary>
            [Flags]
            public enum ASSOCF : uint
            {
                /// <summary>
                /// None of the following options are set.
                /// </summary>
                ASSOCF_NONE = 0
            }

            /// <summary>
            /// Used by IQueryAssociations::GetString to define the type of string that is to be returned.
            /// </summary>
            public enum ASSOCSTR
            {
                /// <summary>
                /// An executable from a Shell verb command string. 
                /// For example, this string is found as the (Default) value for a subkey such as HKEY_CLASSES_ROOT\ApplicationName\shell\Open\command. 
                /// If the command uses Rundll.exe, set the ASSOCF_REMAPRUNDLL flag in the flags parameter of IQueryAssociations::GetString to retrieve the target executable.
                /// </summary>
                ASSOCSTR_EXECUTABLE = 2,

                /// <summary>
                /// Returns the path to the icon resources to use by default for this association. 
                /// Positive numbers indicate an index into the dll's resource table, while negative numbers indicate a resource ID. 
                /// An example of the syntax for the resource is "c:\myfolder\myfile.dll,-1".
                /// </summary>
                ASSOCSTR_DEFAULTICON = 15
            }

            public enum AssocQueryStringResult : uint
            {
                /// <summary>
                /// Success
                /// </summary>
                S_OK = 0,

                /// <summary>
                /// The buffer is too small to hold the entire string.
                /// </summary>
                E_POINTER = 0x80004003,

                /// <summary>
                /// Buffer is null. pcchOut contains the required buffer size.
                /// </summary>
                S_FALSE = 0x80004005,

                /// <summary>
                /// Special: Returned if used with *.lnk files.
                /// </summary>
                COM_NoAssociation = 2147943555
            }

            /// <summary>
            /// Retrieves the name of and handle to the executable (.exe) file associated with a specific document file.
            /// </summary>
            /// <param name="lpFile">The address of a null-terminated string that specifies a file name. This file should be a document.</param>
            /// <param name="lpDirectory">The address of a null-terminated string that specifies the default directory. This value can be NULL.</param>
            /// <param name="lpResult">The address of a buffer that receives the file name of the associated executable file. This file name is a null-terminated string that specifies the executable file started when an "open" by association is run on the file specified in the lpFile parameter. Put simply, this is the application that is launched when the document file is directly double-clicked or when Open is chosen from the file's shortcut menu. This parameter must contain a valid non-null value and is assumed to be of length MAX_PATH. Responsibility for validating the value is left to the programmer.</param>
            /// <returns>Returns a value greater than 32 if successful, or a value less than or equal to 32 representing an error.</returns>
            [DllImport("Shell32", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern IntPtr FindExecutable(String lpFile, String lpDirectory, StringBuilder lpResult);

            /// <summary>
            /// Searches for and retrieves a file or protocol association-related string from the registry.
            /// </summary>
            /// <param name="flags">The flags that can be used to control the search. It can be any combination of <see cref="ASSOCF" /> values, except that only one ASSOCF_INIT value can be included.</param>
            /// <param name="str">The <see cref="ASSOCSTR"/> value that specifies the type of string that is to be returned.</param>
            /// <param name="pszAssoc">A pointer to a null-terminated string that is used to determine the root key. The following four types of strings can be used.</param>
            /// <param name="pszExtra">An optional null-terminated string with additional information about the location of the string. It is typically set to a Shell verb such as open. Set this parameter to NULL if it is not used.</param>
            /// <param name="pszOut">Pointer to a null-terminated string that, when this function returns successfully, receives the requested string. Set this parameter to NULL to retrieve the required buffer size.</param>
            /// <param name="pcchOut">A pointer to a value that, when calling the function, is set to the number of characters in the pszOut buffer. When the function returns successfully, the value is set to the number of characters actually placed in the buffer.</param>
            /// <returns></returns>
            [DllImport("Shlwapi", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern AssocQueryStringResult AssocQueryString(ASSOCF flags, ASSOCSTR str, String pszAssoc, String pszExtra, StringBuilder pszOut, ref UInt32 pcchOut);

            /// <summary>
            /// Destroys an icon and frees any memory the icon occupied.
            /// </summary>
            /// <param name="hIcon">A handle to the icon to be destroyed. The icon must not be in use.</param>
            /// <returns>If the function succeeds, the return value is <c>true</c>. If the function fails, the return value is <c>false</c>.</returns>
            [DllImport("User32", SetLastError = true)]
            public static extern Boolean DestroyIcon(IntPtr hIcon);

            /// <summary>
            /// Retrieves a handle to an icon from the specified executable file, DLL, or icon file.
            /// </summary>
            /// <param name="hInst">A handle to the instance of the application calling the function.</param>
            /// <param name="lpszExeFileName">The name of an executable file, DLL, or icon file.</param>
            /// <param name="nIconIndex">The zero-based index of the icon to retrieve. For example, if this value is 0, the function returns a handle to the first icon in the specified file.</param>
            /// <returns>The return value is a handle to an icon. If the file specified was not an executable file, DLL, or icon file, the return is 1. If no icons were found in the file, the return value is NULL.</returns>
            [DllImport("Shell32", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern IntPtr ExtractIcon(IntPtr hInst, String lpszExeFileName, Int32 nIconIndex);
        }
    }
}
