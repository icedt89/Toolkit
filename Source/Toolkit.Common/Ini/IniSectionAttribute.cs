namespace JanHafner.Toolkit.Common.Ini
{
    using System;
    using JetBrains.Annotations;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class IniSectionAttribute : Attribute
    {
        public IniSectionAttribute([NotNull] String section)
        {
            if (String.IsNullOrWhiteSpace(section))
            {
                throw new ArgumentNullException(nameof(section));
            }

            this.Section = section;
        }

        [NotNull]
        public String Section { get; private set; }
    }
}