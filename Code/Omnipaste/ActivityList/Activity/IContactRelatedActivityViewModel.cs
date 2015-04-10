namespace Omnipaste.ActivityList.Activity
{
    using Omnipaste.Models;

    public interface IContactRelatedActivityViewModel : IActivityViewModel
    {
        ContactModel Contact { get; set; }
    }
}