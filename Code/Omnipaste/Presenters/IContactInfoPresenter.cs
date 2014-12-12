namespace Omnipaste.Presenters
{
    using System.Windows.Media;

    public interface IContactInfoPresenter
    {
        string Identifier { get; set; }

        ImageSource Image { get; set; }
    }
}