namespace Omnipaste
{
    using System.Diagnostics;
    using System.Windows.Forms;

    public partial class TokenInputForm : Form, ITokenInputForm
    {
        public string Token { get; private set; }

        void ITokenInputForm.ShowDialog()
        {
            ShowDialog();
        }

        public TokenInputForm()
        {
            InitializeComponent();
        }

        private void TokenLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(ApplicationInfoFactory.TokenLink);
        }

        private void OkButtonClick(object sender, System.EventArgs e)
        {
            Token = tokenTextBox.Text;
            Close();
        }
    }
}
