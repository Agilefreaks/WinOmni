namespace OmniUI.Models
{
    using System;

    public interface IContactInfo
    {
        string Name { get; }

        string Phone { get; }

        Uri ImageUri { get; }
    }
}