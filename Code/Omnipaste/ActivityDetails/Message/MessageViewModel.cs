namespace Omnipaste.ActivityDetails.Message
{
    using Omnipaste.Activity.Presenters;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;

    public class MessageViewModel : DetailsViewModelBase<Message>, IMessageViewModel
    {
        #region Fields

        private ContactInfoPresenter _contactInfo;

        #endregion

        #region Public Properties

        public ContactInfoPresenter ContactInfo
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

        public override Message Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                ContactInfo = new ContactInfoPresenter(value.ContactInfo);
            }
        }

        #endregion
    }
}