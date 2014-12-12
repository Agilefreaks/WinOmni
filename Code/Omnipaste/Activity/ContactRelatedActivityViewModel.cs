namespace Omnipaste.Activity
{
    using Omnipaste.Activity.Presenters;
    using Omnipaste.Models;
    using Omnipaste.Services;

    using OmniUI.Attributes;

    [UseView("Omnipaste.Activity.ActivityView", IsFullyQualifiedName = true)]
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

        public override Models.Activity Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                ContactInfo contactInfo = value.ExtraData.ContactInfo;
                ContactInfo = new ContactInfoPresenter(contactInfo);
            }
        }

        #endregion
    }
}