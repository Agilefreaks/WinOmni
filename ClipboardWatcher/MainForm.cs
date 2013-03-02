using System.Windows.Forms;

namespace ClipboardWatcher
{
    public partial class MainForm : Form
    {
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
