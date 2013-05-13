using System;
using System.ComponentModel;
using System.Windows.Forms;
using OmniCommon.ExtensionMethods;
using PubNubClipboard;
using PubNubClipboard.Services;

namespace Omnipaste
{
    public partial class ConfigureForm : Form
    {
        public const int MaxRetryCount = 10;

        private readonly IActivationDataProvider _activationDataProvider;
        private readonly IConfigurationService _configurationService;
        private readonly IPubNubClipboard _pubNubClipboard;

        private int _retryCount;

        public ConfigureForm(IActivationDataProvider activationDataProvider, IConfigurationService configurationService, IPubNubClipboard pubNubClipboard)
        {
            InitializeComponent();
            _activationDataProvider = activationDataProvider;
            _configurationService = configurationService;
            _pubNubClipboard = pubNubClipboard;
        }

        public void AssureClipboardIsInitialized()
        {
            var activationData = _activationDataProvider.GetActivationData();
            if (activationData.Channel.IsNullOrWhiteSpace())
            {
                if (ShouldRetryActivation())
                {
                    _retryCount++;
                    AssureClipboardIsInitialized();
                }
            }
            else
            {
                _configurationService.UpdateCommunicationChannel(activationData.Channel);
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
            return !_pubNubClipboard.Initialize() && _retryCount < MaxRetryCount && !BackgroundWorker.CancellationPending;
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