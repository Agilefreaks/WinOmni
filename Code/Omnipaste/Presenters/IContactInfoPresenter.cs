namespace Omnipaste.Presenters
{
    using System.ComponentModel;
    using System.Windows.Media;
    using Omnipaste.Models;
    using OmniUI.Presenters;

    public interface IContactInfoPresenter : IPresenter<ContactInfo>, INotifyPropertyChanged
    {
        string Identifier { get; set; }

        ImageSource Image { get; set; }

        bool IsStarred { get; set; }
    }
}