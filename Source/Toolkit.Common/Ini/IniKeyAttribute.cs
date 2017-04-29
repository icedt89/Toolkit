namespace JanHafner.Toolkit.Common.Ini
{
    using System;
    using JetBrains.Annotations;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IniKeyAttribute : Attribute
    {
        public IniKeyAttribute([NotNull] String key)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            this.Key = key;
        }

        [NotNull]
        public String Key { get; private set; }
    }
}