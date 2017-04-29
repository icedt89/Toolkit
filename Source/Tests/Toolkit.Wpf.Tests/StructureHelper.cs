namespace JanHafner.Toolkit.Wpf.Tests
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class DisposableStructure<T> : IDisposable
        where T : struct
    {
        public DisposableStructure(T structure, IntPtr structurePointer)
        {
            if (structurePointer == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(structurePointer));
            }

            this.Structure = structure;
            this.StructurePointer = structurePointer;
        }

        public T Structure { get; set; }

        public IntPtr StructurePointer { get; set; }

        public void Dispose()
        {
            if (this.StructurePointer != IntPtr.Zero)
            {
                Marshal.DestroyStructure(this.StructurePointer, typeof(T));
            }

            this.StructurePointer = IntPtr.Zero;
        }

        public static DisposableStructure<T> Create()
        {
            var structureSize = Marshal.SizeOf<T>();
            var structurePointer = Marshal.AllocHGlobal(structureSize);

            var structure = new T();
            Marshal.StructureToPtr(structure, structurePointer, true);

            return new DisposableStructure<T>(structure, structurePointer);
        } 
    }
}
