namespace JanHafner.Toolkit.Windows.Icons
{
    using System;
    using System.IO;
    using JetBrains.Annotations;

    internal sealed class IconFileHeader
    {
        public IconFileHeader([NotNull] BinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException(nameof(binaryReader));
            }

            this.Reserved = binaryReader.ReadInt16();
            this.Type = binaryReader.ReadInt16();
            this.EntryCount = binaryReader.ReadInt16();
        }

        public Int16 Reserved { get; private set; }

        public Int16 Type { get; private set; }

        public Int16 EntryCount { get; private set; }
    }
}