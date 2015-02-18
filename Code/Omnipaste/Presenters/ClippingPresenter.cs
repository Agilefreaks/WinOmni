namespace Omnipaste.Presenters
{
    using System;
    using Caliburn.Micro;
    using Clipboard.Models;
    using Omnipaste.Models;
    using Omnipaste.Properties;

    public class ClippingPresenter : PropertyChangedBase
    {
        public ClippingPresenter()
        {
        }

        public ClippingPresenter(ClippingModel clipping)
        {
            BackingModel = clipping;
            Content = clipping.Content;
            Device = BackingModel.Source == Clipping.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
        }

        public ClippingModel BackingModel { get; private set; }

        public string Content { get; private set; }

        public string Device { get; private set; }
        
        public string UniqueId
        {
            get
            {
                return BackingModel.UniqueId;
            }
        }
        
        public DateTime Time
        {
            get
            {
                return BackingModel.Time;
            }
        }

        public bool WasViewed
        {
            get
            {
                return BackingModel.WasViewed;
            }
            set
            {
                if (value.Equals(BackingModel.WasViewed))
                {
                    return;
                }
                BackingModel.WasViewed = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsDeleted
        {
            get
            {
                return BackingModel.IsDeleted;
            }
            set
            {
                if (value.Equals(BackingModel.IsDeleted))
                {
                    return;
                }
                BackingModel.IsDeleted = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsStarred
        {
            get
            {
                return BackingModel.IsStarred;
            }
            set
            {
                if (value.Equals(BackingModel.IsStarred))
                {
                    return;
                }
                BackingModel.IsStarred = value;
                NotifyOfPropertyChange(() => IsStarred);
            }
        }
    }
}
