namespace Omnipaste.Shell.ContextMenu
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.Framework.EventAggregatorMessages;

    public interface IContextMenuViewModel : IScreen, IDisposable, IHandle<ApplicationClosingMessage>
    {
        #region Public Properties

        bool AutoStart { get; set; }
        
        string IconSource { get; set; }

        string TooltipText { get; }

        #endregion

        #region Public Methods and Operators

        void Show();

        void ShowBalloon(string balloonTitle, string balloonMessage);

        #endregion
    }
}