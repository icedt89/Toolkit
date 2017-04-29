namespace JanHafner.Toolkit.Windows
{
    using System;

    public static class IconIdentifier
    {
        public static IconIdentifierType Identify(Int32 iconIdentifier)
        {
            return iconIdentifier < -1 
                ? IconIdentifierType.ResourceId 
                : IconIdentifierType.Index;
        }
    }
}
