namespace Omnipaste.Presenters
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.Models;

    public class ActivityPresenter : PropertyChangedBase
    {
        #region Fields

        private readonly Activity _activity;

        private bool _markedForDeletion;

        #endregion

        #region Constructors and Destructors

        public ActivityPresenter()
        {
            _activity = new Activity();
        }

        public ActivityPresenter(Activity activity)
        {
            _activity = activity;
        }

        #endregion

        #region Public Properties

        public string Content
        {
            get
            {
                return _activity.Content;
            }
        }

        public string Device
        {
            get
            {
                return _activity.Device;
            }
        }

        public dynamic ExtraData
        {
            get
            {
                return _activity.ExtraData;
            }
        }

        public bool MarkedForDeletion
        {
            get
            {
                return _markedForDeletion;
            }
            set
            {
                if (value.Equals(_markedForDeletion))
                {
                    return;
                }
                _markedForDeletion = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime Time
        {
            get
            {
                return _activity.Time;
            }
        }

        public ActivityTypeEnum Type
        {
            get
            {
                return _activity.Type;
            }
        }

        public bool WasViewed
        {
            get
            {
                return _activity.WasViewed;
            }
            set
            {
                if (_activity.WasViewed == value)
                {
                    return;
                }

                _activity.WasViewed = value;
                NotifyOfPropertyChange(() => WasViewed);
            }
        }

        #endregion
    }
}