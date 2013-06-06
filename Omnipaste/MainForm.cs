namespace Omnipaste
{
    using System;
    using System.Deployment.Application;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;
    using WindowsClipboard.Imports;
    using WindowsClipboard.Interfaces;

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
        public IOmniClipboard OmniClipboard { get; set; }

        [Inject]
        public IApplicationDeploymentInfoProvider ApplicationDeploymentInfoProvider { get; set; }

        [Inject]
        public ConfigureForm ConfigureForm { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        public IntPtr GetHandle()
        {
            var handle = new IntPtr();
            Action getHandleDelegate = () => handle = this.Handle;
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

            IsNotificationIconVisible = true;
            HideWindowFromAltTab();
            LoadInitialConfiguration();
            SetVersionInfo();
            Task.Factory.StartNew(() =>
                {
                    Task.WaitAll(OmniService.Start());
                    AddCurrentUserMenuEntry();
                });
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
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                version = ad.CurrentVersion;
            }

            NotifyIcon.Text = string.Format("{0} - {1}.{2}.{3}.{4}", MainModule.ApplicationName, version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
