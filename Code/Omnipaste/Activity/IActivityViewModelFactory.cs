namespace Omnipaste.Activity
{
    using Omnipaste.Models;

    public interface IActivityViewModelFactory
    {
        IActivityViewModel Create(ActivityModel activity);
    }
}