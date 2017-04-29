namespace JanHafner.Toolkit.Common.Ini
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;

    public static class IniHelper
    {
        public static void ReadIniSection([NotNull] Object container, [NotNull] String iniLikeFile)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var sectionAttribute = container.GetType().GetCustomAttribute<IniSectionAttribute>();
            if (sectionAttribute == null)
            {
                return;
            }

            var section = sectionAttribute.Section;

            var properties = container.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.GetCustomAttribute<IniKeyAttribute>() != null);
            foreach (var property in properties)
            {
                var key = property.GetCustomAttribute<IniKeyAttribute>().Key;
                var value = GetIniKey(iniLikeFile, section, key);

                object convertedValue;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var nullableConverter = new NullableConverter(property.PropertyType);
                    convertedValue = nullableConverter.ConvertFrom(value);
                }
                else
                {
                    convertedValue = Convert.ChangeType(value, property.PropertyType);
                }

                property.SetValue(container, convertedValue);
            }
        }
        
        [CanBeNull]
        public static String GetIniKey(String file, String section, String key)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException(null, file);
            }

            var keyValue = new StringBuilder(1024);
            NativeMethods.GetPrivateProfileString(section, key, String.Empty, keyValue, (UInt32)keyValue.MaxCapacity, file);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != 0)
            {
                throw new Win32Exception(lastWin32Error);
            }

            return Encoding.Default.GetString(Encoding.Convert(Encoding.ASCII, Encoding.Default, Encoding.ASCII.GetBytes(keyValue.ToString())));
        }

        private static class NativeMethods
        {
            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
            public static extern UInt32 GetPrivateProfileString(String appName, String keyName, String @default, StringBuilder returnedString, UInt32 size, String fileName);
        }
    }
}