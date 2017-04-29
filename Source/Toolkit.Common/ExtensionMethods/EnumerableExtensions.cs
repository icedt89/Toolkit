namespace JanHafner.Toolkit.Common.ExtensionMethods
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Left joins all elements from the left sequence by the specified key selector function with the elements from the right sequence.
        /// Unjoinable items are returned as their default value.
        /// </summary>
        /// <typeparam name="TLeft">The element <see cref="Type"/> fromt the left sequence.</typeparam>
        /// <typeparam name="TRight">The element <see cref="Type"/> fromt the right sequence.</typeparam>
        /// <typeparam name="TKey">The <see cref="Type"/> of the selected key.</typeparam>
        /// <typeparam name="TResult">The <see cref="Type"/> of the element returned.</typeparam>
        /// <param name="left">The left sequence.</param>
        /// <param name="right">The right sequence.</param>
        /// <param name="leftKeySelector">The key selector function for the left sequence.</param>
        /// <param name="rightKeySelector">The key selector function for the right sequence.</param>
        /// <param name="projection">The selector function for the result.</param>
        /// <returns>A lazy evaluated list of the projected elements.</returns>
        [NotNull]
        [LinqTunnel]
        public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>([NotNull] this IEnumerable<TLeft> left,
            [NotNull] IEnumerable<TRight> right, [NotNull] Func<TLeft, TKey> leftKeySelector,
            [NotNull] Func<TRight, TKey> rightKeySelector, [NotNull] Func<TLeft, TRight, TResult> projection)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (leftKeySelector == null)
            {
                throw new ArgumentNullException(nameof(leftKeySelector));
            }

            if (rightKeySelector == null)
            {
                throw new ArgumentNullException(nameof(rightKeySelector));
            }

            if (projection == null)
            {
                throw new ArgumentNullException(nameof(projection));
            }

            var rightLookup = right.ToLookup(rightKeySelector);

            foreach (var leftItem in left)
            {
                var rightItem = rightLookup[leftKeySelector(leftItem)].SingleOrDefault();

                yield return projection(leftItem, rightItem);
            }
        }

        /// <summary>
        /// Creates groups where each group has the same size.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="elementsPerGroup">The number of elements in the groups.</param>
        /// <returns>A grouped list by the size of the groups.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The '<paramref name="elementsPerGroup"/>' parameter is less than 1.</exception>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<IGrouping<Int32, T>> Batch<T>([NotNull] this IEnumerable<T> source, Int32 elementsPerGroup)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            if (elementsPerGroup == 1)
            {
                throw new ArgumentException("There must be at least one element per group.");
            }

            return source.Select((element, index) => new {element, index}).GroupBy(x => x.index / elementsPerGroup, pair => pair.element);
        }

        /// <summary>
        /// Ensures that only elements are turned which exactly match the type. No inheritors of the type will be returned.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source list.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>' cannot be null. </exception>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<T> OfTypeExactly<T>(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.Cast<object>().Where(@object => @object.GetType() == typeof (T)).Cast<T>();
        }

        /// <summary>
        /// Distincts the supplied list by the property specified.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <typeparam name="TKey">The type of the selected key.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="by">The key selector.</param>
        /// <returns>The distincted list.</returns>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> by)
        {
            return source.Distinct(by, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Distincts the supplied list by the property specified.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <typeparam name="TKey">The type of the selected key.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="by">The key selector.</param>
        /// <param name="equalityComparer">The used equality comparer.</param>
        /// <returns>The distincted list.</returns>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> by, IEqualityComparer<TKey> equalityComparer)
        {
            var hashSet = new HashSet<TKey>(equalityComparer);
            return source.Where(item => hashSet.Add(by(item)));
        }

        /// <summary>
        /// Calls the specified <see cref="Action{T}"/> on each element in the list.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="action">The action to call.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>' and '<paramref name="action"/>' cannot be null. </exception>
        public static void ForEach<T>([NotNull, InstantHandle] this IEnumerable<T> source, [NotNull, InstantHandle] Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Calls the specified <see cref="Action{T}" /> on each element in the list.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="action">The action to call.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>' and '<paramref name="action"/>' cannot be null. </exception>
        /// <returns>The lazy evaluated element of the list the action was applied on.</returns>
        [LinqTunnel]
        public static IEnumerable<T> TunneledForEach<T>([NotNull] this IEnumerable<T> source, [NotNull] Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

            /// <summary>
        /// Removes all elements from the source where the supplied <see cref="Predicate{T}"/> matches.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="selector">The <see cref="Predicate{T}"/>.</param>
        public static IReadOnlyList<KeyValuePair<T, Boolean>> Remove<T>([NotNull, InstantHandle] this ICollection<T> source, [NotNull, InstantHandle] Func<T, Boolean> selector)
            {
                return source.Where(selector).ToList().Select(item => new KeyValuePair<T, Boolean>(item, source.Remove(item))).ToList();
            }

        /// <summary>
        /// Executes the supplied <see cref="Func{T, Int32, Boolean}"/> against all elements in the source list and especially, recursive against all children.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="sourceChildSelector">A <see cref="Func{T, IEnumerable{T}}"/> that selects the children of the current element.</param>
        /// <param name="predicate">The <see cref="Func{T, Int32, Boolean}"/> that is executed against each element.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>', '<paramref name="sourceChildSelector"/>' and '<paramref name="predicate"/>' cannot be null. </exception>
        /// <returns>A flattened list of elements.</returns>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<T> WhereRecursive<T>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, IEnumerable<T>> sourceChildSelector, [NotNull] Func<T, UInt32, Boolean> predicate)
            where T : class
        {
            return source.WhereRecursive(sourceChildSelector, predicate, 0);
        }

        /// <summary>
        /// Executes the supplied <see cref="Func{T, Int32, Boolean}"/> against all elements in the source list and especially, recursive against all children.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="sourceChildSelector">A <see cref="Func{T, IEnumerable{T}}"/> that selects the children of the current element.</param>
        /// <param name="predicate">The <see cref="Func{T, Int32, Boolean}"/> that is executed against each element.</param>
        /// <param name="currentDeep">The current recursion deep.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>', '<paramref name="sourceChildSelector"/>' and '<paramref name="predicate"/>' cannot be null. </exception>
        /// <returns>A flattened list of elements.</returns>
        [LinqTunnel]
        [NotNull]
        private static IEnumerable<T> WhereRecursive<T>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, IEnumerable<T>> sourceChildSelector, [NotNull] Func<T, UInt32, Boolean> predicate, UInt32 currentDeep)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sourceChildSelector == null)
            {
                throw new ArgumentNullException(nameof(sourceChildSelector));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var item in source)
            {
                if (predicate(item, currentDeep))
                {
                    yield return item;
                }

                foreach (var child in sourceChildSelector(item).WhereRecursive(sourceChildSelector, predicate, currentDeep + 1))
                {
                    if (predicate(child, currentDeep))
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the supplied function against each element in the source list and all children.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result function.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="sourceChildSelector">A <see cref="Func{T, IEnumerable{T}}"/> that selects the children of the current element.</param>
        /// <param name="resultSelector">The selector that selects the new result.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>', '<paramref name="sourceChildSelector"/>' and '<paramref name="resultSelector"/>' cannot be null. </exception>
        /// <returns>The result fo the selector function.</returns>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<TResult> SelectRecursive<T, TResult>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, IEnumerable<T>> sourceChildSelector, [NotNull] Func<T, IEnumerable<TResult>, UInt32, TResult> resultSelector)
            where T : class
        {
            return source.SelectRecursive(sourceChildSelector, resultSelector, 0);
        }

        /// <summary>
        /// Executes the supplied function against each element in the source list and all children.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TResult">The <see cref="Type"/> of the result function.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="sourceChildSelector">A <see cref="Func{T, IEnumerable{T}}"/> that selects the children of the current element.</param>
        /// <param name="resultSelector">The selector that selects the new result.</param>
        /// <param name="currentDeep">The current recursion deep.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>', '<paramref name="sourceChildSelector"/>' and '<paramref name="resultSelector"/>' cannot be null. </exception>
        /// <returns>The result fo the selector function.</returns>
        [LinqTunnel]
        [NotNull]
        private static IEnumerable<TResult> SelectRecursive<T, TResult>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, IEnumerable<T>> sourceChildSelector, [NotNull] Func<T, IEnumerable<TResult>, UInt32, TResult> resultSelector, UInt32 currentDeep)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sourceChildSelector == null)
            {
                throw new ArgumentNullException(nameof(sourceChildSelector));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            foreach (var item in source)
            {
                var childs = sourceChildSelector(item).SelectRecursive(sourceChildSelector, resultSelector, currentDeep + 1);
                yield return resultSelector(item, childs, currentDeep);
            }
        }

        /// <summary>
        /// Converts a flattened list of elements to a tree-like list of parent-child relation.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="isParentCondition">A <see cref="Func{T, Boolean}"/> that identifies a parent element in the source list.</param>
        /// <param name="parentChildCondition">A <see cref="Func{T, T, Boolean}"/> that connects each parent with its children. If the function returns <c>true</c> than the children is a child of the parent; otherwise <c>false</c>.</param>
        /// <returns>A tree-like structure where each children is connected with its parent.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>', '<paramref name="isParentCondition"/>' and '<paramref name="parentChildCondition"/>' cannot be null. </exception>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<TreeNode<T>> Treeify<T>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, Boolean> isParentCondition, [NotNull] Func<T, T, Boolean> parentChildCondition)
            where T : class
        {
            return source.Treeify(isParentCondition, parentChildCondition, null);
        }

        /// <summary>
        /// Converts a flattened list of elements to a tree-like list of parent-child relation.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="isParentCondition">A <see cref="Func{T, Boolean}"/> that identifies a parent element in the source list.</param>
        /// <param name="parentChildCondition">A <see cref="Func{T, T, Boolean}"/> that connects each parent with its children. If the function returns <c>true</c> than the node is a child of the parent; otherwise <c>false</c>.</param>
        /// <param name="parent">The current parent.</param>
        /// <returns>A tree-like structure where each children is connected with its parent.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>', '<paramref name="isParentCondition"/>' and '<paramref name="parentChildCondition"/>' cannot be null. </exception>
        [LinqTunnel]
        [NotNull]
        private static IEnumerable<TreeNode<T>> Treeify<T>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, Boolean> isParentCondition, [NotNull] Func<T, T, Boolean> parentChildCondition, [CanBeNull] TreeNode<T> parent)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (isParentCondition == null)
            {
                throw new ArgumentNullException(nameof(isParentCondition));
            }

            if (parentChildCondition == null)
            {
                throw new ArgumentNullException(nameof(parentChildCondition));
            }

            var whereClause = parent != null ? p => !isParentCondition(p) : isParentCondition;
            var sourceList = source.ToList();
            foreach (var item in sourceList.Where(whereClause))
            {
                var condition = parent == null || parentChildCondition(parent.Node, item);
                if (condition)
                {
                    var newParent = new TreeNode<T>(item);
                    newParent.ChildNodes = sourceList.Treeify(isParentCondition, parentChildCondition, newParent);

                    yield return newParent;
                }
            }
        }
    }
}