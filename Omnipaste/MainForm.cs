using System;
using System.Windows.Forms;
using Ninject;
using OmniCommon.Interfaces;
using PubNubClipboard;
using PubNubClipboard.Services;
using WindowsClipboard.Imports;
using WindowsClipboard.Interfaces;

namespace Omnipaste
{
    public partial class MainForm : Form, IDelegateClipboardMessageHandling
    {
        public event MessageHandler HandleClipboardMessage;

        public bool IsNotificationIconVisible
        {
            get { return NotifyIcon.Visible; }
            set { NotifyIcon.Visible = value; }
        }

        [Inject]
        public IOmniService OmniService { get; set; }

        [Inject]
        public IWindowsClipboard WindowsClipboard { get; set; }

        [Inject]
        public IPubNubClipboard PubNubClipboard { get; set; }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IActivationDataProvider ActivationDataProvider { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            OmniService.Stop();
            IsNotificationIconVisible = false;

            base.OnClosing(e);
        }

        protected override void WndProc(ref Message message)
        {
            if (HandleClipboardMessage == null || !HandleClipboardMessage(ref message))
            {
                base.WndProc(ref message);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IsNotificationIconVisible = true;
            HideWindowFromAltTab();
            OmniService.Start();
            AssureRemoteClipboardIsInitialized();
            AddCurrentUserMenuEntry();
        }

        protected void AssureRemoteClipboardIsInitialized()
        {
            if (PubNubClipboard.IsInitialized) return;
            var configureForm = new ConfigureForm(ActivationDataProvider, ConfigurationService, PubNubClipboard);
            configureForm.ShowDialog();
            if (!PubNubClipboard.IsInitialized)
            {
                Close();
            }
        }

        protected void AddCurrentUserMenuEntry()
        {
            var toolStripMenuItem = new ToolStripLabel
                {
                    Enabled = false,
                    Text = string.Format("Logged in as: \"{0}\"", PubNubClipboard.Channel)
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

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DisableButton_Click(object sender, EventArgs e)
        {
            if (DisableButton.Checked)
            {
                OmniService.Start();
            }
            else
            {
                OmniService.Stop();
            }
        }
    }
}
