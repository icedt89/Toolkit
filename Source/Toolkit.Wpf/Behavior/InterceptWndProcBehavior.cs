namespace JanHafner.Toolkit.Wpf.Behavior
{
    using System;
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Interop;
    using JetBrains.Annotations;

    /// <summary>
    /// Base class for behaviors which intercept the WndProc of a WPF-<see cref="Window"/>.
    /// </summary>
    public abstract class InterceptWndProcBehavior : Behavior<Window>
    {
        private HwndSource hwndSource;

        #region Enabled

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
            "Enabled", typeof(Boolean), typeof(InterceptWndProcBehavior), new PropertyMetadata(default(Boolean), EnabledChangedCallback));

        private static void EnabledChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = (InterceptWndProcBehavior)dependencyObject;
            behavior.EnabledChanged((Boolean)dependencyPropertyChangedEventArgs.OldValue, (Boolean)dependencyPropertyChangedEventArgs.NewValue);
        }

        public Boolean Enabled
        {
            get { return (Boolean)this.GetValue(EnabledProperty); }
            set { this.SetValue(EnabledProperty, value); }
        }

        #endregion

        protected override void OnAttached()
        {
            this.AssociatedObject.Loaded += this.AssociatedObjectOnLoaded;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= this.AssociatedObjectOnLoaded;
            this.hwndSource?.RemoveHook(this.InterceptWndProc);
        }

        protected virtual void AssociatedObjectOnLoaded([CanBeNull] Object sender, [CanBeNull] RoutedEventArgs routedEventArgs)
        {
            if (this.hwndSource == null)
            {
                var windowInteropHelper = new WindowInteropHelper(this.AssociatedObject);
                this.hwndSource = HwndSource.FromHwnd(windowInteropHelper.Handle);
                if (this.hwndSource == null)
                {
                    return;
                }
            }

            this.hwndSource.AddHook(this.InterceptWndProc);
        }

        protected virtual void EnabledChanged(Boolean oldValue, Boolean newValue)
        {
        }

        protected abstract IntPtr InterceptWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled);
    }
}