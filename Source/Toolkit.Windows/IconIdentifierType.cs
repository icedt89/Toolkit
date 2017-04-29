namespace JanHafner.Toolkit.Windows
{
    /// <summary>
    /// Defines the type of the identifier which is used as lookup in the file.
    /// </summary>
    public enum IconIdentifierType
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Unknown,

        /// <summary>
        /// The number should be treated as icon index inside the file.
        /// </summary>
        Index,

        /// <summary>
        /// The number should be treated as resource identifier inside the file.
        /// </summary>
        ResourceId
    }
}