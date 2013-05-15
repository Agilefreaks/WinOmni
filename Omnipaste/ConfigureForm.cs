using System;
using System.ComponentModel;
using System.Windows.Forms;
using OmniCommon.ExtensionMethods;
using OmniCommon.Interfaces;

namespace Omnipaste
{
    public partial class ConfigureForm : Form
    {
        public const int MaxRetryCount = 10;

        private readonly IActivationDataProvider _activationDataProvider;
        private readonly IConfigurationService _configurationService;
        private readonly IOmniClipboard _omniClipboard;

        private int _retryCount;

        public ConfigureForm(IActivationDataProvider activationDataProvider, IConfigurationService configurationService, IOmniClipboard omniClipboard)
        {
            InitializeComponent();
            _activationDataProvider = activationDataProvider;
            _configurationService = configurationService;
            this._omniClipboard = omniClipboard;
        }

        public void AssureClipboardIsInitialized()
        {
            var activationData = _activationDataProvider.GetActivationData();
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
                _configurationService.UpdateCommunicationChannel(activationData.Email);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BackgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
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
            return !this._omniClipboard.Initialize() && _retryCount < MaxRetryCount && !BackgroundWorker.CancellationPending;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (BackgroundWorker.IsBusy)
            {
                BackgroundWorker.CancelAsync();
            }

            Close();
        }
    }
}