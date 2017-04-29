namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Dynamic;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="DynamicObjectSetMemberBinder"/> class.
    /// </summary>
    internal sealed class DynamicObjectSetMemberBinder : SetMemberBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicObjectSetMemberBinder"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DynamicObjectSetMemberBinder([NotNull] String name)
            : base(name, false)
        {
        }

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Method is not supported on this <see cref="Type"/>.</exception>
        public override DynamicMetaObject FallbackSetMember([CanBeNull]DynamicMetaObject target, [CanBeNull] DynamicMetaObject value, [CanBeNull] DynamicMetaObject errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}