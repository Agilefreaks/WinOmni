﻿namespace Omnipaste
{
    using System;
    using System.Deployment.Application;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using CustomizedClickOnce.Common;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using WindowsClipboard.Imports;
    using WindowsClipboard.Interfaces;

    public partial class MainForm : Form, IDelegateClipboardMessageHandling
    {
        private const int PopupLifeSpan = 3000;

        public event MessageHandler HandleClipboardMessage;

        public bool IsNotificationIconVisible
        {
            get { return NotifyIcon.Visible; }
            set { NotifyIcon.Visible = value; }
        }

        [Inject]
        public IOmniService OmniService { get; set; }

        [Inject]
        public IOmniClipboard OmniClipboard { get; set; }

        [Inject]
        public IApplicationDeploymentInfoProvider ApplicationDeploymentInfoProvider { get; set; }

        [Inject]
        public IConfigureDialog ConfigureForm { get; set; }

        [Inject]
        public IClickOnceHelper ClickOnceHelper { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        public IntPtr GetHandle()
        {
            var handle = new IntPtr();
            Action getHandleDelegate = () => handle = Handle;
            if (InvokeRequired)
            {
                Invoke(getHandleDelegate);
                return handle;
            }

            return Handle;
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

            PerformInitializations();
        }

        protected void PerformInitializations()
        {
            IsNotificationIconVisible = true;
            HideWindowFromAltTab();
            LoadInitialConfiguration();
            SetVersionInfo();
            SetAutoStartInfo();
            OmniClipboard.Logger = new SimpleDefferingLogger(ShowLogMessage);
            Task.Factory.StartNew(StartOmniService);
        }

        protected void LoadInitialConfiguration()
        {
            if (ApplicationDeploymentInfoProvider.IsFirstNetworkRun)
            {
                ConfigureForm.ShowDialog();
            }
        }

        protected void AddCurrentUserMenuEntry()
        {
            var toolStripMenuItem = new ToolStripLabel
                {
                    Enabled = false,
                    Text = string.Format("Logged in as: \"{0}\"", OmniClipboard.Channel)
                };
            Action addItemsAction = () =>
                {
                    trayIconContextMenuStrip.Items.Insert(0, toolStripMenuItem);
                    trayIconContextMenuStrip.Items.Insert(1, new ToolStripSeparator());
                };
            if (InvokeRequired)
            {
                Invoke(addItemsAction);
            }
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private void HideWindowFromAltTab()
        {
            var exStyle = (int)User32.GetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WindowHelper.SetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void DisableButtonClick(object sender, EventArgs e)
        {
            if (DisableButton.Checked)
            {
                OmniService.Stop();
            }
            else
            {
                OmniService.Start();
            }
        }

        private void SetVersionInfo()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var ad = ApplicationDeployment.CurrentDeployment;
                version = ad.CurrentVersion;
            }

            NotifyIcon.Text = string.Format("{0} - {1}.{2}.{3}.{4}", ApplicationInfoFactory.ApplicationName, version.Major, version.Minor, version.Build, version.Revision);
        }

        private void ShowLogMessage(string message)
        {
            if (InvokeRequired)
            {
                Action invokeDelegate = () => ShowBalloonPopup(message);
                Invoke(invokeDelegate);
            }
            else
            {
                ShowBalloonPopup(message);
            }
        }

        private void ShowBalloonPopup(string message)
        {
            NotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            NotifyIcon.BalloonTipText = message;
            NotifyIcon.BalloonTipTitle = ApplicationInfoFactory.ApplicationName;
            NotifyIcon.ShowBalloonTip(PopupLifeSpan);
        }

        private void StartOmniService()
        {
            var startTask = OmniService.Start();
            Task.WaitAll(startTask);
            if (startTask.Result)
            {
                AddCurrentUserMenuEntry();
                DisableButton.Checked = false;
            }
            else
            {
                OmniService.Stop();
                DisableButton.Checked = true;
                ShowLogMessage(Resources.CouldNotInitializeSynchronizationService);
            }
        }

        private void SetAutoStartInfo()
        {
            AutoStartCheckbox.Checked = ClickOnceHelper.StartupShortcutExists();
        }
    }
}
