namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using System.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniUI.Presenters;

    public class ContactListContentViewModel : Conductor<IContactViewModel>.Collection.AllActive, IContactListContentViewModel
    {
        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            Enumerable.Range(0, 20).ToList().ForEach(_ => ActivateItem(GetContactViewModel(new ContactInfoPresenter())));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(true);
        }

        private IContactViewModel GetContactViewModel(ContactInfoPresenter contactInfoPresenter)
        {
            var contactViewModel = Kernel.Get<IContactViewModel>();
            contactViewModel.Model = contactInfoPresenter;

            return contactViewModel;
        }

        #endregion
    }
}