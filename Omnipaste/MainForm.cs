using System;
using System.Windows.Forms;
using ClipboardWrapper;
using ClipboardWrapper.Imports;
using Ninject;
using Omniclipboard;
using Omniclipboard.Services;

namespace Omnipaste
{
    public partial class MainForm : Form
    {
        private IOmniclipboard _omniclipboard;

        public bool CanSendData { get; protected set; }

        public bool IsNotificationIconVisible
        {
            get { return NotifyIcon.Visible; }
            set { NotifyIcon.Visible = value; }
        }

        [Inject]
        public IClipboardWrapper ClipboardWrapper { get; set; }

        [Inject]
        public IOmniclipboard Omniclipboard
        {
            get
            {
                return _omniclipboard;
            }

            set
            {
                _omniclipboard = value;
                if (_omniclipboard != null)
                {
                    _omniclipboard.DataReceived += OmniclipboardOnDataReceived;
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
            CanSendData = false;
            ClipboardWrapper.SendToClipboard(clipboardEventArgs.Data);
            CanSendData = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            ClipboardWrapper.RegisterClipboardViewer(Handle);
            base.OnHandleCreated(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ClipboardWrapper.UnRegisterClipboardViewer(Handle);
            Omniclipboard.Dispose();
            IsNotificationIconVisible = false;

            base.OnClosing(e);
        }

        protected override void WndProc(ref Message message)
        {
            var handleResult = ClipboardWrapper.HandleClipboardMessage(message);
            if (!handleResult.MessageHandled)
            {
                base.WndProc(ref message);
            }
            else if (handleResult.MessageData != null && CanSendData)
            {
                Omniclipboard.Copy(handleResult.MessageData);
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
            if (!Omniclipboard.IsInitialized)
            {
                var configureForm = new ConfigureForm(ActivationDataProvider, ConfigurationService, Omniclipboard);
                configureForm.ShowDialog();
                if (!Omniclipboard.IsInitialized)
                {
                    Close();
                }
            }

            CanSendData = Omniclipboard.IsInitialized;
        }

        private void HideWindowFromAltTab()
        {
            var exStyle = (int)User32.GetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WindowHelper.SetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void OmniclipboardOnDataReceived(object sender, ClipboardEventArgs clipboardEventArgs)
        {
            Invoke((Action)(() => SendDataToClipboard(clipboardEventArgs)));
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
