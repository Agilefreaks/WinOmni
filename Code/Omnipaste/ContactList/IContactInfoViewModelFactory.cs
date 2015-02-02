namespace Omnipaste.ContactList
{
    using Omnipaste.Presenters;

    public interface IContactInfoViewModelFactory
    {
        IContactInfoViewModel Create(ContactInfoPresenter contactInfoPresenter);
    }
}