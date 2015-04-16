namespace Omnipaste.Activities.ActivityList
{
    using Omnipaste.Activities.ActivityList.Activity;
    using Omnipaste.Framework.Models;

    public interface IActivityViewModelFactory
    {
        IActivityViewModel Create(ActivityModel activity);
    }
}