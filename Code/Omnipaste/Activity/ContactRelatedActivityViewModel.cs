namespace Omnipaste.Activity
{
    using Omnipaste.Presenters;
    using Omnipaste.Services;

    using OmniUI.Attributes;
    using OmniUI.Presenters;

    [UseView(typeof(ActivityView))]
    public class ContactRelatedActivityViewModel : ActivityViewModel, IContactRelatedActivityViewModel
    {
        #region Fields

        private IContactInfoPresenter _contactInfo;

        #endregion

        #region Constructors and Destructors

        public ContactRelatedActivityViewModel(IUiRefreshService uiRefreshService)
            : base(uiRefreshService)
        {
        }

        #endregion

        #region Public Properties

        public IContactInfoPresenter ContactInfo
        {
            get
            {
                return _contactInfo;
            }
            set
            {
                if (Equals(value, _contactInfo))
                {
                    return;
                }
                _contactInfo = value;
                NotifyOfPropertyChange();
            }
        }

        public override ActivityPresenter Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                ContactInfo = new ContactInfoPresenter(value.ExtraData.ContactInfo);
            }
        }

        #endregion
    }
}