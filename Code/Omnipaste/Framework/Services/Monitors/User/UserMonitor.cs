namespace Omnipaste.Framework.Services.Monitors.User
{
    using System;
    using System.Reactive.Subjects;

    public class UserMonitor : IUserMonitor
    {
        #region Fields

        private readonly ReplaySubject<UserEventTypeEnum> _subject;

        #endregion

        #region Constructors and Destructors

        public UserMonitor()
        {
            _subject = new ReplaySubject<UserEventTypeEnum>(0);
        }

        #endregion

        #region Public Properties

        public IObservable<UserEventTypeEnum> UserEventObservable
        {
            get
            {
                return _subject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void SendEvent(UserEventTypeEnum eventType)
        {
            _subject.OnNext(eventType);
        }

        #endregion
    }
}