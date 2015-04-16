namespace Omnipaste.Conversations.ContactList
{
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IContactViewModelFactory
    {
        T Create<T>(ContactModel contactModel) 
            where T : IDetailsViewModel<ContactModel>;
    }
}