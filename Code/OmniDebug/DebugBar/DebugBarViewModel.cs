namespace OmniDebug.DebugBar
{
    using System.Collections.Generic;
    using MahApps.Metro.Controls;
    using OmniUI.Flyout;

    public class DebugBarViewModel : FlyoutBaseViewModel, IDebugBarViewModel
    {
        #region Constructors and Destructors

        public DebugBarViewModel(IEnumerable<IDebugBarPanel> debugBarPanels)
        {
            DebugBarPanels = debugBarPanels;
            Position = Position.Left;
        }

        #endregion

        #region Public Properties

        public IEnumerable<IDebugBarPanel> DebugBarPanels { get; set; }

        #endregion
    }
}