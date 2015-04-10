namespace Omnipaste.ActivityList.Activity
{
    using Omnipaste.Framework.Models;
    using Omnipaste.Services;
    using OmniUI.Attributes;

    [UseView(typeof(ActivityView))]
    public class ContactRelatedActivityViewModel : ActivityViewModel, IContactRelatedActivityViewModel
    {
        #region Fields

        private ContactModel _contact;

        #endregion

        #region Constructors and Destructors

        public ContactRelatedActivityViewModel(IUiRefreshService uiRefreshService, ISessionManager sessionManager)
            : base(uiRefreshService, sessionManager)
        {
        }

        #endregion

        #region Public Properties

        public ContactModel Contact
        {
            get
            {
                return _contact;
            }
            set
            {
                if (Equals(value, _contact))
                {
                    return;
                }
                _contact = value;
                NotifyOfPropertyChange();
            }
        }

        public override ActivityModel Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                if (base.Model != value)
                {
                    Contact = new ContactModel(value.ContactEntity);
                }
                base.Model = value;
            }
        }

        #endregion
    }
}