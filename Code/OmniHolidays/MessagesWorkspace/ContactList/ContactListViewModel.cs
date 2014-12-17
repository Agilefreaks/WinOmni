namespace OmniHolidays.MessagesWorkspace.ContactList
{
    using Caliburn.Micro;
    using Ninject;

    public class ContactListViewModel : Screen, IContactListViewModel
    {
        [Inject]
        public IContactListHeaderViewModel HeaderViewModel { get; set; }

        [Inject]
        public IContactListContentViewModel ContentViewModel { get; set; }

        protected override void OnActivate()
        {
            HeaderViewModel.Activate();
            ContentViewModel.Activate();
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            HeaderViewModel.Deactivate(close);
            ContentViewModel.Deactivate(close);
            base.OnDeactivate(close);
        }
    }
}