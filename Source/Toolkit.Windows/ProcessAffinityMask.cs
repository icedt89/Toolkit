namespace JanHafner.Toolkit.Windows
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;

    /// <summary>
    /// All methods of this class do not manipulate the affinity mask of a process!
    /// </summary>
    public sealed class ProcessAffinityMask : IEquatable<ProcessAffinityMask>, ICloneable
    {
        private readonly UInt32 systemAffinityMask;

        [NotNull]
        private readonly BitArray systemAffinityMaskBits;

        [NotNull]
        private readonly BitArray affinityMaskBits;

        private ProcessAffinityMask(UInt32 affinityMask, UInt32 systemAffinityMask)
        {
            this.systemAffinityMask = systemAffinityMask;
            this.systemAffinityMaskBits =  new BitArray(BitConverter.GetBytes(systemAffinityMask));
            this.affinityMaskBits = new BitArray(BitConverter.GetBytes(affinityMask));
        }

        public UInt32 SystemAffinityMask
        {
            get { return this.systemAffinityMask; }
        }

        public static ProcessAffinityMask FromAffinityMask(UInt32 affinityMask)
        {
            var result = ProcessAffinityMask.ForCurrentProcess();
            result =  new ProcessAffinityMask(affinityMask, result.systemAffinityMask);
            if (!result.IsAffinityMaskValid())
            {
                throw new InvalidOperationException("Affinity mask is invalid.");
            }

            return result;
        }

        [NotNull]
        public static ProcessAffinityMask ForSystem()
        {
            var processAffinityMaskForSystem = ProcessAffinityMask.ForCurrentProcess();

            return new ProcessAffinityMask(processAffinityMaskForSystem.SystemAffinityMask, processAffinityMaskForSystem.SystemAffinityMask);
        }

        [NotNull]
        public static ProcessAffinityMask ForCurrentProcess()
        {
            return ProcessAffinityMask.ForProcess(Process.GetCurrentProcess());
        }

        [NotNull]
        public static ProcessAffinityMask ForProcess(IntPtr processHandle)
        {
            UInt32 processAffinityMask;
            UInt32 systemAffinityMask;

            var result = NativeMethods.GetProcessAffinityMask(processHandle, out processAffinityMask, out systemAffinityMask);
            if (!result)
            {
                throw new Win32Exception();
            }

            return new ProcessAffinityMask(processAffinityMask, systemAffinityMask);
        }

        [NotNull]
        public static ProcessAffinityMask ForProcess([NotNull] Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            return ForProcess(process.Handle);
        }

        public void SetBit(Int32 position, Boolean value)
        {
            if (value && !this.CanSet(position))
            {
                throw new InvalidOperationException("Affinity mask would become invalid.");
            }

            this.affinityMaskBits.Set(position, value);
        }

        private Boolean IsBitValid(Int32 position)
        {
            return !this.affinityMaskBits[position] || this.CanSet(position);
        }

        public Boolean CanSet(Int32 position)
        {
            return this.systemAffinityMaskBits[position];
        }

        public Boolean IsAffinityMaskValid()
        {
            for (var i = 0; i < this.systemAffinityMaskBits.Count; i++)
            {
                if (!this.IsBitValid(i))
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<Boolean> AffinityMaskBits
        {
            get { return this.affinityMaskBits.Cast<Boolean>(); }
        }

        public Int32 SelectedBitCount
        {
            get { return this.AffinityMaskBits.Count(bit => bit); }
        }

        public UInt32 AffinityMask
        {
            get { return this.affinityMaskBits.ToUInt32(); }
        }

        public Boolean Equals(ProcessAffinityMask other)
        {
            return this.AffinityMask.Equals(other.AffinityMask);
        }

        public Boolean IsSystemAffinityMask
        {
            get { return this.SystemAffinityMask == this.AffinityMask; }
        }

        public Object Clone()
        {
            return ProcessAffinityMask.FromAffinityMask(this.AffinityMask);
        }

        internal static class NativeMethods
        {
            [DllImport("Kernel32.dll", SetLastError = true)]
            public static extern Boolean GetProcessAffinityMask(IntPtr hProcess, out UInt32 lpProcessAffinityMask, out UInt32 lpSystemAffinityMask);
        }
    }
}
