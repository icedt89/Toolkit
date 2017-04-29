namespace JanHafner.Toolkit.Common.Reflection
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="DynamicPropertyInfoBase"/> class provides a base implementation for dynamic created <see cref="PropertyInfo"/> instances.
    /// </summary>
    internal abstract class DynamicPropertyInfoBase : PropertyInfo
    {
        private readonly Type dynamicPropertyType;

        [NotNull]
        protected readonly String DynamicPropertyName;

        [NotNull]
        private readonly Type dynamicDeclaringType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicPropertyInfoBase"/> class.
        /// </summary>
        /// <param name="dynamicPropertyName">Name of the dynamic property.</param>
        /// <param name="dynamicPropertyType"><see cref="Type"/> of the dynamic property.</param>
        /// <param name="dynamicDeclaringType"><see cref="Type"/> of the dynamic declaring <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="dynamicPropertyName"/>' and '<paramref name="dynamicDeclaringType"/>' cannot be null. </exception>
        protected DynamicPropertyInfoBase([NotNull] String dynamicPropertyName, Type dynamicPropertyType, [NotNull] Type dynamicDeclaringType)
        {
            if (dynamicPropertyName == null)
            {
                throw new ArgumentNullException(nameof(dynamicPropertyName));
            }

            if (dynamicDeclaringType == null)
            {
                throw new ArgumentNullException(nameof(dynamicDeclaringType));
            }

            this.dynamicDeclaringType = dynamicDeclaringType;
            this.DynamicPropertyName = dynamicPropertyName;
            this.dynamicPropertyType = dynamicPropertyType;
        }

        /// <inheritdoc />
        public override String Name
        {
            get { return this.DynamicPropertyName; }
        }

        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return this.dynamicDeclaringType; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return this.DeclaringType; }
        }

        /// <inheritdoc />
        public override Type PropertyType
        {
            get { return this.dynamicPropertyType; }
        }

        /// <inheritdoc />
        public override PropertyAttributes Attributes
        {
            get { return PropertyAttributes.None; }
        }

        /// <inheritdoc />
        public override Boolean CanRead
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override Boolean CanWrite
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override Object[] GetCustomAttributes(Boolean inherit)
        {
            return new Object[0];
        }

        /// <inheritdoc />
        public override Boolean IsDefined(Type attributeType, Boolean inherit)
        {
            return false;
        }

        /// <inheritdoc />
        public abstract override Object GetValue(Object obj, Object[] index);

        /// <inheritdoc />
        public abstract override void SetValue(Object obj, Object value, Object[] index);

        /// <inheritdoc />
        public override void SetValue(Object obj, Object value, BindingFlags invokeAttr, Binder binder, Object[] index, CultureInfo culture)
        {
            this.SetValue(obj, value, null);
        }

        /// <inheritdoc />
        public override Object GetValue(Object obj, BindingFlags invokeAttr, Binder binder, Object[] index, CultureInfo culture)
        {
            return this.GetValue(obj, null);
        }

        /// <inheritdoc />
        public override MethodInfo[] GetAccessors(Boolean nonPublic)
        {
            return new MethodInfo[0];
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Method is not supported on this <see cref="Type"/>.</exception>
        public override MethodInfo GetGetMethod(Boolean nonPublic)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Object[] GetCustomAttributes(Type attributeType, Boolean inherit)
        {
            return new Object[0];
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Method is not supported on this <see cref="Type"/>.</exception>
        public override MethodInfo GetSetMethod(Boolean nonPublic)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override ParameterInfo[] GetIndexParameters()
        {
            return new ParameterInfo[0];
        }
    }
}