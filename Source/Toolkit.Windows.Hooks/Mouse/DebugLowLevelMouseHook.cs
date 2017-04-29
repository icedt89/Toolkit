namespace JanHafner.Toolkit.Windows.Hooks.Mouse
{
    using System.Diagnostics;

    public sealed class DebugLowLevelMouseHook : LowLevelMouseWindowsHook
    {
        protected override void Handle(LowLevelMouseMessageIdentifier mouseMessageIdentifier, MSLLHOOKSTRUCT lowlevelMouseHookStructure)
        {
            Debug.WriteLine($"{mouseMessageIdentifier} {lowlevelMouseHookStructure.MouseData} {lowlevelMouseHookStructure.Flags} {lowlevelMouseHookStructure.Point}");
        }
    }
}