namespace Omnipaste.ContactList
{
    using Omnipaste.Models;
    using OmniUI.Details;

    public interface IContactInfoViewModelFactory
    {
        T Create<T>(ContactModel contactModel) 
            where T : IDetailsViewModel<ContactModel>;
    }
}