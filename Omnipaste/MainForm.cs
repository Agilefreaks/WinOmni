using System;
using System.Deployment.Application;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Ninject;
using OmniCommon.Interfaces;
using PubNubClipboard;
using WindowsClipboard.Imports;
using WindowsClipboard.Interfaces;
using System.Reflection;

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
        public IPubNubClipboard OmniClipboard { get; set; }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IActivationDataProvider ActivationDataProvider { get; set; }

        public MainForm()
        {
            InitializeComponent();

            Assembly exeAssembly = Assembly.GetEntryAssembly();
            AssemblyName exeName = exeAssembly.GetName();
            Version exeVersion = exeName.Version;
            string fullVersion = exeVersion.ToString(4);
            NotifyIcon.Text = string.Format("{0} - {1}", MainModule.ApplicationName, fullVersion);
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
            AssureRemoteClipboardIsInitialized();

            OmniService.Start();
            AddCurrentUserMenuEntry();
        }

        protected void AssureRemoteClipboardIsInitialized()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment deployment = ApplicationDeployment.CurrentDeployment;

                if (deployment.IsFirstRun)
                {
                    var configureForm = new ConfigureForm(ActivationDataProvider, ConfigurationService);
                    configureForm.ShowDialog();
                }
            }
            else
            {
                var configureForm = new ConfigureForm(ActivationDataProvider, ConfigurationService);
                configureForm.ShowDialog();
            }
        }

        protected void AddCurrentUserMenuEntry()
        {
            var toolStripMenuItem = new ToolStripLabel
                {
                    Enabled = false,
                    Text = string.Format("Logged in as: \"{0}\"", this.OmniClipboard.Channel)
                };
            trayIconContextMenuStrip.Items.Insert(0, toolStripMenuItem);
            trayIconContextMenuStrip.Items.Insert(1, new ToolStripSeparator());
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
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
                OmniService.Stop();
            }
            else
            {
                OmniService.Start();
            }
        }
    }
}
