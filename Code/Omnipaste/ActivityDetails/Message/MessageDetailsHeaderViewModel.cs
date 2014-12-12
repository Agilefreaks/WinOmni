namespace Omnipaste.ActivityDetails.Message
{
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    public class MessageDetailsHeaderViewModel : ActivityDetailsHeaderViewModel, IMessageDetailsHeaderViewModel
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

        public override Activity Model
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