namespace Omnipaste.ActivityList
{
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Framework.Models;

    public interface IActivityViewModelFactory
    {
        IActivityViewModel Create(ActivityModel activity);
    }
}