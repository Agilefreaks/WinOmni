namespace Omnipaste.ContactList.Contact
{
    using System;
    using Omnipaste.Models;
    using OmniUI.Details;

    public interface IContactViewModel : IDetailsViewModel<ContactModel>
    {
        DateTime? LastActivityTime { get; }

        string Identifier { get; }

        bool IsSelected { get; set; }

        void ShowDetails();
    }
}
