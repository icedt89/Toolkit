namespace JanHafner.Toolkit.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using JetBrains.Annotations;

    /// <summary>
    /// This exception is thrown when the <see cref="LambdaExpressionDeserializer"/> has constructed an <see cref="Expression"/> with zero parts.
    /// </summary>
    [Serializable]
    public sealed class ConstructedPropertyPathDoesNotContainAnyPartsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedPropertyPathDoesNotContainAnyPartsException"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="sourceType"/>' and '<paramref name="propertyPath"/>' cannot be null. </exception>
        public ConstructedPropertyPathDoesNotContainAnyPartsException([NotNull] Type sourceType, [NotNull] String propertyPath)
            : base($"Thee supplied property path '{sourceType.Name}' does not contain any constructible parts for Type '{propertyPath}'.")
        {
            if (String.IsNullOrEmpty(propertyPath))
            {
                throw new ArgumentNullException(nameof(propertyPath));
            }

            this.SourceType = sourceType;
            this.PropertyPath = propertyPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructedPropertyPathDoesNotContainAnyPartsException"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null. </exception>
        /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0). </exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private ConstructedPropertyPathDoesNotContainAnyPartsException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the type on which the property path should be created.
        /// </summary>
        [NotNull]
        public Type SourceType { get; set; }

        /// <summary>
        /// Gets the property path.
        /// </summary>
        [NotNull]
        public String PropertyPath { get; set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("SourceType", this.SourceType.FullName);
            info.AddValue("PropertyPath", this.PropertyPath);

            base.GetObjectData(info, context);
        }
    }
}