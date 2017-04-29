namespace JanHafner.Toolkit.Wpf.Behavior
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Inhibits movement of the associated <see cref="Window"/>.
    /// </summary>
    public sealed class PinnedWindowBehavior : InterceptMoveWindowMessageBehavior
    {
        private RECT pinnedPosition;

        public PinnedWindowBehavior()
        {
            this.ModifierKeys = ModifierKeys.Control;
        }

        protected override void EnabledChanged(Boolean oldValue, Boolean newValue)
        {
            if (newValue && this.AssociatedObject != null)
            {
                this.UpdateApplicationRectangle();
            }
        }

        protected override Boolean ManipulatePosition(ref RECT rect)
        {
            if (this.TemporaryDisableBehaviorOnModifierKeys && Keyboard.Modifiers.HasFlag(this.ModifierKeys))
            {
                this.UpdateApplicationRectangle();

                return false;
            }

            rect = this.pinnedPosition;

            return true;
        }

        private void UpdateApplicationRectangle()
        {
            this.pinnedPosition = new RECT
            {
                top = (Int32)this.AssociatedObject.Top,
                left = (Int32)this.AssociatedObject.Left
            };
        }
    }
}