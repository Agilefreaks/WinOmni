namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using OmniUI.Details;
    using OmniUI.Presenters;

    public class ContactViewModel : DetailsViewModelBase<IContactInfoPresenter>, IContactViewModel
    {
        #region Fields

        private bool _isSelected;

        #endregion

        #region Public Properties

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value.Equals(_isSelected))
                {
                    return;
                }
                _isSelected = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion
    }
}