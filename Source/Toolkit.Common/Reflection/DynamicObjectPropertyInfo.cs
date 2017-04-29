namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Dynamic;
    using JanHafner.Toolkit.Common.Properties;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="DynamicObjectPropertyInfo"/> class provides GetValue and SetValue implementations for dynamic objects.
    /// </summary>
    internal sealed class DynamicObjectPropertyInfo : DynamicPropertyInfoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicObjectPropertyInfo"/> class.
        /// </summary>
        /// <param name="dynamicPropertyName">Name of the dynamic property.</param>
        /// <param name="dynamicPropertyType">Type of the dynamic property.</param>
        /// <param name="dynamicDeclaringType">Type of the dynamic declaring.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="dynamicPropertyName" />' and '<paramref name="dynamicDeclaringType" />' cannot be null. </exception>
        public DynamicObjectPropertyInfo([NotNull] String dynamicPropertyName, Type dynamicPropertyType, [NotNull] Type dynamicDeclaringType)
            : base(dynamicPropertyName, dynamicPropertyType, dynamicDeclaringType)
        {
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The value of '<paramref name="obj"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The argument '<paramref name="obj"/>' is not of Type DynamicObject.</exception>
        [CanBeNull]
        public override Object GetValue([NotNull] Object obj, [CanBeNull] Object[] index)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var dynamicObject = obj as DynamicObject;
            if (dynamicObject == null)
            {
                throw new ArgumentException(String.Format(ExceptionMessages.ArgumentIsNotDynamicObjectExceptionMessage, nameof(obj)));
            }

            Object value;
            dynamicObject.TryGetMember(new DynamicObjectGetMemberBinder(this.DynamicPropertyName), out value);
            return value;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The value of '<paramref name="obj"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The argument '<paramref name="obj"/>' is not of Type DynamicObject.</exception>
        public override void SetValue([NotNull] Object obj, [CanBeNull] Object value, [CanBeNull] Object[] index)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var dynamicObject = obj as DynamicObject;
            if (dynamicObject == null)
            {
                throw new ArgumentException(String.Format(ExceptionMessages.ArgumentIsNotDynamicObjectExceptionMessage, nameof(obj)));
            }

            dynamicObject.TrySetMember(new DynamicObjectSetMemberBinder(this.DynamicPropertyName), value);
        }
    }
}