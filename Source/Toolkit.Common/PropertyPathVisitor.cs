namespace JanHafner.Toolkit.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="PropertyPathVisitor"/> class provides methods which converts a <see cref="LambdaExpression"/> to a property path.
    /// </summary>
    public sealed class PropertyPathVisitor : ExpressionVisitor
    {
        [NotNull]
        private readonly Stack<String> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPathVisitor"/> class.
        /// </summary>
        public PropertyPathVisitor()
        {
            this.stack = new Stack<String>();
        }

        /// <summary>
        /// Gets the property path. Uses <see cref="Type.Delimiter"/> to concat the parts of the path.
        /// </summary>
        /// <param name="expression">The <see cref="LambdaExpression"/>.</param>
        /// <returns>A <see cref="String"/> representing the property path.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="expression"/>' cannot be null.</exception>
        [NotNull]
        public String GetPropertyPath([NotNull] LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            this.Visit(expression);
            var result = String.Join(Type.Delimiter.ToString(), this.stack);
            this.stack.Clear();
            return result;
        }


        /// <summary>
        /// Called during the visit of a <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="expression">The visited <see cref="MemberExpression"/>.</param>
        /// <returns>The visited <see cref="Expression"/>.</returns>
        /// <exception cref="ArgumentException">The evaluated <see cref="MemberExpression"/> is no <see cref="PropertyInfo"/>.</exception>
        protected override Expression VisitMember(MemberExpression expression)
        {
            if (!(expression.Member is PropertyInfo))
            {
                throw new ArgumentException("The MemberInfo can not be converted to a PropertyInfo.");
            }

            this.stack.Push(expression.Member.Name);
            return base.VisitMember(expression);
        }
    }
}