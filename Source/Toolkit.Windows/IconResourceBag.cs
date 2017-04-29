namespace JanHafner.Toolkit.Windows
{
    using System;
    using System.Drawing;
    using JetBrains.Annotations;

    public sealed class IconResourceBag
    {
        public IconResourceBag(Int32 identifier, IconIdentifierType identifierType, [NotNull] Icon icon)
        {
            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }

            this.Identifier = identifier;
            this.Icon = icon;
            this.IdentifierType = identifierType;
        }
        
        public Int32 Identifier { get; private set; }

        [NotNull]
        public Icon Icon { get; private set; }

        public IconIdentifierType IdentifierType { get; private set; }
    }
}
