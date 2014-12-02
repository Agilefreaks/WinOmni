namespace Omnipaste.Activity
{
    using System.Windows.Media;

    public interface IContactRelatedActivityViewModel : IActivityViewModel
    {
        ImageSource ContactImage { get; set; }
    }
}