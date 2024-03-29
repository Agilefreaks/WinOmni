﻿namespace Omnipaste.Conversations.ContactList.Contact
{
    using System;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IContactViewModel : IDetailsViewModel<ContactModel>
    {
        DateTime? LastActivityTime { get; }

        string Identifier { get; }

        bool IsSelected { get; set; }

        void ShowDetails();
    }
}
