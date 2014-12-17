namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System.Linq;
    using Caliburn.Micro;
    using OmniUI.Presenters;

    public class ContactListContentViewModel : Screen, IContactListContentViewModel
    {
        #region Constructors and Destructors

        public ContactListContentViewModel()
        {
            Items = new BindableCollection<IContactInfoPresenter>();
        }

        #endregion

        #region Public Properties

        public IObservableCollection<IContactInfoPresenter> Items { get; private set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            Enumerable.Range(0, 20).ToList().ForEach(_ => Items.Add(new ContactInfoPresenter()));
        }

        protected override void OnDeactivate(bool close)
        {
            Items.Clear();
            base.OnDeactivate(close);
        }

        #endregion
    }
}