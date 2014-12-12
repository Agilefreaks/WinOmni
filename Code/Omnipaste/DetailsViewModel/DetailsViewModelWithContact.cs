namespace Omnipaste.DetailsViewModel
{
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;

    public abstract class DetailsViewModelWithContact<TModel> : DetailsViewModelWithAutoRefresh<TModel>
        where TModel : class, IHaveContactInfo
    {
        #region Fields

        private ContactInfoPresenter _contactInfo;

        #endregion

        #region Constructors and Destructors

        protected DetailsViewModelWithContact(IUiRefreshService uiRefreshService)
            : base(uiRefreshService)
        {
        }

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

        public override TModel Model
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