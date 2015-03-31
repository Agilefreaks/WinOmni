namespace Omnipaste.ContactList
{
    using System;
    using Omnipaste.Models;
    using OmniUI.Details;

    public interface IContactInfoViewModel : IDetailsViewModel<ContactModel>
    {
        DateTime? LastActivityTime { get; }

        string Identifier { get; }

        bool IsSelected { get; set; }

        void ShowDetails();
    }
}
