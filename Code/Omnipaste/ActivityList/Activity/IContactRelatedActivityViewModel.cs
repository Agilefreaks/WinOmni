namespace Omnipaste.ActivityList.Activity
{
    using Omnipaste.Framework.Models;

    public interface IContactRelatedActivityViewModel : IActivityViewModel
    {
        ContactModel Contact { get; set; }
    }
}