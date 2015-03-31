namespace Omnipaste.ContactList
{
    using Microsoft.Practices.ServiceLocation;
    using Omnipaste.Models;
    using OmniUI.Details;

    public class ContactViewModelFactory : IContactViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public ContactViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public T Create<T>(ContactModel contactModel)
            where T : IDetailsViewModel<ContactModel>
        {
            var viewModel = _serviceLocator.GetInstance<T>();
            viewModel.Model = contactModel;

            return viewModel;
        }
    }
}