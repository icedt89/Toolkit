using System.Windows.Forms;

namespace Toolkit.TestWindow
{
    using System.ComponentModel;
    using JanHafner.Toolkit.Windows.Hooks;
    using JanHafner.Toolkit.Windows.Hooks.Keyboard;

    public partial class Form1 : Form
    {
        private readonly WindowsHook hook;

        public Form1()
        {
            this.InitializeComponent();

            this.hook = new DebugLowLevelKeyboardHook();
            this.hook.Install();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.hook.Dispose();

            base.OnClosing(e);
        }
    }
}
