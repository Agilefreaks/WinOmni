using System;
using System.Windows.Forms;

namespace ClipboardWatcher
{
    public partial class ConfigureForm : Form
    {
        public string Email
        {
            get { return EmailTextBox.Text; }
        }

        public ConfigureForm()
        {
            InitializeComponent();
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
