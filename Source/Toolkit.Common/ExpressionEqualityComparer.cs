namespace JanHafner.Toolkit.Common
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// Creates an <see cref="EqualityComparer{T}"/> comparer from a selector function.
    /// </summary>
    /// <typeparam name="TSource">The <see cref="Type"/> of the source.</typeparam>
    /// <typeparam name="TKey">The <see cref="Type"/> of the selected property.</typeparam>
    public sealed class ExpressionEqualityComparer<TSource, TKey> : EqualityComparer<TSource>
    {
        [NotNull]
        private readonly Func<TSource, TKey> selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEqualityComparer&lt;TSource, TKey&gt;"/> class.
        /// </summary>
        /// <param name="selector">The <see cref="Func{TSource, TKey}"/> which selects the property to compare.</param>
        public ExpressionEqualityComparer([NotNull] Func<TSource, TKey> selector)
        {
            this.selector = selector;
        }

        /// <ineritdoc />
        public override Boolean Equals([CanBeNull] TSource x, [CanBeNull] TSource y)
        {
            if (Equals(x, null) && Equals(y, null))
            {
                return true;
            }

            if (Equals(x, null) || Equals(y, null))
            {
                return false;
            }

            var xValue = this.selector(x);
            var yValue = this.selector(y);
            return EqualityComparer<TKey>.Default.Equals(xValue, yValue);
        }

        /// <ineritdoc />
        public override Int32 GetHashCode(TSource obj)
        {
            return Equals(obj, null) ? 0 : EqualityComparer<TKey>.Default.GetHashCode(this.selector(obj));
        }
    }
}