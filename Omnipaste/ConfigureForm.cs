namespace Omnipaste
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    public partial class ConfigureForm : Form, IConfigureDialog
    {
        public const int MaxRetryCount = 10;

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IActivationDataProvider ActivationDataProvider { get; set; }

        private int _retryCount;

        public ConfigureForm()
        {
            InitializeComponent();
        }

        void IConfigureDialog.ShowDialog()
        {
            ShowDialog();
        }

        public void AssureClipboardIsInitialized()
        {
            var activationData = ActivationDataProvider.GetActivationData();
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
                ConfigurationService.UpdateCommunicationChannel(activationData.Email);
                BackgroundWorker.CancelAsync();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _retryCount = 0;
            BackgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            AssureClipboardIsInitialized();
            if (BackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }

            Invoke((Action)Close);
        }

        private bool ShouldRetryActivation()
        {
            return _retryCount < MaxRetryCount && !BackgroundWorker.CancellationPending;
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            if (BackgroundWorker.IsBusy)
            {
                BackgroundWorker.CancelAsync();
            }

            Close();
        }
    }
}