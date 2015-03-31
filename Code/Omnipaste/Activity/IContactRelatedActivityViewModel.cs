namespace Omnipaste.Activity
{
    using Omnipaste.Models;

    public interface IContactRelatedActivityViewModel : IActivityViewModel
    {
        IContactModel Contact { get; set; }
    }
}