namespace JanHafner.Toolkit.Windows
{
    using System;
    using JetBrains.Annotations;

    public sealed class NativeResourceDescriptor
    {
        private NativeResourceDescriptor([NotNull] String file, UInt32 resourceId)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.File = Environment.ExpandEnvironmentVariables(file);
            this.ResourceId = resourceId;
        }

        public String File { get; private set; }

        public UInt32 ResourceId { get; private set; }

        public static String SplitResourceString([NotNull] String resourceString, out Int32 identifier)
        {
            if (String.IsNullOrWhiteSpace(resourceString))
            {
                throw new ArgumentNullException(nameof(resourceString));
            }

            var splittedResourceString = resourceString.Split(',');
            if (!TryParseResourceIdentifier(splittedResourceString[1], out identifier))
            {
                throw new InvalidOperationException();
            }

            return splittedResourceString[0].Trim('\"').TrimStart('@');
        }

        public static Boolean TryParseResourceIdentifier([NotNull] String name, out Int32 identifier)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(1);
            }

            return Int32.TryParse(name, out identifier);
        }

        public static NativeResourceDescriptor ParseFromResourceString(String resourceString)
        {
            try
            {
                Int32 identifier;
                var file = NativeResourceDescriptor.SplitResourceString(resourceString, out identifier);

                return new NativeResourceDescriptor(file, (UInt32)Math.Abs(identifier));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot parse '{resourceString}' to NativeResourceDescriptor.", ex);
            }
        }

        public static Boolean TryParseFromResourceString(String resourceString,
            out NativeResourceDescriptor nativeResourceDescriptor)
        {
            try
            {
                nativeResourceDescriptor = NativeResourceDescriptor.ParseFromResourceString(resourceString);
                return true;
            }
            catch
            {
                nativeResourceDescriptor = null;
                return false;
            }
        }
    }
}
