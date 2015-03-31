namespace Omnipaste.Presenters
{
    using System.ComponentModel;
    using System.Windows.Media;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using OmniUI.Presenters;

    public interface IContactInfoPresenter : IPresenter<ContactEntity>, INotifyPropertyChanged
    {
        string Identifier { get; set; }

        ImageSource Image { get; set; }

        bool IsStarred { get; set; }
    }
}