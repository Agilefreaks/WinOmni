using System.Windows.Forms;

namespace ClipboardWatcher
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnActivated(System.EventArgs e)
        {
            NotifyIcon.Visible = true;
        }
    }
}
