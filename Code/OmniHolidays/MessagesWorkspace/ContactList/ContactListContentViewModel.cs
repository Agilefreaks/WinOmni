namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using OmniUI.Presenters;

    public class ContactListContentViewModel : Screen, IContactListContentViewModel
    {
        #region Constructors and Destructors

        public ContactListContentViewModel()
        {
            Items = new List<IContactInfoPresenter>();
        }

        #endregion

        #region Public Properties

        public IList<IContactInfoPresenter> Items { get; private set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            Enumerable.Range(0, 20).ToList().ForEach(_ => Items.Add(new ContactInfoPresenter()));
        }

        #endregion
    }
}