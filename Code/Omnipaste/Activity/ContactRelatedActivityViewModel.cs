namespace Omnipaste.Activity
{
    using Omnipaste.Presenters;
    using Omnipaste.Services;

    using OmniUI.Attributes;

    [UseView(typeof(ActivityView))]
    public class ContactRelatedActivityViewModel : ActivityViewModel, IContactRelatedActivityViewModel
    {
        #region Fields

        private IContactInfoPresenter _contactInfo;

        #endregion

        #region Constructors and Destructors

        public ContactRelatedActivityViewModel(IUiRefreshService uiRefreshService, ISessionManager sessionManager)
            : base(uiRefreshService, sessionManager)
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
                if (base.Model != value)
                {
                    ContactInfo = new ContactInfoPresenter(value.ContactInfo);
                }
                base.Model = value;
            }
        }

        #endregion
    }
}