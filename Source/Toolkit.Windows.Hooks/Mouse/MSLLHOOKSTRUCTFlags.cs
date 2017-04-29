namespace JanHafner.Toolkit.Windows.Hooks.Mouse
{
    using System;

    [Flags]
    public enum MSLLHOOKSTRUCTFlags
    {
        /// <summary>
        /// Test the event-injected (from any process) flag.
        /// </summary>
        LLMHF_INJECTED = 1,

        /// <summary>
        /// Test the event-injected (from a process running at lower integrity level) flag.
        /// </summary>
        LLMHF_LOWER_IL_INJECTED = 2
    }
}