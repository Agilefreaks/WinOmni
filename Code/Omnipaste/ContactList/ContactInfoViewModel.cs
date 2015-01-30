namespace Omnipaste.ContactList
{
    using System.ComponentModel;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;

    public class ContactInfoViewModel : DetailsViewModelBase<ContactInfoPresenter>, IContactInfoViewModel
    {
        [Inject]
        public IContactRepository ContactRepository { get; set; }

        protected override void HookModel(ContactInfoPresenter model)
        {
            model.PropertyChanged += OnPropertyChanged;
        }

        protected override void UnhookModel(ContactInfoPresenter model)
        {
            model.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Model.GetPropertyName(m => m.IsStarred))
            {
                SaveChanges();
            }
        }

        private void SaveChanges()
        {
            ContactRepository.Save(Model.ContactInfo).RunToCompletion();
        }
    }
}