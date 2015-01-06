namespace Omnipaste.ActivityDetails.Conversation
{
    using Omnipaste.Presenters;
    using OmniUI.Presenters;

    public class ConversationHeaderViewModel : ActivityDetailsHeaderViewModel, IConversationHeaderViewModel
    {
        #region Fields

        private IContactInfoPresenter _contactInfo;

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