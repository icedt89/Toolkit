namespace JanHafner.Toolkit.Windows
{
    using System;

    public sealed class DefaultBrowser
    {
        private static readonly Lazy<DefaultBrowser> defaultBrowser = new Lazy<DefaultBrowser>(() =>
        {
            Int32? identifier = null;
            var iconIdentifierType = IconIdentifierType.Index;
            var iconFile = SafeNativeMethods.RetrieveAssociatedIcon("http", out identifier, out iconIdentifierType);

            var exePath = SafeNativeMethods.RetrieveAssociatedExecutable("http");

            return new DefaultBrowser(exePath, iconIdentifierType, identifier, iconFile);
        });

        private DefaultBrowser(String exePath, IconIdentifierType iconIdentifierType, Int32? identifier, String iconFile)
        {
            this.ExePath = exePath;
            this.IconIdentifierType = iconIdentifierType;
            this.Identifier = identifier;
            this.IconFile = iconFile;
        }

        public String ExePath { get; private set; }

        public IconIdentifierType IconIdentifierType { get; private set; }

        public Int32? Identifier { get; private set; }

        public String IconFile { get; private set; }

        public static DefaultBrowser Current
        {
            get
            {
                return DefaultBrowser.defaultBrowser.Value;
            }
        }
    }
}