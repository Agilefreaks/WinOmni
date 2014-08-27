namespace Omnipaste.Services.SystemService
{
    using System;
    using Microsoft.Win32;

    public class SystemService : ISystemService
    {
        #region Public Events

        public event EventHandler<EventArgs> Resume;

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            SystemEvents.PowerModeChanged += PowerModeChanged;
        }

        public void Stop()
        {
            SystemEvents.PowerModeChanged -= PowerModeChanged;
        }

        #endregion

        #region Methods

        private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                Resume(this, new EventArgs());
            }
        }

        #endregion
    }
}