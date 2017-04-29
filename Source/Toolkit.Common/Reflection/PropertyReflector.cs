namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="PropertyReflector"/> class.
    /// Default implementation uses BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic.
    /// </summary>
    public class PropertyReflector : IPropertyReflector
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> PropertyInfocache = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// Gets an <see cref="IEnumerable{PropertyInfo}"/> from the supplied instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>An <see cref="IEnumerable{PropertyInfo}"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instance" />' cannot be null. </exception>
        [LinqTunnel]
        public IEnumerable<PropertyInfo> ReflectProperties(Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var instanceType = instance as Type ??  instance.GetType();

            return PropertyInfocache.GetOrAdd(instanceType, _ => this.ReflectPropertiesCore(instanceType));
        }

        [LinqTunnel]
        [NotNull]
        protected virtual IEnumerable<PropertyInfo> ReflectPropertiesCore([NotNull] Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        } 

        /// <summary>
        /// Returns a vlaue if the <see cref="IPropertyReflector"/> can reflect <see cref="PropertyInfo"/>s of the supplied <see cref="Object"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A value indicating, whether, <see cref="PropertyInfo"/>s can be reflected.</returns>
        public virtual Boolean CanReflectProperties(Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return instance is Type || (!(instance is DynamicObject) && !(instance is ExpandoObject));
        }
    }
}