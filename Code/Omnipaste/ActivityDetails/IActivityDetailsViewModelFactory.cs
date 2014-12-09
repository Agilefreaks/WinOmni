namespace Omnipaste.ActivityDetails
{
    using Omnipaste.Activity.Models;

    public interface IActivityDetailsViewModelFactory
    {
        IActivityDetailsViewModel Create(Activity activity);
    }
}