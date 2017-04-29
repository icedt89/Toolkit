namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Dynamic;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="DynamicObjectGetMemberBinder"/> class.
    /// </summary>
    internal sealed class DynamicObjectGetMemberBinder : GetMemberBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicObjectGetMemberBinder"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DynamicObjectGetMemberBinder([NotNull] String name)
            : base(name, false)
        {
        }

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Method is not supported on this <see cref="Type"/>.</exception>
        public override DynamicMetaObject FallbackGetMember([CanBeNull] DynamicMetaObject target, [CanBeNull] DynamicMetaObject errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}