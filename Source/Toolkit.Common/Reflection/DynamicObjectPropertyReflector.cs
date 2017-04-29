namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Reflection;
    using JanHafner.Toolkit.Common.Properties;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="DynamicObjectPropertyReflector"/> class is specialized in processing dynamic objects instances.
    /// </summary>
    public sealed class DynamicObjectPropertyReflector : IPropertyReflector
    {
        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> from the supplied instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>An <see cref="IEnumerable{PropertyInfo}"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instance"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The argument '<paramref name="instance"/>' is not of Type ExpandoObject.</exception>
        [LinqTunnel]
        public IEnumerable<PropertyInfo> ReflectProperties(Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var dynamicInstance = instance as DynamicObject;
            if (dynamicInstance == null)
            {
                throw new ArgumentException(String.Format(ExceptionMessages.ArgumentIsNotDynamicObjectExceptionMessage, nameof(instance)));
            }
            
            foreach (var dynamicMemberName in dynamicInstance.GetDynamicMemberNames())
            {
                Object value;
                if (dynamicInstance.TryGetMember(new DynamicObjectGetMemberBinder(dynamicMemberName), out value))
                {
                    yield return new DynamicObjectPropertyInfo(dynamicMemberName, value.GetType(), instance.GetType());
                }

                yield return new DynamicObjectPropertyInfo(dynamicMemberName, null, instance.GetType());
            }
        }

        /// <summary>
        /// Returns a vlaue if the <see cref="IPropertyReflector"/> can reflect <see cref="PropertyInfo"/>s of the supplied <see cref="Object"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A value indicating, whether, <see cref="PropertyInfo"/>s can be reflected.</returns>
        public Boolean CanReflectProperties(Object instance)
        {
            return instance is DynamicObject;
        }
    }
}