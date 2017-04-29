namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="PropertyReflectorSelector"/> class.
    /// </summary>
    public sealed class PropertyReflectorSelector : IPropertyReflectorSelector
    {
        [NotNull]
        private readonly IEnumerable<IPropertyReflector> propertyReflectors;

        public PropertyReflectorSelector([NotNull] IEnumerable<IPropertyReflector> propertyReflectors)
        {
            if (propertyReflectors == null)
            {
                throw new ArgumentNullException(nameof(propertyReflectors));
            }

            this.propertyReflectors = propertyReflectors;
        }

        /// <summary>
        /// Gets an instance of an <see cref="IPropertyReflector"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>An <see cref="IPropertyReflector"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instance"/>' cannot be null. </exception>
        public IPropertyReflector GetPropertyReflector(Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));    
            }

            return this.propertyReflectors.First(pr => pr.CanReflectProperties(instance));
        }
    }
}