namespace JanHafner.Toolkit.Common
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="TreeNode{T}"/> class provides a common layout for tree-like structures.
    /// </summary>
    public sealed class TreeNode<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="node"/>' cannot be null. </exception>
        public TreeNode([NotNull] T node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));    
            }

            this.Node = node;
        }

        /// <summary>
        /// Gets the <see cref="T"/> with is contained in this <see cref="TreeNode{T}"/>.
        /// </summary>
        [NotNull]
        public T Node { get; private set; }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> which contains the child nodes.
        /// </summary>
        [CanBeNull]
        public IEnumerable<TreeNode<T>> ChildNodes { get; set; }
    }
}