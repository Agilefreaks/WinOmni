namespace Omnipaste.ContactList
{
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Presenters;

    public class ContactInfoViewModelFactory : IContactInfoViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public ContactInfoViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IContactInfoViewModel Create(ContactInfoPresenter contactInfoPresenter)
        {
            var viewModel = _serviceLocator.GetInstance<IContactInfoViewModel>();
            viewModel.Model = contactInfoPresenter;

            return viewModel;
        }
    }
}