namespace Omnipaste.ContactList
{
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public interface IContactInfoViewModelFactory
    {
        T Create<T>(ContactInfoPresenter contactInfoPresenter) 
            where T : IDetailsViewModel<ContactInfoPresenter>;
    }
}