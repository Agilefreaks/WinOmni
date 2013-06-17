namespace Omnipaste
{
    using System.Diagnostics;
    using System.Windows.Forms;
    using Omnipaste.Services;

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
            Process.Start(OnlineActivationDataProvider.TokenLink);
        }

        private void OkButtonClick(object sender, System.EventArgs e)
        {
            Token = tokenTextBox.Text;
            Close();
        }

        private void CancelButtonClick(object sender, System.EventArgs e)
        {
            Token = null;
            Close();
        }
    }
}
