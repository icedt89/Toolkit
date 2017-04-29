namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="IPropertyReflectorSelector"/> interface provides methods for creating <see cref="IPropertyReflector"/> instances.
    /// </summary>
    public interface IPropertyReflectorSelector
    {
        /// <summary>
        /// Gets an instance of an <see cref="IPropertyReflector"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>An <see cref="IPropertyReflector"/>.</returns>
        [NotNull]
        IPropertyReflector GetPropertyReflector([NotNull] Object instance);
    }
}