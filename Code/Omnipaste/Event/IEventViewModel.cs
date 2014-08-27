﻿namespace Omnipaste.Event
{
    using Events.Models;

    public interface IEventViewModel : IDetailsViewModel<Event>
    {
        #region Public Properties

        EventTypeEnum Type { get; }

        #endregion

        #region Public Methods and Operators

        void CallBack();

        void SendSms();

        #endregion
    }
}