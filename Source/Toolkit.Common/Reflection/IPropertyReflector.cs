namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="IPropertyReflector"/> interface provides methods for reflecting the properties of the supplied <see cref="object"/>.
    /// </summary>
    public interface IPropertyReflector
    {
        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> from the supplied instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>An <see cref="IEnumerable{PropertyInfo}"/>.</returns>
        [NotNull]
        [LinqTunnel]
        IEnumerable<PropertyInfo> ReflectProperties([NotNull] Object instance);

        /// <summary>
        /// Returns a vlaue if the <see cref="IPropertyReflector"/> can reflect <see cref="PropertyInfo"/>s of the supplied <see cref="Object"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A value indicating, whether, <see cref="PropertyInfo"/>s can be reflected.</returns>
        Boolean CanReflectProperties([NotNull] Object instance);
    }
}