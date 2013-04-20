using System;
using System.Windows.Forms;
using ClipboardWatcher.Core;
using ClipboardWatcher.Core.Services;
using ClipboardWrapper;
using ClipboardWrapper.Imports;
using Ninject;

namespace ClipboardWatcher
{
    public partial class MainForm : Form
    {
        private ICloudClipboard _cloudClipboard;
        private bool _canSendData = true;

        public bool IsNotificationIconVisible
        {
            get { return NotifyIcon.Visible; }
            set { NotifyIcon.Visible = value; }
        }

        [Inject]
        public IClipboardWrapper ClipboardWrapper { get; set; }

        [Inject]
        public ICloudClipboard CloudClipboard
        {
            get
            {
                return _cloudClipboard;
            }

            set
            {
                _cloudClipboard = value;
                if (_cloudClipboard != null)
                {
                    _cloudClipboard.DataReceived += CloudClipboardOnDataReceived;
                }
            }
        }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IActivationDataProvider ActivationDataProvider { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        public void SendDataToClipboard(ClipboardEventArgs clipboardEventArgs)
        {
            this._canSendData = false;
            ClipboardWrapper.SendToClipboard(clipboardEventArgs.Data);
            this._canSendData = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            ClipboardWrapper.RegisterClipboardViewer(Handle);
            base.OnHandleCreated(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ClipboardWrapper.UnRegisterClipboardViewer(Handle);
            CloudClipboard.Dispose();
            IsNotificationIconVisible = false;

            base.OnClosing(e);
        }

        protected override void WndProc(ref Message message)
        {
            var pasteDataFromMessage = ClipboardWrapper.HandleClipboardMessage(message);
            if (!pasteDataFromMessage.MessageHandled)
            {
                base.WndProc(ref message);
            }
            else if (pasteDataFromMessage.MessageData != null && this._canSendData)
            {
                CloudClipboard.Copy(pasteDataFromMessage.MessageData);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IsNotificationIconVisible = true;
            HideWindowFromAltTab();
            AssureClipboardIsInitialized();
        }

        protected void AssureClipboardIsInitialized()
        {
            if (CloudClipboard.IsInitialized)
            {
                return;
            }

            var configureForm = new ConfigureForm(ActivationDataProvider, ConfigurationService, CloudClipboard);
            configureForm.ShowDialog();
            if (!CloudClipboard.IsInitialized)
            {
                Close();
            }
        }

        private void HideWindowFromAltTab()
        {
            var exstyle = (int)User32.GetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exstyle |= ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WindowHelper.SetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exstyle);
        }

        private void CloudClipboardOnDataReceived(object sender, ClipboardEventArgs clipboardEventArgs)
        {
            Invoke((Action)(() => SendDataToClipboard(clipboardEventArgs)));
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
