namespace Omnipaste.ContactList
{
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public class ContactInfoViewModelFactory : IContactInfoViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public ContactInfoViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public T Create<T>(ContactInfoPresenter contactInfoPresenter)
            where T : IDetailsViewModel<ContactInfoPresenter>
        {
            var viewModel = _serviceLocator.GetInstance<T>();
            viewModel.Model = contactInfoPresenter;

            return viewModel;
        }
    }
}