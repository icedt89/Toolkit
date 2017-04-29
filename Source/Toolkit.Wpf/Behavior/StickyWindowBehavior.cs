namespace JanHafner.Toolkit.Wpf.Behavior
{
    using System;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Point = System.Drawing.Point;

    /// <summary>
    /// Sticks the associated <see cref="Window"/> to the borders of the screen which contains the upper-left corner of the <see cref="Window"/>.
    /// </summary>
    public sealed class StickyWindowBehavior : InterceptMoveWindowMessageBehavior
    {
        #region SnapOnOffset

        public static readonly DependencyProperty SnapOnOffsetProperty = DependencyProperty.Register(
            "SnapOnOffset", typeof(Int32), typeof(StickyWindowBehavior), new PropertyMetadata(10));

        public Int32 SnapOnOffset
        {
            get { return (Int32)this.GetValue(SnapOnOffsetProperty); }
            set { this.SetValue(SnapOnOffsetProperty, value); }
        }

        #endregion

        public StickyWindowBehavior()
        {
            this.ModifierKeys = ModifierKeys.Control | ModifierKeys.Alt;
        }

        protected override Boolean ManipulatePosition(ref RECT rect)
        {
            if (this.TemporaryDisableBehaviorOnModifierKeys && Keyboard.Modifiers.HasFlag(this.ModifierKeys))
            {
                return false;
            }

            if (rect.left <= this.SnapOnOffset)
            {
                rect.left = 0;
            }

            if (rect.top <= this.SnapOnOffset)
            {
                rect.top = 0;
            }

            var currentScreen = Screen.FromPoint(new Point(rect.left, rect.top));

            var currentScreenWidth = currentScreen.WorkingArea.Width;
            var currentScreenHeight = currentScreen.WorkingArea.Height;

            var window = (Int32)this.AssociatedObject.Width;
            if (rect.left + window >= currentScreenWidth - this.SnapOnOffset)
            {
                rect.left = currentScreenWidth - window;
            }

            window = (Int32)this.AssociatedObject.Height;
            if (rect.top + window >= currentScreenHeight - this.SnapOnOffset)
            {
                rect.top = currentScreenHeight - window;
            }

            return true;
        }
    }
}
