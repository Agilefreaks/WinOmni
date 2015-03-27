namespace Omnipaste.ContactList
{
    using System;
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public interface IContactInfoViewModel : IDetailsViewModel<ContactInfoPresenter>
    {
        DateTime? LastActivityTime { get; }

        string Identifier { get; }

        void ShowDetails();
    }
}
