using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClipboardWatcher.Core;
using ClipboardWatcher.Core.Services;

namespace Omnipaste
{
    public partial class ConfigureForm : Form
    {
        public const int MaxRetryCount = 10;

        private readonly IActivationDataProvider _activationDataProvider;
        private readonly IConfigurationService _configurationService;
        private readonly ICloudClipboard _cloudClipboard;

        private int _retryCount;

        public ConfigureForm(IActivationDataProvider activationDataProvider, IConfigurationService configurationService, ICloudClipboard cloudClipboard)
        {
            InitializeComponent();
            _activationDataProvider = activationDataProvider;
            _configurationService = configurationService;
            _cloudClipboard = cloudClipboard;
        }

        public void AssureClipboardIsInitialized()
        {
            var activationData = _activationDataProvider.GetActivationData();
            _configurationService.UpdateCommunicationChannel(activationData.Channel);
            if (ShouldRetryActivation())
            {
                _retryCount++;
                AssureClipboardIsInitialized();
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
            return !_cloudClipboard.Initialize() && _retryCount < MaxRetryCount && !BackgroundWorker.CancellationPending;
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