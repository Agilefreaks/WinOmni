namespace Omnipaste.ContactList
{
    using System;
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public interface IContactInfoSelectionViewModel : IDetailsViewModel<ContactInfoPresenter>
    {
        DateTime? LastActivityTime { get; set; }
    }
}