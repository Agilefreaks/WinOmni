namespace Omnipaste
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    public partial class ConfigureForm : Form, IConfigureDialog
    {
        public const int MaxRetryCount = 10;

        public bool Succeeded { get; private set; }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IActivationDataProvider ActivationDataProvider { get; set; }

        [Inject]
        public ITokenInputForm TokenInputForm { get; set; }

        [Inject]
        public IApplicationDeploymentInfoProvider ApplicationDeploymentInfoProvider { get; set; }

        private int _retryCount;

        public ConfigureForm()
        {
            InitializeComponent();
        }

        void IConfigureDialog.ShowDialog()
        {
            Succeeded = false;
            ShowDialog();
        }

        public void AssureClipboardIsInitialized()
        {
            var activationToken = GetActivationToken();
            if (string.IsNullOrEmpty(activationToken)) return;

            var activationData = ActivationDataProvider.GetActivationData(activationToken);
            if (activationData.Email.IsNullOrWhiteSpace())
            {
                if (ShouldRetryActivation())
                {
                    _retryCount++;
                    AssureClipboardIsInitialized();
                }
            }
            else
            {
                Succeeded = true;
                ConfigurationService.UpdateCommunicationChannel(activationData.Email);
                backgroundWorker.CancelAsync();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _retryCount = 0;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            AssureClipboardIsInitialized();
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }

            Invoke((Action)Close);
        }

        private bool ShouldRetryActivation()
        {
            return _retryCount < MaxRetryCount && !backgroundWorker.CancellationPending;
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }

            Close();
        }

        private string GetActivationToken()
        {
            return ApplicationDeploymentInfoProvider.HasValidActivationUri
                       ? GetActivationTokenFromDeploymentParameters()
                       : (InvokeRequired
                              ? Invoke((Func<string>)GetActivationTokenFromUser) as string
                              : GetActivationTokenFromUser());
        }

        private string GetActivationTokenFromUser()
        {
            Visible = false;
            TokenInputForm.ShowDialog();
            var token = TokenInputForm.Token;
            Visible = true;

            return token;
        }

        private string GetActivationTokenFromDeploymentParameters()
        {
            var deploymentParameters = new NameValueCollection();
            if (ApplicationDeploymentInfoProvider.HasValidActivationUri)
            {
                deploymentParameters = ApplicationDeploymentInfoProvider.ActivationUri.GetQueryStringParameters();
            }

            return deploymentParameters["token"];
        }
    }
}