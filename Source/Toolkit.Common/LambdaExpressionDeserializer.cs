namespace JanHafner.Toolkit.Common
{
    using System;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines methods for serializing a property path back into a <see cref="LambdaExpression"/>.
    /// </summary>
    public static class LambdaExpressionDeserializer
    {
        /// <summary>
        /// Deserializes a propery path back into a <see cref="LambdaExpression"/> with the provided <see cref="Type"/> as source.
        /// Parts are splitted on the <see cref="String"/> represented by <see cref="Type.Delimiter"/>.
        /// </summary>
        /// <param name="sourceType">The <see cref="Type"/> of the source.</param>
        /// <param name="propertyPath">The property path to deserialize.</param>
        /// <returns>The deserialized <see cref="LambdaExpression"/>.</returns>
        /// <exception cref="ConstructedPropertyPathDoesNotContainAnyPartsException">Thee supplied property path '<paramref name="propertyPath"/>' does not contain any constructible parts for Type '<paramref name="sourceType"/>'. </exception>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="sourceType"/>' and '<paramref name="propertyPath"/>' cannot be null. </exception>
        public static LambdaExpression Deserialize([NotNull] Type sourceType, [NotNull] String propertyPath)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }
            if (propertyPath == null)
            {
                throw new ArgumentNullException(nameof(propertyPath));
            }

            var lambdaParameter = Expression.Parameter(sourceType, "source");
            Expression expressionBody = null;
            foreach (var propertyPathPart in propertyPath.Split(Type.Delimiter))
            {
                expressionBody = Expression.Property(expressionBody ?? lambdaParameter, propertyPathPart);
            }

            if (expressionBody == null)
            {
                throw new ConstructedPropertyPathDoesNotContainAnyPartsException(sourceType, propertyPath);
            }

            return Expression.Lambda(expressionBody, lambdaParameter);
        }
    }
}