namespace Omnipaste.ContactList
{
    using Omnipaste.Models;
    using OmniUI.Details;

    public interface IContactViewModelFactory
    {
        T Create<T>(ContactModel contactModel) 
            where T : IDetailsViewModel<ContactModel>;
    }
}