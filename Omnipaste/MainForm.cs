using System;
using System.Windows.Forms;
using Ninject;
using Omniclipboard;
using Omniclipboard.Services;
using WindowsClipboard;
using WindowsClipboard.Imports;

namespace Omnipaste
{
    public partial class MainForm : Form
    {
        private IOmniclipboard _omniclipboard;
        private bool _isSynchronizationDisabled;

        public bool CanSendData { get; protected set; }

        public bool IsNotificationIconVisible
        {
            get { return NotifyIcon.Visible; }
            set { NotifyIcon.Visible = value; }
        }

        public bool IsSynchronizationDisabled
        {
            get
            {
                return _isSynchronizationDisabled;
            }
            private set
            {
                _isSynchronizationDisabled = value;
                OnIsSynchronizationDisabledChanged(IsSynchronizationDisabled);
            }
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
            ClipboardWrapper.Initialize(Handle);
            base.OnHandleCreated(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ClipboardWrapper.Dispose();
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
            AddCurrentUserMenuEntry();
        }

        protected void OnIsSynchronizationDisabledChanged(bool isSynchronizationDisabled)
        {
            if (isSynchronizationDisabled)
            {
                ClipboardWrapper.Dispose();
                Omniclipboard.Dispose();
            }
            else
            {
                ClipboardWrapper.Initialize(Handle);
                Omniclipboard.Initialize();
            }
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

        protected void AddCurrentUserMenuEntry()
        {
            var toolStripMenuItem = new ToolStripLabel
                {
                    Enabled = false,
                    Text = string.Format("Logged in as: \"{0}\"", Omniclipboard.Channel)
                };
            trayIconContextMenuStrip.Items.Insert(0, toolStripMenuItem);
            trayIconContextMenuStrip.Items.Insert(1, new ToolStripSeparator());
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

        private void DisableButton_Click(object sender, EventArgs e)
        {
            IsSynchronizationDisabled = DisableButton.Checked;
        }
    }
}
