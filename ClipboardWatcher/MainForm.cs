using System;
using System.Windows.Forms;
using ClipboardWatcher.Core;
using ClipboardWrapper;
using ClipboardWrapper.Imports;
using Ninject;

namespace ClipboardWatcher
{
    public partial class MainForm : Form
    {
        public bool IsNotificationIconVisible
        {
            get { return NotifyIcon.Visible; }
            set { NotifyIcon.Visible = value; }
        }

        [Inject]
        public IClipboardWrapper ClipboardWrapper { get; set; }

        [Inject]
        public ICloudClipboard CloudClipboard { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            ClipboardWrapper.RegisterClipboardViewer(Handle);
            base.OnHandleCreated(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ClipboardWrapper.UnRegisterClipboardViewer(Handle);
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
            else if(pasteDataFromMessage.MessageData != null)
            {
                CloudClipboard.Copy(pasteDataFromMessage.MessageData);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IsNotificationIconVisible = true;
            HideWindowFromAltTab();
        }

        private void HideWindowFromAltTab()
        {
            var exStyle = (int)User32.GetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WindowHelper.SetWindowLong(Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
    }
}
