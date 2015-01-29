namespace Omnipaste.Presenters
{
    using System.ComponentModel;
    using System.Windows.Media;
    using Omnipaste.Models;

    public interface IContactInfoPresenter : INotifyPropertyChanged
    {
        string Identifier { get; set; }

        ImageSource Image { get; set; }

        IContactInfo ContactInfo { get; }

        bool IsSelected { get; set; }
    }
}