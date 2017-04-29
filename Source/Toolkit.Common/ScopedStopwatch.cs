namespace JanHafner.Toolkit.Common
{
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a wrapper around the <see cref="Stopwatch"/> which can be used in a using-statement and invokes an <see cref="Action{Stopwatch}"/> before calling Dispose().
    /// </summary>
    public sealed class ScopedStopwatch : DisposableWrapper<Stopwatch>
    {
        [NotNull]
        private static readonly Action<Stopwatch> DefaultLeaveScopeAction = s => Debug.WriteLine(s.Elapsed);

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedStopwatch"/> class.
        /// </summary>
        public ScopedStopwatch()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedStopwatch"/> class.
        /// </summary>
        /// <param name="leaveScopeAction">The action that is called when this class gets disposed.</param>
        public ScopedStopwatch([CanBeNull] Action<Stopwatch> leaveScopeAction)
            : this(new Stopwatch(), leaveScopeAction ?? DefaultLeaveScopeAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedStopwatch"/> class.
        /// </summary>
        /// <param name="source">The stopwatch.</param>
        /// <param name="disposeAction">The action that is called when this class gets disposed.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>' cannot be null. </exception>
        private ScopedStopwatch([NotNull] Stopwatch source, [CanBeNull] Action<Stopwatch> disposeAction)
            : base(source, disposeAction)
        {
            Debug.WriteLine("Consider removing ScopedStopwatch before release build. Iam sure you just wanted to measure execution to improve performance ;-)");

            source.Start();
        }
    }
}
