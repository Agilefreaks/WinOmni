namespace Omnipaste
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AutoStartCheckbox = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.DisableButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitButton = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIconContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.ContextMenuStrip = this.trayIconContextMenuStrip;
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "Omnipaste";
            this.NotifyIcon.Visible = true;
            // 
            // trayIconContextMenuStrip
            // 
            this.trayIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AutoStartCheckbox,
            this.settingsSeparator,
            this.DisableButton,
            this.ExitButton});
            this.trayIconContextMenuStrip.Name = "trayIconContextMenuStrip";
            this.trayIconContextMenuStrip.Size = new System.Drawing.Size(187, 76);
            // 
            // AutoStartCheckbox
            // 
            this.AutoStartCheckbox.Name = "AutoStartCheckbox";
            this.AutoStartCheckbox.Size = new System.Drawing.Size(186, 22);
            this.AutoStartCheckbox.Text = "Start With Windows";
            // 
            // settingsSeparator
            // 
            this.settingsSeparator.Name = "settingsSeparator";
            this.settingsSeparator.Size = new System.Drawing.Size(183, 6);
            // 
            // DisableButton
            // 
            this.DisableButton.CheckOnClick = true;
            this.DisableButton.Name = "DisableButton";
            this.DisableButton.Size = new System.Drawing.Size(186, 22);
            this.DisableButton.Text = "Stop Synchronization";
            this.DisableButton.ToolTipText = "Temporarily stop clipboard synchronization";
            this.DisableButton.Click += new System.EventHandler(this.DisableButtonClick);
            // 
            // ExitButton
            // 
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(186, 22);
            this.ExitButton.Text = "Exit";
            this.ExitButton.Click += new System.EventHandler(this.ExitButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1, 1);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.Text = "ClipboardWatcher";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.trayIconContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.ContextMenuStrip trayIconContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ExitButton;
        private System.Windows.Forms.ToolStripMenuItem DisableButton;
        private System.Windows.Forms.ToolStripSeparator settingsSeparator;
        public System.Windows.Forms.ToolStripMenuItem AutoStartCheckbox;


    }
}

