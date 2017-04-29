namespace JanHafner.Toolkit.Windows.Icons
{
    using System;
    using System.IO;
    using JetBrains.Annotations;

    internal sealed class IconDirectoryEntry
    {
        public IconDirectoryEntry([NotNull] BinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException(nameof(binaryReader));
            }

            this.Width = binaryReader.ReadByte();
            this.Height = binaryReader.ReadByte();
            this.ColorCount = binaryReader.ReadByte();
            this.Reserved = binaryReader.ReadByte();
            this.ColorPlanes = binaryReader.ReadInt16();
            this.BitsPerPixel = binaryReader.ReadInt16();
            this.SizeOfData = binaryReader.ReadInt32();
            this.OffsetOfData = binaryReader.ReadInt32();
        }

        public Byte Width { get; private set; }

        public Byte Height { get; private set; }

        public Byte ColorCount { get; private set; }

        public Byte Reserved { get; private set; }

        public Int16 ColorPlanes { get; private set; }

        public Int16 BitsPerPixel { get; private set; }

        public Int32 SizeOfData { get; private set; }

        public Int32 OffsetOfData { get; private set; }
    }
}