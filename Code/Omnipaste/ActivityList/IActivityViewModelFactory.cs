namespace Omnipaste.ActivityList
{
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Models;

    public interface IActivityViewModelFactory
    {
        IActivityViewModel Create(ActivityModel activity);
    }
}