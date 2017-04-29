namespace JanHafner.Toolkit.Common.ExtensionMethods
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using JetBrains.Annotations;
    using Properties;

    /// <summary>
    /// The <see cref="CommonExtensions"/> class provides common extensions.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Gets the default value of the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>The default value of the supplied <see cref="Type"/>.</returns>
        [CanBeNull]
        public static Object GetDefault([NotNull] this Type type)
        {
            return type.IsClass ? null : Activator.CreateInstance(type);
        }

        /// <summary>
        /// Converts the supplied selector function to a property path (eg. Root.Class1.Property).
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the source.</typeparam>
        /// <typeparam name="TProperty">The <see cref="Type"/> of the property.</typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns>A <see cref="String"/> in  the form {p1}.{p2}.{pN} representing the property path.</returns>
        [NotNull]
        public static String GetPropertyPath<T, TProperty>([NotNull] this Expression<Func<T, TProperty>> propertySelector)
        {
            return new PropertyPathVisitor().GetPropertyPath(propertySelector);
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> of the last node in the selector function.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the source.</typeparam>
        /// <typeparam name="TProperty">The <see cref="Type"/> of the property.</typeparam>
        /// <param name="propertySelector">An <see cref="Expression{Func{T, TProperty}}"/> which selects the property.</param>
        /// <returns>The <see cref="PropertyInfo"/> of the last property in the <see cref="Expression{Func{T, TProperty}}"/>.</returns>
        [NotNull]
        public static PropertyInfo GetPropertyInfo<T, TProperty>([NotNull] this Expression<Func<T, TProperty>> propertySelector)
        {
            return ((LambdaExpression) propertySelector).GetPropertyInfo();
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> from the supplied <see cref="LambdaExpression"/>.
        /// </summary>
        /// <param name="propertySelector">A <see cref="LambdaExpression"/> which represents a selector function.</param>
        /// <returns>The <see cref="PropertyInfo"/> of the last property in the <see cref="LambdaExpression"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertySelector"/>' cannot be null. </exception>
        public static PropertyInfo GetPropertyInfo([NotNull] this LambdaExpression propertySelector)
        {
            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            var unaryExpressionBody = propertySelector.Body as UnaryExpression;
            if (unaryExpressionBody == null)
            {
                return (PropertyInfo)((MemberExpression) propertySelector.Body).Member;
            }

            if (unaryExpressionBody.NodeType == ExpressionType.Convert ||
                unaryExpressionBody.NodeType == ExpressionType.ConvertChecked)
            {
                return (PropertyInfo) ((MemberExpression) ((UnaryExpression) propertySelector.Body).Operand).Member;
            }

            throw new InvalidOperationException("LambdaExpression not supported.");
        }

        /// <summary>
        /// Returns a new instance of the <see cref="DisposableWrapper{T}"/> class for the supplied object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="object">The object to dispose.</param>
        /// <param name="disposeAction">The action that is called if the object does not implement <see cref="IDisposable"/>.</param>
        /// <returns>The <see cref="DisposableWrapper{T}"/> with the dispoing behavior</returns>
        [NotNull]
        public static DisposableWrapper<T> ToDisposable<T>([NotNull] this T @object, [CanBeNull] Action<T> disposeAction)
        {
            return new DisposableWrapper<T>(@object, disposeAction);
        }

        /// <summary>
        /// Returns a new instance of the <see cref="DisposableWrapper{T}"/> class for the supplied object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        [NotNull]
        public static DisposableWrapper<T> ToDisposable<T>([NotNull] this T @object)
        {
            return @object.ToDisposable(null);
        }
        
        /// <summary>
        /// Gets a value indicating if the supplied <see cref="MemberInfo"/> is read-only.
        /// That means: if the <see cref="MemberInfo"/> is a <see cref="FieldInfo"/>, the properties "IsInitOnly" and "IsLiteral" are checked.
        /// If the <see cref="MemberInfo"/> is a <see cref="PropertyInfo"/>, the "CanWrite" property is checked.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>A value indicating if the <see cref="MemberInfo"/> is read-only.</returns>
        /// <exception cref="ArgumentNullException">The value of '<see cref="memberInfo"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The <paramref name="memberInfo"/> parameter is not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        public static Boolean IsReadonly([NotNull] this MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));    
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.IsInitOnly || fieldInfo.IsLiteral;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return !propertyInfo.CanWrite;
            }

            throw new ArgumentException(ExceptionMessages.MemberInfoIsNotFieldInfoOrPropertyInfoExceptionMessage);
        }

        /// <summary>
        /// Tries to call GetValue on a <see cref="MemberInfo"/> by converting it to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> .
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>The value returned from the call to GetValue.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="memberInfo"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The <paramref name="memberInfo"/> parameter is not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        [CanBeNull]
        public static Object GetValue([NotNull] this MemberInfo memberInfo)
        {
            return memberInfo.GetValue(null);
        }

        /// <summary>
        /// Tries to call GetValue on a <see cref="MemberInfo"/> by converting it to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> .
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="memberInfo"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The <paramref name="memberInfo"/> parameter is not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        /// <returns>The value returned from the call to GetValue.</returns>
        [CanBeNull]
        public static T GetValue<T>([NotNull] this MemberInfo memberInfo)
        {
            return (T)memberInfo.GetValue(null);
        }

        /// <summary>
        /// Tries to call GetValue on a <see cref="MemberInfo"/> by converting it to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> .
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <param name="instance">The instance of the <see cref="Object"/> containing the <see cref="MemberInfo"/>.</param>
        /// <returns>The value returned from the call to GetValue.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="memberInfo"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The <paramref name="memberInfo"/> parameter is not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        [CanBeNull]
        public static Object GetValue([NotNull] this MemberInfo memberInfo, [CanBeNull] Object instance)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(instance);
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(instance, null);
            }

            throw new ArgumentException(ExceptionMessages.MemberInfoIsNotFieldInfoOrPropertyInfoExceptionMessage);
        }

        /// <summary>
        /// Tries to call GetValue on a <see cref="MemberInfo"/> by converting it to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> .
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <param name="instance">The instance of the <see cref="Object"/> containing the <see cref="MemberInfo"/>.</param>
        /// <returns>The value returned from the call to GetValue.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="memberInfo"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">The <paramref name="memberInfo"/> parameter is not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        [CanBeNull]
        public static T GetValue<T>([NotNull] this MemberInfo memberInfo, [CanBeNull] Object instance)
        {
            return (T)memberInfo.GetValue(instance);
        }

        /// <summary>
        /// Tries to call SetValue on a <see cref="MemberInfo"/> by converting it to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> .
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <param name="value">The instance of the value to set.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="memberInfo" />' cannot be null. </exception>
        /// <exception cref="ArgumentException">The <paramref name="memberInfo"/> parameter is not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        public static void SetValue([NotNull] this MemberInfo memberInfo, [CanBeNull] Object value)
        {
            memberInfo.SetValue(null, value);
        }

        /// <summary>
        /// Tries to call SetValue on a <see cref="MemberInfo"/> by converting it to a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> .
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <param name="instance">The instance of the <see cref="Object"/> containing the <see cref="MemberInfo"/>.</param>
        /// <param name="value">The instance of the value to set.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="memberInfo" />' cannot be null. </exception>
        /// <exception cref="ArgumentException">The <paramref name="memberInfo"/> parameter is not <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</exception>
        public static void SetValue([NotNull] this MemberInfo memberInfo, [CanBeNull] Object instance, [CanBeNull] Object value)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(instance, value);
                return;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(instance, value, null);
                return;
            }

            throw new ArgumentException(ExceptionMessages.MemberInfoIsNotFieldInfoOrPropertyInfoExceptionMessage);
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> is a <see cref="Nullable{T}"/>, if so the type parameter is returned, otherweise <paramref name="type"/> is returned.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>The generic type parameter or the supplied type.</returns>
        [NotNull]
        public static Type TryUnwrapIfNullableType([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var genericTypeDefinition = type;
            if (genericTypeDefinition.IsConstructedGenericType)
            {
                genericTypeDefinition = type.GetGenericTypeDefinition();
            }

            return genericTypeDefinition != typeof(Nullable<>) 
                ? type 
                : genericTypeDefinition.GetSingleGenericParameterFromGenericTypeDefinition();
        }

        /// <summary>
        /// Gets the single generic type parameter from the supplied <see cref="Type"/> which must represent a generic type definition.
        /// </summary>
        /// <param name="genericTypeDefinition">The generic type definition.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="genericTypeDefinition"/>' cannot be null.</exception>
        /// <exception cref="InvalidOperationException">The supplied <see cref="Type"/> is not a generic type definitio; or the supplied generic type definition contains more than one generic type parameter.</exception>
        /// <returns>The type parameter from the generic type definition.</returns>
        [NotNull]
        private static Type GetSingleGenericParameterFromGenericTypeDefinition([NotNull] this Type genericTypeDefinition)
        {
            if (genericTypeDefinition == null)
            {
                throw new ArgumentNullException(nameof(genericTypeDefinition));
            }

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentException($"The supplied type '{genericTypeDefinition.Name}' is not a generic type.");
            }

            var genericArguments = genericTypeDefinition.GetGenericArguments();
            if (genericArguments.Length > 1)
            {
                throw new ArgumentException(
                    $"The supplied type '{genericTypeDefinition.Name}' provides more than one generic type parameters: '{String.Join(", ", genericArguments.Select(n => n.Name))}'.");
            }

            return genericArguments[0];
        }
    }
}