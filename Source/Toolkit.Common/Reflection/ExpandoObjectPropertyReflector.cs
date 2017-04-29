namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Reflection;
    using JanHafner.Toolkit.Common.Properties;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="ExpandoObjectPropertyReflector"/> class is specialized in processing <see cref="ExpandoObject"/> instances.
    /// </summary>
    public sealed class ExpandoObjectPropertyReflector : IPropertyReflector
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

            if (!(instance is ExpandoObject))
            {
                throw new ArgumentException(String.Format(ExceptionMessages.ArgumentIsNotExpandoObjectExceptionMessage, nameof(instance)));
            }

            var expandoInstance = (IDictionary<String, Object>) instance;
            foreach (var expandoProperty in expandoInstance)
            {
                if (expandoProperty.Value != null)
                {
                    yield return new ExpandoObjectPropertyInfo(expandoProperty.Key, expandoProperty.Value.GetType(), instance.GetType());
                }

                yield return new ExpandoObjectPropertyInfo(expandoProperty.Key, null, instance.GetType());
            }
        }

        /// <summary>
        /// Returns a vlaue if the <see cref="IPropertyReflector"/> can reflect <see cref="PropertyInfo"/>s of the supplied <see cref="Object"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A value indicating, whether, <see cref="PropertyInfo"/>s can be reflected.</returns>
        public Boolean CanReflectProperties(Object instance)
        {
            return instance is ExpandoObject;
        }
    }
}