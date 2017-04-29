namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using JanHafner.Toolkit.Common.Properties;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="ExpandoObjectPropertyInfo"/> class provides GetValue and SetValue implementations for <see cref="ExpandoObject"/>.
    /// </summary>
    internal sealed class ExpandoObjectPropertyInfo : DynamicPropertyInfoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandoObjectPropertyInfo"/> class.
        /// </summary>
        /// <param name="dynamicPropertyName">Name of the dynamic property.</param>
        /// <param name="dynamicPropertyType">Type of the dynamic property.</param>
        /// <param name="dynamicDeclaringType">Type of the dynamic declaring.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="dynamicPropertyName" />' and '<paramref name="dynamicDeclaringType" />' cannot be null. </exception>
        public ExpandoObjectPropertyInfo([NotNull] String dynamicPropertyName, Type dynamicPropertyType, [NotNull] Type dynamicDeclaringType)
            : base(dynamicPropertyName, dynamicPropertyType, dynamicDeclaringType)
        {
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The value of '<paramref name="obj"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The parameter '<paramref name="obj"/>' is not of Type ExpandoObject.</exception>
        [CanBeNull]
        public override Object GetValue([NotNull] Object obj, [CanBeNull] Object[] index)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!(obj is ExpandoObject))
            {
                throw new ArgumentException(String.Format(ExceptionMessages.ArgumentIsNotExpandoObjectExceptionMessage, nameof(obj)));
            }

            var expandoDictionary = (IDictionary<String, Object>) obj;
            Object value;
            expandoDictionary.TryGetValue(this.DynamicPropertyName, out value);
            return value;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The value of '<paramref name="obj"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The parameter '<paramref name="obj"/>' is not of Type ExpandoObject.</exception>
        public override void SetValue([NotNull] Object obj, [CanBeNull] Object value, [CanBeNull] Object[] index)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!(obj is ExpandoObject))
            {
                throw new ArgumentException(String.Format(ExceptionMessages.ArgumentIsNotExpandoObjectExceptionMessage, nameof(obj)));
            }

            var expandoDictionary = (IDictionary<String, Object>) obj;
            if (expandoDictionary.ContainsKey(this.DynamicPropertyName))
            {
                expandoDictionary[this.DynamicPropertyName] = value;
            }
            else
            {
                expandoDictionary.Add(this.DynamicPropertyName, value);
            }
        }
    }
}