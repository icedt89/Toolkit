namespace JanHafner.Toolkit.Common.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// The <see cref="ValueTypeExtensions"/> class contains extension methods for <see cref="ValueType"/>`s.
    /// </summary>
    public static class ValueTypeExtensions
    {
        /// <summary>
        /// The <see cref="DateTime"/> where the "Unixtime" begins, better known as the 01.01.1970 00:00:00.000.
        /// </summary>
        public static readonly DateTime UnixTimestampOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Prepends a timestamp in the current locale to the message provided.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The messsage with prepended timestamp.</returns>
        public static String PrependTimestamp(this String message)
        {
            return $"[{DateTime.Now}]: {message}";
        }

        /// <summary>
        /// Gets a list of <see cref="Enum"/>-values from a <see cref="FlagsAttribute"/> <see cref="Enum"/>-value.
        /// </summary>
        /// <param name="enumeration">The enum.</param>
        /// <returns>The list of all applied flags.</returns>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<T> GetEnumValuesFromFlags<T>(this Enum enumeration)
        {
            return Enum.GetValues(enumeration.GetType()).Cast<Enum>().Where(enumeration.HasFlag).Cast<T>();
        }

        /// <summary>
        /// Gets a list of <see cref="Enum"/>-values from a <see cref="FlagsAttribute"/> <see cref="Enum"/>-value.
        /// </summary>
        /// <param name="enumeration">The enum.</param>
        /// <returns>The list of all applied flags.</returns>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<object> GetEnumValuesFromFlags(this Enum enumeration)
        {
            return enumeration.GetEnumValuesFromFlags<Object>();
        }

        /// <summary>
        /// Gets the <see cref="Attribute"/> from <see cref="Enum"/>-Member.
        /// </summary>
        /// <typeparam name="TAttribute">The <see cref="Type"/> of the <see cref="Attribute"/>.</typeparam>
        /// <param name="enumeration">The <see cref="Enum"/>.</param>
        /// <returns>The instance of the <see cref="Attribute"/> as <see cref="TAttribute"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="enumeration"/>' cannot be null.</exception>
        /// <exception cref="MissingMemberException">There is no enum-member whith the specified name.</exception>
        [CanBeNull]
        public static TAttribute GetAttributeFromEnumMember<TAttribute>([NotNull] this Enum enumeration)
            where TAttribute : Attribute
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException(nameof(enumeration));
            }

            var enumType = enumeration.GetType();
            var matches = enumType.GetMember(enumeration.ToString());
            if (matches.Length == 0)
            {
                throw new MissingMemberException($"The Enum '{enumType.Name}' has no member with the specified value of '{enumeration}'.");
            }

            return matches[0].GetCustomAttribute<TAttribute>();
        }

            /// <summary>
        /// Converts the <see cref="DateTime"/> to Unixtime.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to convert to Unixtime.</param>
        /// <returns>An <see cref="Int64"/> representing Unixtime.</returns>
        public static Int64 ToUnixTimestamp(this DateTime dateTime)
        {
            var difference = dateTime - UnixTimestampOrigin;
            return (Int64) Math.Floor(difference.TotalSeconds);
        }

        /// <summary>
        /// Converts the <see cref="Int64"/> to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="unixTimestamp">The <see cref="Int64"/> to convert.</param>
        /// <returns>A <see cref="DateTime"/> converted from Unixtime.</returns>
        public static DateTime FromUnixTimestamp(this Int64 unixTimestamp)
        {
            return UnixTimestampOrigin.AddSeconds(unixTimestamp);
        }
    }
}