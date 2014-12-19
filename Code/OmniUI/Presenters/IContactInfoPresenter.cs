namespace OmniUI.Presenters
{
    using System.Windows.Media;
    using OmniUI.Models;

    public interface IContactInfoPresenter
    {
        string Identifier { get; set; }

        ImageSource Image { get; set; }

        IContactInfo ContactInfo { get; }
    }
}