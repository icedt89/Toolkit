namespace JanHafner.Toolkit.Common
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a wrapper class for objects which do not implement <see cref="IDisposable"/>.
    /// If the supplied object implements <see cref="IDisposable"/>, the dispose action is ignored and <see cref="IDisposable.Dispose"/> is called instead.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    public class DisposableWrapper<T> : IDisposable
    {
        [NotNull]
        private readonly Action<T> disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableWrapper{T}"/> class.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="disposeAction">The disposeAction that is called to a call to <see cref="IDisposable.Dispose"/> if available.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>' cannot be null. </exception>
        public DisposableWrapper([NotNull] T source, [CanBeNull] Action<T> disposeAction)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));    
            }

            var disposable = source as IDisposable;
            this.disposeAction = _ =>
                                {
                                    disposable?.Dispose();
                                    disposeAction?.Invoke(source);
                                };
            
            this.Indisposable = source;
        }

        /// <summary>
        /// Provides access to the object which is not disposable.
        /// </summary>
        [CanBeNull]
        public T Indisposable { get; private set; }

        /// <summary>
        /// Indicates whether the <see cref="Dispose"/>() method was called on <see langword="this"/> instance.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                try
                {
                    this.disposeAction(this.Indisposable);
                }
                finally
                {
                    this.Indisposable = default(T);
                    this.IsDisposed = true;
                }
            }
        }
    }
}