using System.Windows.Forms;
using ClipboardWrapper;
using Ninject;

namespace ClipboardWatcher
{
    public partial class MainForm : Form
    {
        [Inject]
        public IClipboardManager ClipboardManager { get; set; }

        public bool IsNotificationIconVisible
        {
            get { return NotifyIcon.Visible; }
            set { NotifyIcon.Visible = value; }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {

        }

        protected override void OnActivated(System.EventArgs e)
        {
            IsNotificationIconVisible = true;
        }
    }
}
